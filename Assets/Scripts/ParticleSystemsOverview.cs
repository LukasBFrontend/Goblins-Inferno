using UnityEngine;

public class ParticleSystemControls : MonoBehaviour
{
    [SerializeField] bool looping;
    [SerializeField] float effectiveDuration = 1;
    [Header("Readonly")]
    [SerializeField] float simulationSpeed = 1;

    ParticleSystem[] _particleSystems;

    public bool Looping => looping;
    public float SimulationSpeed => simulationSpeed;
    public float EffectiveDuration => effectiveDuration;

    void Awake()
    {
        _particleSystems = Utils.CombineReferences(
            true,
            GetComponentsInChildren<ParticleSystem>(),
            GetComponent<ParticleSystem>()
        );
    }

    void OnValidate()
    {
        Initialize();
    }

    void Initialize()
    {
        _particleSystems = Utils.CombineReferences(
            true,
            GetComponentsInChildren<ParticleSystem>(),
            GetComponent<ParticleSystem>()
        );

        SetEffectiveDuration(effectiveDuration);

        foreach (var particleSystem in _particleSystems)
        {
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = particleSystem.main;
            main.loop = looping;
            main.simulationSpeed = simulationSpeed;
        }
    }

    void SetEffectiveDuration(float targetDuration)
    {
        SetSimulationSpeed(targetDuration != 0 ? GetHighestDuration() / targetDuration : 0);
    }

    float GetHighestDuration()
    {
        float longest = 0f;

        foreach (var particleSystem in _particleSystems)
        {
            longest = Mathf.Max(longest, particleSystem.main.duration);
        }

        return longest;
    }

    void SetLooping(bool looping)
    {
        this.looping = looping;

        foreach (var particleSystem in _particleSystems)
        {
            var main = particleSystem.main;
            main.loop = looping;
        }
    }

    void SetSimulationSpeed(float factor)
    {
        simulationSpeed = factor;

        foreach (var particleSystem in _particleSystems)
        {
            var main = particleSystem.main;
            main.simulationSpeed = factor;
        }
    }

}
