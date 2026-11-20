using System;
using UnityEngine;

// static used because the events are shared across the whole game.
public static class GameEvents
{
    public static event Action<int> EnemyKilled;
    public static event Action<int> EnemyBreached;

    public static void RaiseEnemyKilled(int points)
    {
#if UNITY_EDITOR
        Debug.Log("EnemyKilled: +" + points + " skill points");
#endif
        EnemyKilled?.Invoke(points);
    }

    public static void RaiseEnemyBreached(int points)
    {
#if UNITY_EDITOR
        Debug.Log("EnemyBreached: -" + points + " skill points");
#endif
        EnemyBreached?.Invoke(points);
    }
}