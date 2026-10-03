using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof (BaseCharacter))]
public class Health : MonoBehaviour
{
    [SerializeField] Color originalColor;
    [SerializeField] Color damageColor;
    [Range(0, 100)]
    [SerializeField] int health;
    public int Current => health;
    public int Max => _maxHealth;
    int _maxHealth;
    BaseCharacter _character;
    List<Material> _materialInstances;

    void Awake()
    {
        _maxHealth = health;
        _character = GetComponent<BaseCharacter>();
        _materialInstances = new();

        foreach (var renderer in _character.Renderers)
        {
            _materialInstances.Add(renderer.materials[0]);
        }
    }

    /// <summary>
    /// Subtracts amount from the character health. If it reaches zero, invokes Die().
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, _maxHealth);

        if (_character is Player)
        {
            GameEvents.RaiseHealthChanged(health, _maxHealth);
        }

        if (health <= 0)
        {
            _character.Die();
        }

        StartCoroutine(AnimateMaterialRoutine());
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
            _materialInstances.ForEach(material => material.SetColor("_BaseColor", i % 2 == 1 ? originalColor : damageColor)) ;
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

        health = Mathf.Clamp(health + amount, 0, _maxHealth);
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
}
