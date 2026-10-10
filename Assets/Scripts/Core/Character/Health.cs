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
    [SerializeField] int baseHealth = 50;
    [SerializeField] StatsConfig statsConfig;
    [Header("Animation")]
    [SerializeField] string propertyName = "_EmissionColor";
    [ColorUsage(false, true)]
    [SerializeField] Color originalColor;
    [ColorUsage(false, true)]
    [SerializeField] Color damageColor;

    public int Current => _currentHealth;
    public int Max => _maxHealth;

    int _currentHealth;
    int _maxHealth;
    BaseCharacter _character;
    List<Material> _materialInstances;


    /// <summary>
    /// Subtracts amount from the character health. If it reaches zero, invokes Die().
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        if (_character is Player)
        {
            GameEvents.RaiseHealthChanged(_currentHealth, _maxHealth);
        }

        if (_currentHealth <= 0)
        {
            _character.Die();
        }

        StartCoroutine(AnimateMaterialRoutine());
    }

    /// <summary>
    /// Sets the
    /// </summary>
    /// <param name="maxHealth"></param>
    public void SetMaxHealth(int maxHealth)
    {
        if (maxHealth <= _maxHealth)
        {
            Debug.LogWarning($"Tried invoking method <b><color=white>{nameof(Heal)}()</color></b> of <b>{name}</b> with value = {maxHealth} which is lower than current MaxHealth = {_maxHealth}.");
            return;
        }

        int difference = maxHealth - _maxHealth;
        _maxHealth = maxHealth;

        Heal(difference);
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
        var stats = statsConfig.characterStatsConfig;

        if (stats != null)
        {
            baseHealth = (int)stats.maxHealth.baseValue;
        }
    }

    void GetMaterialRefs()
    {
        _materialInstances = new();

        foreach (var renderer in _character.Renderers)
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
            if (this == null)
            {
                break;
            }
            _materialInstances.ForEach(material => material.SetColor(propertyName, i % 2 == 1 ? originalColor : damageColor)) ;
            i++;
            yield return new WaitForSeconds(.1f);
        }
    }

    /// <summary>
    /// Adds amount to character health up to MaxHealth
    /// </summary>
    /// <param name="amount"></param>
    void Heal(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"Tried invoking method <b><color=white>{nameof(Heal)}()</color></b> of <b>{name}</b> with a non-positive value.");
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
    }
}
