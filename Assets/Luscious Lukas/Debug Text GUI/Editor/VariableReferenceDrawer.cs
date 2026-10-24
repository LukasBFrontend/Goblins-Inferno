using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(VariableReference))]
public class VariableReferenceDrawer : PropertyDrawer {
    const float SPACING = 4f;
    static List<Type> cachedStaticTypes;

    static float Line => EditorGUIUtility.singleLineHeight;
    static List<Type> StaticTypes => cachedStaticTypes ??= FindStaticClasses();

    static List<Type> FindStaticClasses()
    {
        var result = new List<Type>();

        foreach (string guid in AssetDatabase.FindAssets("t:MonoScript", new[] { "Assets" }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            if (script == null)
            {
                continue;
            }

            Type type = script.GetClass();

            bool skipType =
                type == null
                || !type.IsClass
                || !type.IsAbstract
                || !type.IsSealed
                || type.ContainsGenericParameters
            ;

            if (skipType)
            {
                continue;
            }

            const BindingFlags flags =
                BindingFlags.Public
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy
            ;

            if (GetMembers(type, flags).Count > 0)
            {
                result.Add(type);
            }
        }

        return result
            .Distinct()
            .OrderBy(t => t.FullName)
            .ToList()
        ;
    }

    class MemberEntry
    {
        public string Name;
        public Type ValueType;
    }

    static List<MemberEntry> GetMembers(Type type, BindingFlags flags)
    {
        var result = new List<MemberEntry>();

        if (type == null)
        {
            return result;
        }

        foreach (var field in type.GetFields(flags))
        {
            if (field.IsSpecialName)
            {
                continue;
            }

            result.Add(new MemberEntry
            {
                Name = field.Name,
                ValueType = field.FieldType
            });
        }

        foreach (var property in type.GetProperties(flags))
        {
            var getter = property.GetGetMethod();

            if (getter == null || !getter.IsPublic || property.GetIndexParameters().Length != 0)
            {
                continue;
            }

            result.Add(new MemberEntry
            {
                Name = property.Name,
                ValueType = property.PropertyType
            });
        }

        return result
            .GroupBy(m => m.Name)
            .Select(g => g.First())
            .OrderBy(m => m.Name)
            .ToList()
        ;
    }

    static bool CanExpand(Type type)
    {
        if (type == null)
        {
            return false;
        }

        type = Nullable.GetUnderlyingType(type) ?? type;

        bool result =
            !type.IsPrimitive &&
            !type.IsEnum &&
            type != typeof(string) &&
            type != typeof(decimal) &&
            type != typeof(DateTime) &&
            type != typeof(TimeSpan) &&
            !type.IsArray &&
            !typeof(UnityEngine.Object).IsAssignableFrom(type) &&
            !typeof(System.Collections.IEnumerable).IsAssignableFrom(type)
        ;

        return result;
    }

    private static string StateKey(SerializedProperty property, Type rootType, string path)
    {
        return "VariableReferenceDrawer:"
            + property.serializedObject.targetObject.GetEntityId()
            + ":" + property.propertyPath
            + ":" + rootType.AssemblyQualifiedName
            + ":" + path
        ;
    }

    private static bool IsExpanded(SerializedProperty property, Type rootType, string path)
    {
        return SessionState.GetBool(
            StateKey(property, rootType, path), false);
    }

    private static void SetExpanded(
        SerializedProperty property,
        Type rootType,
        string path,
        bool expanded)
    {
        SessionState.SetBool(StateKey(property, rootType, path), expanded);
    }

    private static bool HasChildren(
        Type type,
        SerializedProperty property,
        Type rootType,
        string path,
        int depth,
        HashSet<Type> ancestors)
    {
        if (!CanExpand(type) || depth >= 5)
        {
            return false;
        }

        if (ancestors.Contains(type))
        {
            return false;
        }

        ancestors.Add(type);

        var members = GetMembers(
            type,
            BindingFlags.Public |
            BindingFlags.Instance |
            BindingFlags.FlattenHierarchy)
        ;

        bool hasChildren = members.Count > 0;

        ancestors.Remove(type);

        return hasChildren;
    }

    private static int CountVisibleRows(
        SerializedProperty property,
        Type rootType,
        Type currentType,
        string prefix,
        bool rootStatic,
        int depth,
        HashSet<Type> ancestors)
    {
        if (currentType == null || depth > 5)
        {
            return 0;
        }

        BindingFlags flags =
            BindingFlags.Public
            | BindingFlags.FlattenHierarchy
            | (rootStatic ? BindingFlags.Static : BindingFlags.Instance)
        ;

        int count = 0;

        foreach (var member in GetMembers(currentType, flags))
        {
            string path = string.IsNullOrEmpty(prefix)
                ? member.Name
                : prefix + "." + member.Name
            ;

            count++;

            if (!CanExpand(member.ValueType) ||
                !IsExpanded(property, rootType, path) ||
                depth >= 5 ||
                ancestors.Contains(member.ValueType))
            {
                continue;
            }

            var childMembers = GetMembers(
                member.ValueType,
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.FlattenHierarchy)
            ;

            if (childMembers.Count == 0)
            {
                continue;
            }

            ancestors.Add(member.ValueType);

            count += CountVisibleRows(
                property,
                rootType,
                member.ValueType,
                path,
                false,
                depth + 1,
                ancestors)
            ;

            ancestors.Remove(member.ValueType);
        }

        return count;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var sourceProp = property.FindPropertyRelative("source");
        var targetProp = property.FindPropertyRelative("target");
        var typeNameProp = property.FindPropertyRelative("staticTypeName");

        var source = (VariableSource)sourceProp.enumValueIndex;

        Type rootType = null;

        if (source == VariableSource.Static)
        {
            rootType = StaticTypes.FirstOrDefault(
                t => t.AssemblyQualifiedName == typeNameProp.stringValue)
            ;
        }
        else
        {
            var target = targetProp.objectReferenceValue as MonoBehaviour;

            if (target != null)
            {
                rootType = target.GetType();
            }
        }

        int rows = 3;

        if (rootType != null)
        {
            rows += CountVisibleRows(
                property,
                rootType,
                rootType,
                "",
                source == VariableSource.Static,
                0,
                new HashSet<Type>())
            ;
        }

        return rows * (Line + SPACING) - SPACING;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var sourceProp = property.FindPropertyRelative("source");
        var targetProp = property.FindPropertyRelative("target");
        var typeNameProp = property.FindPropertyRelative("staticTypeName");
        var memberProp = property.FindPropertyRelative("memberName");

        float y = position.y;

        Rect NextRow(int depth = 0)
        {
            var rect = new Rect(
                position.x + depth * 14f,
                y,
                position.width - depth * 14f,
                Line)
            ;

            y += Line + SPACING;

            return rect;
        }

        EditorGUI.PropertyField(NextRow(), sourceProp, new GUIContent("Source"));

        var source = (VariableSource)sourceProp.enumValueIndex;

        Type rootType = null;

        if (source == VariableSource.Instance)
        {
            EditorGUI.PropertyField(NextRow(), targetProp, new GUIContent("Component"));

            var target = targetProp.objectReferenceValue as MonoBehaviour;

            if (target != null)
            {
                rootType = target.GetType();
            }
        }
        else
        {
            var types = StaticTypes;

            var options = types
                .Select(t => t.FullName)
                .ToArray()
            ;

            var currentType = types.FirstOrDefault(t => t.AssemblyQualifiedName == typeNameProp.stringValue);

            int selectedIndex = types.IndexOf(currentType);

            int nextIndex = EditorGUI.Popup(
                NextRow(),
                "Static Class",
                selectedIndex,
                options)
            ;

            if (nextIndex >= 0 && nextIndex < types.Count)
            {
                rootType = types[nextIndex];

                if (typeNameProp.stringValue != rootType.AssemblyQualifiedName)
                {
                    typeNameProp.stringValue = rootType.AssemblyQualifiedName;
                    memberProp.stringValue = "";
                }
            }
        }

        EditorGUI.LabelField(NextRow(), "Members", EditorStyles.boldLabel);

        if (rootType == null)
        {
            EditorGUI.LabelField(NextRow(1), "Select a source first");

            EditorGUI.EndProperty();

            return;
        }

        DrawMembers(
            property,
            rootType,
            rootType,
            "",
            memberProp,
            source == VariableSource.Static,
            0,
            new HashSet<Type>(),
            position,
            ref y
        );

        EditorGUI.EndProperty();
    }

    private void DrawMembers(
        SerializedProperty property,
        Type rootType,
        Type currentType,
        string prefix,
        SerializedProperty selectedPath,
        bool rootStatic,
        int depth,
        HashSet<Type> ancestors,
        Rect position,
        ref float y)
    {
        if (currentType == null || depth > 5)
        {
            return;
        }

        BindingFlags flags =
            BindingFlags.Public
            | BindingFlags.FlattenHierarchy
            | (rootStatic ? BindingFlags.Static : BindingFlags.Instance)
        ;

        foreach (var member in GetMembers(currentType, flags))
        {
            string path = string.IsNullOrEmpty(prefix)
                ? member.Name
                : prefix + "." + member.Name
            ;

            bool expandable = false;

            if (CanExpand(member.ValueType) &&
                depth < 5 &&
                !ancestors.Contains(member.ValueType))
            {
                expandable = GetMembers(
                    member.ValueType,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.FlattenHierarchy).Count > 0
                ;
            }

            Rect row = new(
                position.x + (depth + 1) * 14f,
                y,
                position.width - (depth + 1) * 14f,
                Line)
            ;

            y += Line + SPACING;

            if (expandable)
            {
                bool expanded = IsExpanded(property, rootType, path);

                Rect foldoutRect = new(
                    row.x,
                    row.y,
                    14f,
                    row.height)
                ;

                bool nextExpanded = EditorGUI.Foldout(
                    foldoutRect,
                    expanded,
                    GUIContent.none,
                    true)
                ;

                if (nextExpanded != expanded)
                {
                    SetExpanded(property, rootType, path, nextExpanded);
                }
            }

            Rect labelRect = new(
                row.x + (expandable ? 14f : 0f),
                row.y,
                row.width - (expandable ? 14f : 0f),
                row.height)
            ;

            bool isSelected = selectedPath.stringValue == path;

            Rect indicatorRect = new(
                labelRect.x,
                labelRect.y,
                14f,
                labelRect.height)
            ;

            Rect textRect = new(
                labelRect.x + 14f,
                labelRect.y,
                labelRect.width - 14f,
                labelRect.height)
            ;

            if (isSelected)
            {
                EditorGUI.DrawRect(labelRect, new Color(0.18f, 0.42f, 0.78f, 0.35f));

                EditorGUI.LabelField(indicatorRect, "✓", EditorStyles.boldLabel);
            }

            var style = isSelected ? EditorStyles.boldLabel : EditorStyles.label;

            if (GUI.Button(textRect, member.Name, style))
            {
                selectedPath.stringValue = path;
            }

            if (expandable && IsExpanded(property, rootType, path))
            {
                ancestors.Add(member.ValueType);

                DrawMembers(
                    property,
                    rootType,
                    member.ValueType,
                    path,
                    selectedPath,
                    false,
                    depth + 1,
                    ancestors,
                    position,
                    ref y)
                ;

                ancestors.Remove(member.ValueType);
            }
        }
    }
}
