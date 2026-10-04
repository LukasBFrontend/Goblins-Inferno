using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class OptionsMenu : BaseSubmenu
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
        _returnButton = rootElement.Q<Button>("OptionsReturnButton");
        _returnButton.clicked += OnReturnButtonClicked;
    }

    void OnReturnButtonClicked()
    {
        // TODO: Replace with more general purpose solution or seperate OptionsMenu into two components.

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        if (sceneIndex == 0)
        {
            SwitchSubmenu<StartMenu>();
        }
        else if (sceneIndex == 1)
        {
            SwitchSubmenu<LevelPauseMenu>();
        }
    }
}
