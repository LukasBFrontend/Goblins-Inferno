using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseSubmenu : MonoBehaviour
{
    private VisualElement _menuContainer;
    private BaseMenu _baseMenu;
    private HashSet<VisualElement> _additionalElements;

    public VisualElement Container => _menuContainer;

    protected void SwitchSubmenu<T>() where T: BaseSubmenu
    {
        _baseMenu.ShowSubMenu<T>();
    }

    public abstract void OnOpen();

    public void Initialize(BaseMenu baseMenu, VisualElement menuContainer, VisualElement[] additionalElements)
    {
        _baseMenu = baseMenu;
        _menuContainer = menuContainer;
        _additionalElements = additionalElements.ToHashSet();
    }

    public void Open()
    {
        OnOpen();
        _menuContainer.visible = true;

        foreach (var element in _additionalElements)
        {
            element.style.opacity = 1f;
        }
    }

    public void Close()
    {
        _menuContainer.visible = false;

        foreach (var element in _additionalElements)
        {
            element.style.opacity = 0f;
        }
    }

    public void UnregisterAdditionalElement(VisualElement element)
    {
        if (!_additionalElements.Contains(element))
        {
            throw new System.InvalidOperationException(nameof(UnregisterAdditionalElement));
        }
        _additionalElements.Remove(element);
    }

    public void RegisterAdditionalElement(VisualElement element)
    {
        _additionalElements.Add(element);
    }
}
