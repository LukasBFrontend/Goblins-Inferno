using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Luscious.Audio;
using System;


[RequireComponent(typeof (BaseCharacter))]
public class Health : MonoBehaviour
{
    const string EMISSION_PROPERTY_NAME = "_EmissionColor";

    [SerializeField] StatsConfig statsConfig;
    [SerializeField] int baseHealth = 50;
    [Header("Animation")]
    [ColorUsage(false, true)]
    [SerializeField] Color originalColor;
    [ColorUsage(false, true)]
    [SerializeField] Color damageColor;
    [Space()]
    [SerializeField] SoundEffectData damagedSound;
    [SerializeField] SoundEffectData healedSound;
    [Header("Damage Text")]
    [SerializeField] PoolableData damageTextPoolableData;

    private static readonly WaitForSeconds _waitForSeconds_1 = new(.1f);
    private int _currentHealth;
    private int _maxHealth;
    private BaseCharacter _character;
    private List<Material> _materialInstances = new();
    private bool _isInvincibilityFrame;

    public int Current => _currentHealth;
    public int Max => _maxHealth;

    public int TakeDamage(int damageAmount, int critLevel)
    {
        if (_isInvincibilityFrame)
        {
            return 0;
        }

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        if (damagedSound.audioClip != null)
        {
            Game.SoundFX.PlaySoundFXClip(damagedSound, transform.position);
        }

        if (_character is Enemy)
        {
            SpawnText(damageAmount, critLevel);
        }

        if (_character is Player)
        {
            GameEvents.RaiseHealthChanged(_currentHealth, _maxHealth);
        }

        if (_currentHealth == 0)
        {
            _character.Die();
            return 0;
        }
        else if (gameObject.activeSelf)
        {
            StartCoroutine(AnimateMaterialRoutine());

        }

        return _currentHealth;
    }

    public void Restore(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(Restore));
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);

        if (healedSound.audioClip != null)
        {
            Game.SoundFX.PlaySoundFXClip(healedSound, transform.position);
        }

        if (_character is Player)
        {
            GameEvents.RaiseHealthChanged(_currentHealth, _maxHealth);
        }

        SpawnText(-amount);
    }

    public void SetMaxHealth(int maxHealth)
    {
        if (maxHealth <= _maxHealth)
        {
            throw new ArgumentOutOfRangeException(nameof(SetMaxHealth));
        }

        int difference = maxHealth - _maxHealth;
        _maxHealth = maxHealth;

        Restore(difference);

        if (_character is Player)
        {
            GameEvents.RaiseHealthChanged(_currentHealth, _maxHealth);
        }
    }

    public void Reset()
    {
        _currentHealth = _maxHealth;
        _materialInstances.ForEach(material => material.SetColor(EMISSION_PROPERTY_NAME, originalColor));
    }

    public void SetInvincibilityFrames(float delay, float seconds)
    {
        StartCoroutine(InvincibilityRoutine(delay, seconds));
    }

    private void Awake()
    {
        _maxHealth = statsConfig.characterStatsConfig != null
            ? (int)statsConfig.characterStatsConfig.maxHealth.baseValue
            : baseHealth
        ;

        _currentHealth = _maxHealth;
        _character = GetComponent<BaseCharacter>();
    }

    private void Start()
    {
        GetMaterialRefs();
    }

    private void OnValidate()
    {
        SO_CharacterStatsConfig stats = statsConfig.characterStatsConfig;

        if (stats != null)
        {
            baseHealth = (int)stats.maxHealth.baseValue;
        }
    }

    private void GetMaterialRefs()
    {
        _materialInstances.Clear();

        foreach (Renderer renderer in _character.Renderers)
        {
            _materialInstances.Add(renderer.materials[0]);
        }
    }

    private void SpawnText(int damageAmount, int critLevel = 0)
    {
            DamageText damageText = Game.ObjectPooler.Spawn<DamageText>
            (
                damageTextPoolableData,
                new (transform.position.x, 1.75f, transform.position.z - .5f),
                Quaternion.identity
            );

            damageText.Initialize(damageAmount, critLevel);
    }

    private IEnumerator AnimateMaterialRoutine()
    {
        int i = 0;
        int maxIterations = 2;

        while (i < maxIterations)
        {
            if (!gameObject.activeSelf)
            {
                _materialInstances.ForEach(material => material.SetColor(EMISSION_PROPERTY_NAME, originalColor));
                yield break;
            }
            _materialInstances.ForEach(material => material.SetColor(EMISSION_PROPERTY_NAME, i % 2 == 1 ? originalColor : damageColor)) ;
            i++;
            yield return _waitForSeconds_1;
        }
    }

    private IEnumerator InvincibilityRoutine(float delay, float seconds)
    {
        yield return new WaitForSeconds(delay);
        _isInvincibilityFrame = true;
        yield return new WaitForSeconds(seconds);
        _isInvincibilityFrame = false;
    }

    [System.Serializable]
    struct StatsConfig
    {
        [Header("Overrides base health if set")]
        public SO_CharacterStatsConfig characterStatsConfig;
    }
}
