using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[System.Obsolete("Should be refactored")]
public class VolumeTransitionHandler : Singleton<VolumeTransitionHandler>
{
    [Space()]
    [SerializeField] VolumeProfile profile;
    [Range(30, 120)]
    [SerializeField] int transitionFPS;
    [Header("Transitions")]
    [SerializeField] Vector4TransitionData gammaFadeIn;
    [SerializeField] Vector4TransitionData gammaFadeOut;
    [SerializeField] ColorTransitionData colorFadeOut;
    LiftGammaGain _liftGammaGain;
    ColorAdjustments _colorAdjustments;

    public Task<bool> GammaFadeIn()
    {
        GetProfileComponents();
        var promise = new TaskCompletionSource<bool>();
        StartCoroutine(GammaFadeRoutine(promise, gammaFadeIn));

        return promise.Task;
    }

    public Task<bool> GammaFadeOut()
    {
        GetProfileComponents();
        var promise = new TaskCompletionSource<bool>();
        StartCoroutine(GammaFadeRoutine(promise, gammaFadeOut));

        return promise.Task;
    }

    public Task<bool> ColorFadeOut()
    {
        GetProfileComponents();
        var promise = new TaskCompletionSource<bool>();
        StartCoroutine(ColorFadeRoutine(promise, colorFadeOut));

        return promise.Task;
    }

    void ResetColorFilter()
    {
        _colorAdjustments.colorFilter.value = colorFadeOut.startValue;
    }

    void Start()
    {
        GetProfileComponents();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        GetProfileComponents();
        ResetColorFilter();
    }

    void GetProfileComponents()
    {
        if (_liftGammaGain == null && profile.TryGet<LiftGammaGain>(out var component))
        {
            _liftGammaGain = component;
        }

        if (_colorAdjustments == null && profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            _colorAdjustments = colorAdjustments;
        }
    }

    IEnumerator ColorFadeRoutine(TaskCompletionSource<bool> promise, ColorTransitionData colorTransition)
    {
        if (colorTransition.logTransition)
        {
            Debug.Log($"{nameof(ColorFadeRoutine)} <color=blue>started</color> at: <color=white>{Time.time}</color>");
        }

        _colorAdjustments.colorFilter.overrideState = true;
        float elapsed = 0f;

        while (elapsed < colorTransition.duration)
        {
            float t = elapsed / colorTransition.duration;
            int totalframes = Mathf.RoundToInt(transitionFPS * colorTransition.duration);
            t = Mathf.Round(t * totalframes) / totalframes;

            Color color = _colorAdjustments.colorFilter.value = Color.Lerp(colorTransition.startValue, colorTransition.targetValue, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        promise.TrySetResult(true);

        if (colorTransition.logTransition)
        {
            Debug.Log($"{nameof(ColorFadeRoutine)} <color=orange>ended</color> at: <color=white>{Time.time}</color>");
        }
    }

    IEnumerator GammaFadeRoutine(TaskCompletionSource<bool> promise, Vector4TransitionData gammaTransition)
    {
        if (gammaTransition.logTransition)
        {
            Debug.Log($"{nameof(GammaFadeRoutine)} <color=blue>started</color> at: <color=white>{Time.time}</color>");
        }

        _liftGammaGain.gamma.overrideState = true;
        float elapsed = 0f;

        while (elapsed < gammaTransition.duration)
        {
            float t = elapsed / gammaTransition.duration;
            int totalframes = Mathf.RoundToInt(transitionFPS * gammaTransition.duration);
            t = Mathf.Round(t * totalframes) / totalframes;

            if (_liftGammaGain == null && profile.TryGet<LiftGammaGain>(out var component))
            {
                _liftGammaGain = component;
            }

            _liftGammaGain.gamma.value = Vector4.Lerp(gammaTransition.startValue, gammaTransition.targetValue, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        promise.TrySetResult(true);

        if (gammaTransition.logTransition)
        {
            Debug.Log($"{nameof(GammaFadeRoutine)} <color=orange>ended</color> at: <color=white>{Time.time}</color>");
        }
    }

    [System.Serializable]
    public struct Vector4TransitionData
    {
        public Vector4 startValue;
        public Vector4 targetValue;
        [Space()]
        public float duration;
        public bool logTransition;
    }

    [System.Serializable]
    public struct ColorTransitionData
    {
        [ColorUsage(false, true)]
        public Color startValue;
        [ColorUsage(false, true)]
        public Color targetValue;
        [Space()]
        public float duration;
        public bool logTransition;
    }
}
