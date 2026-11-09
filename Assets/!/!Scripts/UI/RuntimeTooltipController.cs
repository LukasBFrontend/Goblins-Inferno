
using UnityEngine;
using UnityEngine.UIElements;
using Luscious;

[RequireComponent(typeof(PanelRenderer))]
public class RuntimeTooltipController : MonoBehaviour
{
    [SerializeField] SO_CursorSettings cursorSettings;
    [Header("Tooltip Settings")]
    [SerializeField] long showDelayMs = 500;
    [SerializeField] Vector2 offset = new(20, 20);
    [SerializeField] bool enableAutoFlip = true;
    [SerializeField] TooltipDirection defaultDirection = TooltipDirection.Bottom;

    private PanelRenderer _panelRenderer;
    private VisualElement _root;
    private RuntimeTooltip _tooltip;

    private IVisualElementScheduledItem _showSchedule;
    private VisualElement _currentTarget;

    private void OnEnable()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    private void OnDisable()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);

        Hide();

        _tooltip?.RemoveFromHierarchy();
        _tooltip = null;
        _root = null;
    }

    private void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        Hide();
        _tooltip?.RemoveFromHierarchy();

        _root = rootElement;
        _tooltip = new RuntimeTooltip
        {
            showDelay = showDelayMs,
            offset = offset,
            enableAutoFlip = enableAutoFlip,
            defaultDirection = defaultDirection
        };

        _tooltip.style.display = DisplayStyle.None;

        _root.Add(_tooltip);

        _root.RegisterCallback<PointerEnterEvent>(OnPointerEnter, TrickleDown.TrickleDown);
        _root.RegisterCallback<PointerMoveEvent>(OnPointerMove, TrickleDown.TrickleDown);
        _root.RegisterCallback<PointerLeaveEvent>(OnPointerLeave, TrickleDown.TrickleDown);
    }

    private void OnPointerEnter(PointerEnterEvent evt)
    {
        VisualElement target = FindTooltipTarget(evt.target as VisualElement);

        if (target == null || target == _currentTarget)
        {
            return;
        }

        ShowDelayed(target);

        CursorManager.SetCursor(cursorSettings.progressCursor);
    }

    private void OnPointerMove(PointerMoveEvent evt)
    {
        VisualElement target = FindTooltipTarget(evt.target as VisualElement);

        if (target != _currentTarget)
        {
            if (target == null)
            {
                Hide();
                return;
            }

            ShowDelayed(target);
        }
    }

    private void OnPointerLeave(PointerLeaveEvent evt)
    {
        Hide();
    }

    private VisualElement FindTooltipTarget(VisualElement element)
    {
        while (element != null && element != _root)
        {
            if (!string.IsNullOrEmpty(element.tooltip))
            {
                return element;
            }

            element = element.parent;
        }

        return null;
    }

    private void ShowDelayed(VisualElement target)
    {
        Hide();

        _currentTarget = target;
        _tooltip.text = target.tooltip;

        CursorManager.SetCursor(cursorSettings.progressCursor);

        _showSchedule = _root.schedule.Execute(() =>
        {
            if (_currentTarget == null)
            {
                return;
            }

            _tooltip.UpdatePosition(_currentTarget, _root);
            _tooltip.style.display = DisplayStyle.Flex;
            CursorManager.SetCursor(cursorSettings.focusCursor);
        }).StartingIn(_tooltip.showDelay);
    }

    private void Hide()
    {
        if (_tooltip != null)
        {
            _tooltip.style.display = DisplayStyle.None;
        }

        _showSchedule?.Pause();
        _showSchedule = null;

        _currentTarget = null;

        CursorManager.SetCursor(cursorSettings.defaultCursor);

    }
}
