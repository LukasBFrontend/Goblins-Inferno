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
    [SerializeField] string containerName;
    List<BaseSubmenu> _submenus;
    VisualElement _overlayContainer;
    public VisualElement OverlayContainer => _overlayContainer;

    protected void Initialize(VisualElement root, params SubmenuData[] submenusData)
    {
        _overlayContainer = GetContainer(root, containerName);

        _submenus = new();

        foreach (var menuData in submenusData)
        {
            menuData.menu.Initialize(this, GetContainer(root, menuData.containerName));
            _submenus.Add(menuData.menu);
        }
    }

    protected void ShowMenu()
    {
        _overlayContainer.visible = true;
    }

    protected void HideMenu()
    {
        _overlayContainer.visible = false;

        foreach (var menu in _submenus)
        {
            menu.Container.visible = false;
        }
    }

    public void ShowSubMenu<T>() where T: BaseSubmenu
    {
        foreach (var menu in _submenus)
        {
            menu.Container.visible = menu is T;
        }
    }

    VisualElement GetContainer(VisualElement root, string containerName)
    {
        var container = root.Q<VisualElement>(containerName);

        if (container == null)
        {
            Debug.LogError($"Container with name <color=white>{containerName}</color> could not be found in root visual element <color=white>{root.name}</color>");
        }

        return container;
    }
}
