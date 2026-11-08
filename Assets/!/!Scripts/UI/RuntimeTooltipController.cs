
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(PanelRenderer))]
public class RuntimeTooltipController : MonoBehaviour
{
    [Header("Tooltip Settings")]
    [SerializeField] private long _showDelay = 500;
    [SerializeField] private Vector2 _offset = new(20, 20);
    [SerializeField] private bool _enableAutoFlip = true;
    [SerializeField] private TooltipDirection _defaultDirection = TooltipDirection.Bottom;

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
            showDelay = _showDelay,
            offset = _offset,
            enableAutoFlip = _enableAutoFlip,
            defaultDirection = _defaultDirection
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
            return;

        ShowDelayed(target);
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

        if (_tooltip.style.display == DisplayStyle.Flex)
            _tooltip.UpdatePosition(_currentTarget, _root);
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
                return element;

            element = element.parent;
        }

        return null;
    }

    private void ShowDelayed(VisualElement target)
    {
        Hide();

        _currentTarget = target;
        _tooltip.text = target.tooltip;

        _showSchedule = _root.schedule.Execute(() =>
        {
            if (_currentTarget == null)
                return;

            _tooltip.style.display = DisplayStyle.Flex;
            _tooltip.schedule.Execute(() => _tooltip.UpdatePosition(_currentTarget, _root));
        }).StartingIn(_tooltip.showDelay);
    }

    private void Hide()
    {
        _showSchedule?.Pause();
        _showSchedule = null;

        _currentTarget = null;

        if (_tooltip != null)
            _tooltip.style.display = DisplayStyle.None;
    }
}
