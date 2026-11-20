using System.Collections.Generic;
using UnityEngine;

// Tracks an active enemys health, registers it for wizard targeting, and reports kills or breaches
public class EnemyHealth : MonoBehaviour
{
    public static readonly List<EnemyHealth> ActiveEnemies = new List<EnemyHealth>();

    public EnemyData data;

    public float CurrentHealth { get; private set; }
    public bool IsResolved { get; private set; }

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError("EnemyHealth: missing EnemyData on " + name);
            enabled = false;
            return;
        }

        CurrentHealth = data.maxHealth;
        IsResolved = false;
    }

    private void OnEnable()
    {
        ActiveEnemies.Add(this);
    }

    private void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }

    public void TakeDamage(float damage)
    {
        if (IsResolved || damage <= 0f || data == null)
            return;

        CurrentHealth -= damage;
        if (CurrentHealth > 0f)
            return;

        CurrentHealth = 0f;
        IsResolved = true;
        ActiveEnemies.Remove(this);
        GameEvents.RaiseEnemyKilled(data.killReward);
        Destroy(gameObject);
    }

    public void ReachFortress()
    {
        if (IsResolved || data == null)
            return;

        IsResolved = true;
        ActiveEnemies.Remove(this);
        GameEvents.RaiseEnemyBreached(data.breachPenalty);
        Destroy(gameObject);
    }
}