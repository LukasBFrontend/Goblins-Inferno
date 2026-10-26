using System.Collections;
using System.Threading.Tasks;
using Luscious;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Volume))]
public class VolumeTransitionHandler : Singleton<VolumeTransitionHandler>
{
    [Space()]
    [SerializeField] VolumeSnapshot volumeSnapshot;
    [Range(30, 120)]
    [SerializeField] int transitionFPS;
    [Header("Transitions")]
    [SerializeField] Vector4Transition gammaFadeIn;
    [SerializeField] Vector4Transition gammaFadeOut;
    [SerializeField] ColorTransition colorFadeOut;

    LiftGammaGain _liftGammaGain;
    ColorAdjustments _colorAdjustments;
    Volume _volume;
    VolumeProfile _profile;

    /* PUBLIC METHODS */

    public Task<bool> GammaFadeIn()
    {
        return ColorTransitionTask(_liftGammaGain.gamma, gammaFadeIn);
    }

    public Task<bool> GammaFadeOut()
    {
        return ColorTransitionTask(_liftGammaGain.gamma, gammaFadeOut);
    }

    public Task<bool> ColorFadeOut()
    {
        return ColorTransitionTask(_colorAdjustments.colorFilter, colorFadeOut);
    }

    void ResetColorFilter()
    {
        if (!volumeSnapshot.Default.TryGet(out ColorAdjustments colorAdjustments))
        {
            return;
        }

        _colorAdjustments.colorFilter.value = colorAdjustments.colorFilter.value;
    }

    /* MONOBEHAVIOR LIFECYCLE METHODS */

    protected override void OnSingletonAwake()
    {
        _volume = GetComponent<Volume>();
        _profile = _volume.profile;

        GetProfileComponents();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        GetProfileComponents();
        ResetColorFilter();
    }

    void OnValidate()
    {
        if (_volume == null)
        {
            _volume = GetComponent<Volume>();
        }

        _profile = _volume.profile;
    }

    /* PRIVATE METHODS */

    Task<bool> ColorTransitionTask<T>(T volumeParameter, IColorTransitionData colorTransition) where T : VolumeParameter
    {
        GetProfileComponents();
        var promise = new TaskCompletionSource<bool>();
        StartCoroutine(ColorFadeRoutine(volumeParameter, colorTransition, promise));

        return promise.Task;
    }

    IEnumerator ColorFadeRoutine<T>(T volumeParameter, IColorTransitionData colorTransition, TaskCompletionSource<bool> promise) where T: VolumeParameter
    {
        bool logTransition = colorTransition.LogTransition;
        Color start = colorTransition.StartValue;
        Color end = colorTransition.TargetValue;
        float duration = colorTransition.Duration;
        float elapsed = 0f;

        _colorAdjustments.colorFilter.overrideState = true;

        if (logTransition)
        {
            Debug.Log($"{nameof(ColorFadeRoutine)} <color=blue>started</color> at: <color=white>{Time.time}</color>");
        }

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            int totalframes = Mathf.RoundToInt(transitionFPS * duration);
            t = Mathf.Round(t * totalframes) / totalframes;

            if (volumeParameter is ColorParameter colorParam)
            {
                colorParam.value = Color.Lerp(start, end, t);
            }
            else if (volumeParameter is Vector4Parameter vectorParam)
            {
                vectorParam.value = Color.Lerp(start, end, t);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        promise.TrySetResult(true);

        if (logTransition)
        {
            Debug.Log($"{nameof(ColorFadeRoutine)} <color=orange>ended</color> at: <color=white>{Time.time}</color>");
        }
    }

    void GetProfileComponents()
    {
        if (_liftGammaGain == null)
        {
            if (!_profile.TryGet<LiftGammaGain>(out var component))
            {
                Debug.Log($"Tried getting {nameof(LiftGammaGain)} without success");
            }
            else
            {
                _liftGammaGain = component;
            }
        }

        if (_colorAdjustments == null)
        {
            if (!_profile.TryGet<ColorAdjustments>(out var component))
            {
                Debug.Log($"Tried getting {nameof(ColorAdjustments)} without success");
            }
            else
            {
                _colorAdjustments = component;
            }
        }
    }

    /* TYPES */

    [System.Serializable]
    public struct Vector4Transition : IColorTransitionData
    {
        public Vector4 startValue;
        public Vector4 targetValue;
        [Space()]
        public float duration;
        public bool logTransition;

        public readonly Color StartValue => startValue;
        public readonly Color TargetValue => targetValue;
        public readonly float Duration => duration;
        public readonly bool LogTransition => logTransition;
    }

    [System.Serializable]
    public struct ColorTransition : IColorTransitionData
    {
        [ColorUsage(false, true)]
        public Color startValue;
        [ColorUsage(false, true)]
        public Color targetValue;
        [Space()]
        public float duration;
        public bool logTransition;

        public readonly Color StartValue => startValue;
        public readonly Color TargetValue => targetValue;
        public readonly float Duration => duration;
        public readonly bool LogTransition => logTransition;
    }

    interface IColorTransitionData
    {
        public Color StartValue { get; }
        public Color TargetValue { get; }
        public float Duration { get; }
        public bool LogTransition { get; }
    }
}
