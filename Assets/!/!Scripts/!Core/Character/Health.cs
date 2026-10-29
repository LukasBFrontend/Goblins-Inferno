using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
struct StatsConfig
{
    [Header("Overrides base health if set")]
    public SO_CharacterStatsConfig characterStatsConfig;
}

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
    [Header("Damage Text")]
    [SerializeField] bool showDamageText;
    [SerializeField] PoolableData damageTextPoolableData;

    static readonly WaitForSeconds _waitForSeconds_1 = new(.1f);
    int _currentHealth;
    int _maxHealth;
    BaseCharacter _character;
    List<Material> _materialInstances = new();
    bool _isInvincibilityFrame;

    public int Current => _currentHealth;
    public int Max => _maxHealth;

    /// <summary>
    /// Subtracts amount from the character health. If it reaches zero, invokes Die().
    /// </summary>
    /// <param name="damageAmount"></param>
    public int TakeDamage(int damageAmount, int critLevel)
    {
        if (_isInvincibilityFrame)
        {
            return 0;
        }

        if (showDamageText)
        {
            DamageText damageText = Game.ObjectPooler.Spawn<DamageText>
            (
                damageTextPoolableData,
                new (transform.position.x, 1.75f, transform.position.z - .5f),
                Quaternion.identity
            );

            damageText.Initialize(damageAmount, critLevel);
        }

        _currentHealth -= damageAmount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

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

    /// <summary>
    /// Sets the
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetMaxHealth(int maxHealth)
    {
        if (maxHealth <= _maxHealth)
        {
            Debug.LogWarning($"Tried invoking method <b><color=white>{nameof(Restore)}()</color></b> of <b>{name}</b> with value = {maxHealth} which is lower than current MaxHealth = {_maxHealth}.");
            return;
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

    void Awake()
    {
        _maxHealth = statsConfig.characterStatsConfig != null
            ? (int)statsConfig.characterStatsConfig.maxHealth.baseValue
            : baseHealth
        ;

        _currentHealth = _maxHealth;
        _character = GetComponent<BaseCharacter>();
    }

    void Start()
    {
        GetMaterialRefs();
    }

    void OnValidate()
    {
        SO_CharacterStatsConfig stats = statsConfig.characterStatsConfig;

        if (stats != null)
        {
            baseHealth = (int)stats.maxHealth.baseValue;
        }
    }

    void GetMaterialRefs()
    {
        _materialInstances.Clear();

        foreach (Renderer renderer in _character.Renderers)
        {
            _materialInstances.Add(renderer.materials[0]);
        }
    }

    IEnumerator AnimateMaterialRoutine()
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

    IEnumerator InvincibilityRoutine(float delay, float seconds)
    {
        yield return new WaitForSeconds(delay);
        _isInvincibilityFrame = true;
        yield return new WaitForSeconds(seconds);
        _isInvincibilityFrame = false;
    }

    /// <summary>
    /// Adds amount to character health up to MaxHealth
    /// </summary>
    /// <param name="amount"></param>
    public void Restore(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"Tried invoking method <b><color=white>{nameof(Restore)}()</color></b> of <b>{name}</b> with a non-positive value.");
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
        GameEvents.RaiseHealthChanged(_currentHealth, _maxHealth);
    }
}
