using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(OptionsMenu))]
[RequireComponent(typeof(CreditsMenu))]
[RequireComponent(typeof(PanelRenderer))]
public class MainMenuHandler : BaseMenu
{
    private const string START_MENU_CONTAINER_FADED = "mainMenuContainerFaded";

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
        ShowMenu();
        OverlayContainer.schedule.Execute(() => OverlayContainer.RemoveFromClassList(START_MENU_CONTAINER_FADED)).StartingIn(5);
        _highscoreText = rootElement.Q<Label>("HighscoreText");
        _highscoreText.text = GameSaveState.Score.HighScore.ToString();

        var buttonPressedSound = Game.SoundFX.Default.buttonPressSound;
        rootElement.Query<Button>().ForEach(
            button => button.clicked += () => Game.SoundFX.PlaySoundFXClip(
                buttonPressedSound,
                transform.position
            )
        );

    }
}
