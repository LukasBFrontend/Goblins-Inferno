using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(LevelPauseMenu))]
[RequireComponent(typeof(LevelUpgradeMenu))]
[RequireComponent(typeof(GameOverMenu))]
[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseOverlayHandler : BaseMenu
{
    [Header("Menus")]
    [SerializeField] SubmenuData levelUpgradeData;
    [SerializeField] SubmenuData levelPauseData;
    [SerializeField] SubmenuData gameoverMenuData;
    VisualElement _hudContainer, _sideContainer;
    PanelRenderer _panelRenderer;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);

        GameEvents.LvlUpEvent.AddListener(OnLvlUp);
        GameEvents.GamePaused.AddListener(OnGamePaused);
        GameEvents.GameResumed.AddListener(OnGameResumed);
        GameEvents.GameOverTranitionCompleted.AddListener(OnGameOverTransitionCompleted);
        GameEvents.GameOverEvent.AddListener(OnGameOver);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        GameEvents.LvlUpEvent.RemoveListener(OnLvlUp);
        GameEvents.GamePaused.RemoveListener(OnGamePaused);
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
        GameEvents.GameOverTranitionCompleted.RemoveListener(OnGameOverTransitionCompleted);
        GameEvents.GameOverEvent.RemoveListener(OnGameOver);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        Initialize(rootElement, levelPauseData, levelUpgradeData, gameoverMenuData);

        rootElement.Query<Button>("QuitButton").ForEach(button =>
            button.clicked += OnQuitButtonClicked
        );

        _hudContainer = rootElement.Q<VisualElement>("HUDContainer");
        _sideContainer = rootElement.Q<VisualElement>("SideContainer");
    }

    void OnLvlUp(int oldLvl, int newLvl)
    {
        ShowMenu();
        ShowSubMenu<LevelUpgradeMenu>();
    }

    void OnGameOver()
    {
        _hudContainer.visible = false;
        _sideContainer.style.display = DisplayStyle.None;
    }

    void OnGameOverTransitionCompleted()
    {
        ShowMenu();
        ShowSubMenu<GameOverMenu>();
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

    void OnQuitButtonClicked()
    {
        GameEvents.RaiseQuitLevel();
    }

}
