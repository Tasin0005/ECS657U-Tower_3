using UnityEngine;

// Stores health and tells the game when an enemy dies or escapes
// Unity needs MonoBehaviour to attach this script to an enemy sphere
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 10f;
    public float CurrentHealth;
    public int killReward = 3;
    public int breachPenalty = 4;
    public bool IsResolved = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        IsResolved = false;
    }

    // FUTURE: Call this when a wizard hits an enemy
    public void TakeDamage(float damage)
    {
        if (IsResolved || damage <= 0)
            return;

        CurrentHealth = CurrentHealth - damage;
        if (CurrentHealth > 0)
            return;

        CurrentHealth = 0;
        IsResolved = true; // Stops duplicate rewards.
        GameEvents.RaiseEnemyKilled(killReward);
        Destroy(gameObject);
    }

    // Call when enemy reaches fortress
    public void ReachFortress()
    {
        if (IsResolved)
            return;

        IsResolved = true;
        GameEvents.RaiseEnemyBreached(breachPenalty);
        Destroy(gameObject);
    }

    // Testing functions for enemies (remove later)
    [ContextMenu("TEST: Deal 5 damage")]
    private void TestDamage()
    {
        TakeDamage(5);
    }

    [ContextMenu("TEST: Kill enemy")]
    private void TestKill()
    {
        TakeDamage(99999);
    }
}
