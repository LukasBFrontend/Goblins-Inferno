using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseMenu : BaseSubmenu {
    Button _resumeButton, _optionsButton, _quitButton;

    void OnEnable()
    {
        GetComponent<PanelRenderer>().RegisterUIReloadCallback(OnUIReload);
    }

    void OnDisable()
    {
        GetComponent<PanelRenderer>().UnregisterUIReloadCallback(OnUIReload);
        // _resumeButton.clicked -= OneResumeButtonClicked;
        // _optionsButton.clicked -= OnOptionsButtonClicked;
        // _quitButton.clicked -= OnQuitButtonClicked;
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        // _resumeButton = rootElement.Q<TemplateContainer>("ResumeButton").Q<Button>();
        // _optionsButton = rootElement.Q<TemplateContainer>("OptionsButton").Q<Button>();
        // _quitButton = rootElement.Q<TemplateContainer>("QuitButton").Q<Button>();

        // _resumeButton.clicked += OneResumeButtonClicked;
        // _optionsButton += OnOptionsButtonClicked;
        // _quitButton.clicked += OnQuitButtonClicked;
    }

    void OneResumeButtonClicked()
    {
        Game.UpgradeOptionManager.SelectOptionOne();
    }

    void OnOptionsButtonClicked()
    {
        Game.UpgradeOptionManager.SelectOptionTwo();
    }

    void OnQuitButtonClicked()
    {
        Game.UpgradeOptionManager.SelectOptionThree();
    }
}
