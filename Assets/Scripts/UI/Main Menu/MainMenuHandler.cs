using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(OptionsMenu))]
[RequireComponent(typeof(CreditsMenu))]
[RequireComponent(typeof(PanelRenderer))]
public class MainMenuHandler : BaseMenu
{
    [Header("Menus")]
    [SerializeField] SubmenuData startMenuData;
    [SerializeField] SubmenuData optionsMenuData;
    [SerializeField] SubmenuData creditsMenuData;
    Label _highscoreText;
    PanelRenderer _panelRenderer;

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
        Initialize(rootElement, startMenuData, optionsMenuData, creditsMenuData);
        _highscoreText = rootElement.Q<Label>("HighscoreText");
        _highscoreText.text = Game.HighScore.ToString();
    }
}
