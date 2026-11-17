using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseMenu : BaseSubmenu
{
    [SerializeField] RSO_CharacterStatRows characterStatRows;
    Button _resumeButton, _optionsButton;
    Label _currentHighScoreLabel;
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
        _currentHighScoreLabel = rootElement.Q<Label>("CurrentHighscoreValue");

        _resumeButton.clicked += OneResumeButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;

        _currentHighScoreLabel.text = GameSaveState.Score.HighScore.ToString();
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

    public override void OnDisplaySubmenu(){ }
}
