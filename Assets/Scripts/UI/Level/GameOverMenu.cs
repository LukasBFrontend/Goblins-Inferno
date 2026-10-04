using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class GameOverMenu : BaseSubmenu
{
    Button _retryButton;
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
        _retryButton = rootElement.Q<Button>("RetryButton");

        _retryButton.clicked += OnRetryButtonClicked;
    }

    void OnRetryButtonClicked()
    {
        GameEvents.RaiseLevelStarted();
    }
}
