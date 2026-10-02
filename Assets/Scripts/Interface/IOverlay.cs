using UnityEngine;
using UnityEngine.UIElements;

public abstract class UIOverlay : MonoBehaviour
{
    [SerializeField] PanelRenderer panelRenderer;
    [SerializeField] string overlayContainerName;
    public PanelRenderer PanelRenderer => panelRenderer;
    VisualElement _overlayContainer;
    public void SetOverlayContainer(VisualElement root)
    {
        _overlayContainer = root.Q<VisualElement>(overlayContainerName);

        if (_overlayContainer == null)
        {
            Debug.LogError($"Overlay container with name {overlayContainerName} could not be found")
        }
    }

    public void Show()
    {
        _overlayContainer.visible = true;
    }

    public void Hide()
    {
        _overlayContainer.visible = false;
    }
}