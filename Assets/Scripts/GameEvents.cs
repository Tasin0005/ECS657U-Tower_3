using System;
using UnityEngine;

// Events let other members react to kills and breaches.
// Static is used because the events are shared across the whole game.
public static class GameEvents
{
    public static event Action<int> EnemyKilled;
    public static event Action<int> EnemyBreached;

    public static void RaiseEnemyKilled(int points)
    {
        Debug.Log("EnemyKilled: +" + points + " skill points");
        if (EnemyKilled != null)
            EnemyKilled(points);
    }

    public static void RaiseEnemyBreached(int points)
    {
        Debug.Log("EnemyBreached: -" + points + " skill points");
        if (EnemyBreached != null)
            EnemyBreached(points);
    }
}
