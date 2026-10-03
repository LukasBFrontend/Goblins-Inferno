using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
struct SubmenuData
{
    public BaseSubmenu menu;
    public string containerName;
}

[RequireComponent(typeof(LevelPauseMenu))]
[RequireComponent(typeof(LevelUpgradeMenu))]
[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseOverlay : MonoBehaviour
{
    [SerializeField] string containerName;
    [Header("Menus")]
    [SerializeField] SubmenuData levelUpgrade;
    [SerializeField] SubmenuData levelPause;
    PanelRenderer _panelRenderer;
    VisualElement _overlayContainer;

    void ShowOverlay()
    {
        _overlayContainer.visible = true;
    }

    void HideOverlay()
    {
        _overlayContainer.visible = false;
    }

    void ShowSubMenu<T>() where T: BaseSubmenu
    {
        List<BaseSubmenu> menus = new(){
            levelPause.menu,
            // TODO: Add levelOptionsMenu
            levelUpgrade.menu
        };

        foreach (var menu in menus)
        {
            menu.Container.visible = menu is T;
        }
    }

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _overlayContainer = GetContainer(rootElement, containerName);

        levelUpgrade.menu.Initialize(_overlayContainer, GetContainer(rootElement, levelUpgrade.containerName));
        levelPause.menu.Initialize(_overlayContainer, GetContainer(rootElement, levelPause.containerName));

        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        ShowOverlay();
        ShowSubMenu<LevelUpgradeMenu>();
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
