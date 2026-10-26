
using System;
using System.Reflection;
using UnityEngine;

namespace Luscious.Debug
{
    public enum VariableSource
    {
        Instance,
        Static
    }

    [Serializable]
    public class VariableReference
    {
        public VariableSource source;
        public MonoBehaviour target;
        public string staticTypeName;
        public string memberName;

        public object GetValue()
        {
            Type type;
            object current;

            if (source == VariableSource.Static)
            {
                type = string.IsNullOrEmpty(staticTypeName) ? null : Type.GetType(staticTypeName);

                current = null;
            }
            else
            {
                current = target;
                type = target != null ? target.GetType() : null;
            }

            if (type == null || string.IsNullOrEmpty(memberName))
            {
                return null;
            }

            const BindingFlags flags
                = BindingFlags.Public
                | BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.FlattenHierarchy
            ;

            string[] parts = memberName.Split('.');

            for (int i = 0; i < parts.Length; i++)
            {
                if (i > 0 && current == null)
                {
                    return null;
                }

                type = i == 0 ? type : current.GetType();

                var field = type.GetField(parts[i], flags);

                if (field != null)
                {
                    if (i == 0 && source == VariableSource.Static && !field.IsStatic)
                    {
                        return null;
                    }

                    current = field.GetValue(field.IsStatic ? null : current);

                    continue;
                }

                var property = type.GetProperty(parts[i], flags);

                if (property == null || property.GetIndexParameters().Length != 0)
                {
                    return null;
                }

                var getter = property.GetGetMethod();

                if (getter == null)
                {
                    return null;
                }

                if (i == 0 && source == VariableSource.Static && !getter.IsStatic)
                {
                    return null;
                }

                current = property.GetValue(getter.IsStatic ? null : current);
            }

            return current;
        }
    }
}
