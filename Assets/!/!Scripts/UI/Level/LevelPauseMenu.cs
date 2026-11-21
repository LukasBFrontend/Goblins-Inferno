using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class LevelPauseMenu : BaseSubmenu
{
    [SerializeField] RSO_CharacterStatRows characterStatRows;
    Button _resumeButton, _optionsButton;
    Toggle _characterStatsToggle;
    Label _currentHighScoreLabel;
    PanelRenderer _panelRenderer;
    VisualElement _characterStatsContainer;
    bool _displayStatTextWhilePlaying = false;

    void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
        GameEvents.GameResumed.AddListener(OnGameResumed);
        GameEvents.GamePaused.AddListener(OnGamePaused);
    }

    void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
        GameEvents.GameResumed.RemoveListener(OnGameResumed);
        GameEvents.GamePaused.RemoveListener(OnGamePaused);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _resumeButton = rootElement.Q<Button>("ResumeButton");
        _optionsButton = rootElement.Q<Button>("OptionsButton");
        _characterStatsToggle = rootElement.Q<Toggle>("CharacterStatsToggle");
        _currentHighScoreLabel = rootElement.Q<Label>("CurrentHighscoreValue");
        _characterStatsContainer = rootElement.Q<VisualElement>("CharacterStatsContainer");

        _resumeButton.clicked += OneResumeButtonClicked;
        _optionsButton.clicked += OnOptionsButtonClicked;

        _currentHighScoreLabel.text = GameSaveState.Score.HighScore.ToString();
        //_characterStatsToggle.RegisterValueChangedCallback(OnCharactersStatsToggle);
    }

    void Start()
    {
        characterStatRows.Initialize();
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
        characterStatRows.UpdateExisting();

        if (!_displayStatTextWhilePlaying)
        {
            characterStatRows.HideText();
        }
    }

    void OnGamePaused()
    {
        characterStatRows.ShowText();
    }

    public override void OnOpen(){ }
}
