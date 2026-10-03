using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class StartMenu : BaseSubmenu
{
    Button _startButton, _optionsButton, _creditsButton, _quitButton;

    void OnEnable()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(OnUIReload);
        _startButton.clicked -= OnStartButtonClicked;
        _optionsButton.clicked -= OnOptionsButtonClicked;
        _creditsButton.clicked -= OnCreditsButtonClicked;
        _quitButton.clicked -= OnQuitButtonClicked;
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
        SceneManager.LoadScene(1);
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
