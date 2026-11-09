using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseSubmenu : MonoBehaviour
{
    private VisualElement _menuContainer;
    private BaseMenu _baseMenu;

    public VisualElement Container => _menuContainer;

    protected void SwitchSubmenu<T>() where T: BaseSubmenu
    {
        _baseMenu.ShowSubMenu<T>();
    }

    public abstract void OnDisplaySubmenu();

    public void Open()
    {
        OnDisplaySubmenu();
        _menuContainer.visible = true;
    }

    public void Initialize(BaseMenu baseMenu, VisualElement menuContainer)
    {
        _baseMenu = baseMenu;
        _menuContainer = menuContainer;
    }
}
