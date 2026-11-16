using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public struct SubmenuData
{
    public BaseSubmenu menu;
    public string containerName;
}

public abstract class BaseMenu : MonoBehaviour
{
    private const string MENU_BUTTON_FADED = "menuButtonFaded";

    [SerializeField] string containerName;

    private List<BaseSubmenu> _submenus;
    private VisualElement _overlayContainer;

    public VisualElement OverlayContainer => _overlayContainer;
    public bool IsVisible => _overlayContainer.style.opacity == 1f;

    protected void Initialize(VisualElement root, params SubmenuData[] submenusData)
    {
        _overlayContainer = GetContainer(root, containerName);

        _submenus = new();

        foreach (SubmenuData menuData in submenusData)
        {
            menuData.menu.Initialize(this, GetContainer(root, menuData.containerName));
            _submenus.Add(menuData.menu);
        }

        OverlayContainer.Query<Button>().ForEach(button =>
        {
            button.AddToClassList(MENU_BUTTON_FADED);
        });
    }

    protected void ShowMenu()
    {
        _overlayContainer.style.opacity = 1f;

        OverlayContainer.Query<Button>().ForEach(button =>
        {
            button.RemoveFromClassList(MENU_BUTTON_FADED);
        });
    }

    protected void HideMenu()
    {
        _overlayContainer.style.opacity = 0f;

        foreach (BaseSubmenu menu in _submenus)
        {
            menu.Container.visible = false;
        }

        OverlayContainer.Query<Button>().ForEach(button =>
        {
            button.AddToClassList(MENU_BUTTON_FADED);
        });
    }

    public void ShowSubMenu<T>() where T : BaseSubmenu
    {
        foreach (BaseSubmenu menu in _submenus)
        {
            if (menu is T)
            {
                menu.Open();
                continue;
            }

            menu.Container.visible = false;
        }
    }

    VisualElement GetContainer(VisualElement root, string containerName)
    {
        VisualElement container = root.Q<VisualElement>(containerName);

        if (container == null)
        {
            Debug.LogError(
                $"Container with name <color=white>{containerName}</color> " +
                $"could not be found in root visual element " +
                $"<color=white>{root.name}</color>");
        }

        return container;
    }
}
