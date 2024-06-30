using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaterStats : MonoBehaviour
{
    [Header("Major Stats")]
    public Stat strength; // 1 point increase damage by 1 and crit.power by 1%
    public Stat agility; // 1 point increase evasion by 1% and crit.chance by 1 %
    public Stat intelgence; // 1 point increase magic by 1 and magic resistance by 3
    public Stat vitality; // 1 point increase health by 3 or 5 points

    [Header("Defensive Stats")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;


    public Stat damage;

    [SerializeField] private int currentHealth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        currentHealth = maxHealth.GetValue();
    }

    public virtual void DoDamage(CharaterStats _targetStats)
    {
        int totalDamage = damage.GetValue() + strength.GetValue();

        _targetStats.TakeDamage(totalDamage);
    }

    public virtual void TakeDamage(int _damage)
    {
        currentHealth -= _damage;

        Debug.Log(_damage);
        if (currentHealth < 0)
            Die();

    }

    protected virtual void Die()
    {
        //throw new NotImplementedException();
    }
}
