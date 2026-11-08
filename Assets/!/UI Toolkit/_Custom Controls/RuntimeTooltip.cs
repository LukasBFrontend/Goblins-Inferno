
using UnityEngine;
using UnityEngine.UIElements;

public enum TooltipDirection
{
    Top,
    Bottom,
    Left,
    Right
}

[UxmlElement]
public partial class RuntimeTooltip : VisualElement
{
    private Label _label;

    public RuntimeTooltip()
    {
        AddToClassList("runtime-tooltip");
        _label = new Label();
        _label.AddToClassList("runtime-tooltip__label");
        Add(_label);

        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        text = "Tooltip Text";
    }

    [UxmlAttribute]
    public string text
    {
        get => _label.text;
        set => _label.text = value;
    }

    [UxmlAttribute]
    public long showDelay { get; set; } = 500;

    [UxmlAttribute]
    public Vector2 offset { get; set; } = new(20, 20);

    [UxmlAttribute]
    public bool enableAutoFlip { get; set; } = true;

    [UxmlAttribute]
    public TooltipDirection defaultDirection { get; set; } = TooltipDirection.Bottom;

    public void UpdatePosition(VisualElement target, VisualElement root)
    {
        if (target == null || root == null)
            return;

        Rect bounds = target.worldBound;
        Vector2 position = root.WorldToLocal(bounds.position);

        float width = resolvedStyle.width;
        float height = resolvedStyle.height;

        float panelWidth = root.resolvedStyle.width;
        float panelHeight = root.resolvedStyle.height;

        float x = position.x;
        float y = position.y;

        switch (defaultDirection)
        {
            case TooltipDirection.Top:
                x += (bounds.width - width) * 0.5f;
                y -= height + offset.y;
                break;

            case TooltipDirection.Bottom:
                x += (bounds.width - width) * 0.5f;
                y += bounds.height + offset.y;
                break;

            case TooltipDirection.Left:
                x -= width + offset.x;
                y += (bounds.height - height) * 0.5f;
                break;

            case TooltipDirection.Right:
                x += bounds.width + offset.x;
                y += (bounds.height - height) * 0.5f;
                break;
        }

        if (enableAutoFlip)
        {
            switch (defaultDirection)
            {
                case TooltipDirection.Top:
                    if (y < 0)
                        y = position.y + bounds.height + offset.y;
                    break;

                case TooltipDirection.Bottom:
                    if (y + height > panelHeight)
                        y = position.y - height - offset.y;
                    break;

                case TooltipDirection.Left:
                    if (x < 0)
                        x = position.x + bounds.width + offset.x;
                    break;

                case TooltipDirection.Right:
                    if (x + width > panelWidth)
                        x = position.x - width - offset.x;
                    break;
            }
        }

        style.left = Mathf.Clamp(x, 0, Mathf.Max(0, panelWidth - width));
        style.top = Mathf.Clamp(y, 0, Mathf.Max(0, panelHeight - height));
    }
}
