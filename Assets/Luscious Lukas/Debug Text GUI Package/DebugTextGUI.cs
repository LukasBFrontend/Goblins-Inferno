
using System.Collections.Generic;
using UnityEngine;

public class DebugTextGUI : MonoBehaviour
{
    DebugTextGUISettings settings;

    public void Initialize(DebugTextGUISettings settings)
    {
        this.settings = settings;
    }

    void OnGUI()
    {
        if (settings == null || settings.Enabled == false)
        {
            return;
        }

        List<VariableReference> variables = settings.VariablesToTrack;
        DebugTextGUISettings.StyleData style = settings.Style;

        if (variables == null)
        {
            return;
        }

        float width = style.containerWidth;
        float height = style.containerHeight;

        float totalWidth = width * variables.Count;
        float startX = (Screen.width - totalWidth) / 2f;
        float positionY = Screen.height - height - 20f;

        GUIStyle guiStyle = new(GUI.skin.label)
        {
            fontSize = style.fontSize,
            alignment = TextAnchor.MiddleCenter
        };

        guiStyle.normal.textColor = style.color;

        for (int i = 0; i < variables.Count; i++)
        {
            float positionX = startX + width * i;

            Rect container = new(
                positionX,
                positionY,
                width,
                height
            );

            if (style.showGrid)
            {
                DrawRectOutline(
                    container,
                    style.gridColor,
                    style.gridLineThickness
                );
            }

            VariableReference reference = variables[i];

            GUI.Label(
                container,
                $"{reference.memberName}: [{reference.GetValue()}]",
                guiStyle
            );
        }
    }

    void DrawRectOutline(Rect rect, Color color, float thickness)
    {
        Color previousColor = GUI.color;
        GUI.color = color;

        GUI.DrawTexture(
            new Rect(rect.x, rect.y, rect.width, thickness),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(rect.x, rect.yMax - thickness, rect.width, thickness),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(rect.x, rect.y, thickness, rect.height),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(rect.xMax - thickness, rect.y, thickness, rect.height),
            Texture2D.whiteTexture
        );

        GUI.color = previousColor;
    }
}
