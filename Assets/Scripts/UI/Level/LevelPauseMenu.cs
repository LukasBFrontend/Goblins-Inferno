using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseMenu : BaseSubmenu
{
    [SerializeField] RSO_CharacterStatRows characterStatRows;
    Button _resumeButton, _optionsButton;
    PanelRenderer _panelRenderer;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        GameEvents.GameResumed.AddListener(OnGameResumed);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _resumeButton = rootElement.Q<Button>("ResumeButton");
        _optionsButton = rootElement.Q<Button>("OptionsButton");

        _resumeButton.clicked += OneResumeButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;
    }

    void Start()
    {
        characterStatRows.Populate();
    }

    void OneResumeButtonClicked()
    {
        GameEvents.RaiseGameResumed();
    }

    void OnOptionsButtonClicked()
    {
        //SwitchSubmenu<OptionsMenu>();
    }

    void OnGameResumed()
    {
        characterStatRows.Populate();
    }
}
