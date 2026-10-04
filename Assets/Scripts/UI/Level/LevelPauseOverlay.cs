using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LevelPauseMenu))]
[RequireComponent(typeof(LevelUpgradeMenu))]
[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseOverlayHandler : BaseMenu
{
    [Header("Menus")]
    [SerializeField] SubmenuData levelUpgradeData;
    [SerializeField] SubmenuData levelPauseData;
    PanelRenderer _panelRenderer;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);

        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GamePaused.AddListener(OnGamePaused);
        GameEvents.GameResumed.AddListener(OnGameResumed);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GamePaused.RemoveListener(OnGamePaused);
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        Initialize(rootElement, levelPauseData, levelUpgradeData);
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        ShowMenu();
        ShowSubMenu<LevelUpgradeMenu>();
    }

    void OnGamePaused()
    {
        ShowMenu();
        ShowSubMenu<LevelPauseMenu>();
    }

    void OnGameResumed()
    {
        HideMenu();
    }

}
