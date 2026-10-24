using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class CreditsMenu : BaseSubmenu
{
    Button _returnButton;
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
        _returnButton = rootElement.Q<Button>("CreditsReturnButton");
        _returnButton.clicked += OnReturnButtonClicked;
    }

    void OnReturnButtonClicked()
    {
        SwitchSubmenu<StartMenu>();
    }
}
