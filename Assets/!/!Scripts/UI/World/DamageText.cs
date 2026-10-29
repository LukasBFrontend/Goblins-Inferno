using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageText : MonoBehaviour, IPoolable
{
    [SerializeField] Color baseColor = Color.white;
    [SerializeField] List<Color> damageColorsByCritLevel;
    [SerializeField] string labelId = "DamageTextLabel";
    [Header("USS-transition classes")]
    [SerializeField] string textGrowClass = "textGrow";
    [SerializeField] string textFadeClass = "textFade";

    Label _textLabel;
    PanelRenderer _panelRenderer;

    public string PoolableId { get; set; }

    public void Initialize(int damage, int critLevel)
    {
        _textLabel.text = damage.ToString();
        _textLabel.style.color = critLevel > 0
            ? damageColorsByCritLevel[(critLevel - 1) % damageColorsByCritLevel.Count]
            : baseColor
        ;

        StartCoroutine(TextAnimationRoutine());
    }

    public void Reset()
    {
        _textLabel.RemoveFromClassList(textGrowClass);
        _textLabel.RemoveFromClassList(textFadeClass);
        _textLabel.text = "[damage]";
        _textLabel.style.color = baseColor;
    }


    void Awake()
    {
        _panelRenderer = GetComponent<PanelRenderer>();
        _panelRenderer.RegisterUIReloadCallback(OnUIReload);
    }

    void OnDestroy()
    {
        _panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    void OnUIReload(PanelRenderer renderer, VisualElement rootElement)
    {
        _textLabel = rootElement.Q<Label>(labelId);
    }


    IEnumerator TextAnimationRoutine()
    {
        bool growFinished = false, fadeFinished = false;

        yield return null;

        _textLabel.RegisterCallback<TransitionEndEvent>(OnGrowTransitionEnd);
        _textLabel.AddToClassList(textGrowClass);

        while (!growFinished)
        {
            yield return null;
        }

        _textLabel.RemoveFromClassList(textGrowClass);
        _textLabel.AddToClassList(textFadeClass);

        _textLabel.UnregisterCallback<TransitionEndEvent>(OnGrowTransitionEnd);
        _textLabel.RegisterCallback<TransitionEndEvent>(OnFadeTransitionEnd);

        while (!fadeFinished)
        {
            yield return null;
        }

        _textLabel.RemoveFromClassList(textFadeClass);
        _textLabel.UnregisterCallback<TransitionEndEvent>(OnFadeTransitionEnd);

        Game.ObjectPooler.Return(this);

        void OnGrowTransitionEnd(TransitionEndEvent evt)
        {
            if (evt.target != _textLabel)
            {
                return;
            }

            growFinished = true;
        }

        void OnFadeTransitionEnd(TransitionEndEvent evt)
        {
            if (evt.target != _textLabel)
            {
                return;
            }

            fadeFinished = true;
        }
    }
}
