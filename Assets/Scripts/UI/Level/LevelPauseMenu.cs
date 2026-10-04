using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseMenu : BaseSubmenu
{
    Button _resumeButton, _optionsButton;
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
        _resumeButton = rootElement.Q<Button>("ResumeButton");
        _optionsButton = rootElement.Q<Button>("OptionsButton");

        _resumeButton.clicked += OneResumeButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;
    }

    void OneResumeButtonClicked()
    {
        GameEvents.RaiseGameResumed();
    }

    void OnOptionsButtonClicked()
    {
        throw new System.NotImplementedException("The options menu UI is not yet implemented.");
        //SwitchSubmenu<OptionsMenu>();
    }
}
