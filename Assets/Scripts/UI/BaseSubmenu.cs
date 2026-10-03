using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseSubmenu : MonoBehaviour
{
    VisualElement _menuContainer;
    public VisualElement Container => _menuContainer;
    BaseMenu _baseMenu;

    protected void CloseOverlay()
    {
        _baseMenu.OverlayContainer.visible = false;
        _menuContainer.visible = false;
    }

    protected void SwitchSubmenu<T>() where T: BaseSubmenu
    {
        _baseMenu.ShowSubMenu<T>();
    }

    public void Initialize(BaseMenu baseMenu, VisualElement menuContainer)
    {
        _baseMenu = baseMenu;
        _menuContainer = menuContainer;
    }
}
