using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class StartMenu : BaseSubmenu
{
    Button _startButton, _optionsButton, _creditsButton, _quitButton;
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
        _startButton = rootElement.Q<Button>("StartButton");
        _optionsButton = rootElement.Q<Button>("OptionsButton");
        _creditsButton = rootElement.Q<Button>("CreditsButton");
        _quitButton = rootElement.Q<Button>("QuitButton");

        _startButton.clicked += OnStartButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;
        _creditsButton.clicked += OnCreditsButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
    }

    void OnStartButtonClicked()
    {
        GameEvents.RaiseLevelStarted();
    }
    void OnOptionsButtonClicked()
    {
        SwitchSubmenu<OptionsMenu>();
    }

    void OnCreditsButtonClicked()
    {
        SwitchSubmenu<CreditsMenu>();
    }

    void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
