using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class GameOverMenu : BaseSubmenu
{
    Button _retryButton;
    PanelRenderer _panelRenderer;
    Label _finalScoreValue, _finalLevelValue;

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
        _retryButton = rootElement.Q<Button>("RetryButton");
        _finalScoreValue = rootElement.Q<Label>("FinalScoreValue");
        _finalLevelValue = rootElement.Q<Label>("FinalLevelValue");

        _retryButton.clicked += OnRetryButtonClicked;
    }

    void OnRetryButtonClicked()
    {
        GameEvents.RaiseStartLevel();
    }

    public override void OnDisplaySubmenu()
    {
        _finalScoreValue.text = GameSaveState.Score.Current.ToString();
        _finalLevelValue.text = Game.Player.Lvl.ToString();
    }
}
