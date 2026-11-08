using UnityEngine.UIElements;

[UxmlElement]
public partial class MenuButton : Button
{
    private readonly Label label;

    public override string text
    {
        get => label.text;
        set => label.text = value;
    }

    public MenuButton()
    {
        AddToClassList("menu-button");
        VisualElement background = new()
        {
            name = "Menu Button Background"
        };
        background.AddToClassList("menu-button-background");

        label = new Label
        {
            name = "Menu Button Label"
        };
        label.AddToClassList("menu-button-label");

        background.Add(label);
        hierarchy.Add(background);
    }
}
