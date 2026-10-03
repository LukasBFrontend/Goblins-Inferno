using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseSubmenu : MonoBehaviour
{
    VisualElement _menuContainer;
    public VisualElement Container => _menuContainer;
    VisualElement _overlayContainer;

    protected void CloseOverlay()
    {
        _overlayContainer.visible = false;
        _menuContainer.visible = false;
    }

    public void Initialize(VisualElement overlayContainer, VisualElement menuContainer)
    {
        _overlayContainer = overlayContainer;
        _menuContainer = menuContainer;
    }
}
