using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseMenu : MonoBehaviour
{
    private const string MENU_BUTTON_FADED = "menuButtonFaded";

    [SerializeField] string containerName;

    private List<BaseSubmenu> _submenus;
    private VisualElement _overlayContainer;
    private VisualElement _root;

    public VisualElement OverlayContainer => _overlayContainer;

    protected void Initialize(VisualElement root, params SubmenuData[] submenusData)
    {
        _root = root;
        _overlayContainer = GetContainer(containerName);
        _submenus = new();

        foreach (SubmenuData menuData in submenusData)
        {
            menuData.menu.Initialize(
                this,
                GetContainer(menuData.containerName),
                GetAdditionalElements(menuData.additionalElements)
            );

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
            menu.Close();
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

            menu.Close();
        }
    }

    private VisualElement GetContainer(string containerName)
    {
        VisualElement container = _root.Q<VisualElement>(containerName);

        if (container == null)
        {
            Debug.LogError(
                $"Container with name <color=white>{containerName}</color> " +
                $"could not be found in root visual element " +
                $"<color=white>{_root.name}</color>");
        }

        return container;
    }

    private VisualElement[] GetAdditionalElements(IEnumerable<string> names)
    {
        VisualElement[] result = new VisualElement[names.Count()];

        int i = 0;

        foreach (var name in names)
        {
            result[i] =  _root.Query<VisualElement>(name);
            i++;
        }

        return result;
    }
}


[System.Serializable]
public struct SubmenuData
{
    public BaseSubmenu menu;
    public string containerName;
    public List<string> additionalElements;
}
