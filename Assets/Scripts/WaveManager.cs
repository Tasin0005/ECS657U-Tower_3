using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Member 4 - spawns waves on BOTH routes and tracks when a wave is finished.
// Put this on an empty GameObject called "WaveManager" in Prototype.unity.
// Replaces TestEnemySpawner (turn that off or untick spawnOnStart).
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Enemy prefabs (Member 1)")]
    public GameObject basicEnemyPrefab;   // red sphere
    public GameObject strongEnemyPrefab;  // orange sphere

    [Header("Routes (Member 1) - drag in the LeftRoute / RightRoute parents")]
    public Transform leftRoute;           // children = waypoints, in order
    public Transform rightRoute;

    [Header("Tuning")]
    public float spawnInterval = 2f;

    // Wave compositions from the plan: { red, orange }
    // After the last entry, the last wave repeats until the game ends.
    private readonly int[][] waves =
    {
        new[] { 5, 0 },   // Wave 1
        new[] { 4, 2 },   // Wave 2
        new[] { 5, 3 },   // Wave 3 (repeats)
    };

    public int CurrentWave { get; private set; }        // 0 = no wave started yet
    public bool WaveInProgress { get; private set; }

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();
    private Transform[] leftPoints, rightPoints;
    private bool spawning;
    private bool stopped;

    private void Awake()
    {
        Instance = this;
        leftPoints = GetWaypoints(leftRoute);
        rightPoints = GetWaypoints(rightRoute);
    }

    // Called by the Start Wave button (HUDManager wires it up).
    public void StartNextWave()
    {
        if (WaveInProgress || stopped)
            return;

        CurrentWave++;
        int[] comp = waves[Mathf.Min(CurrentWave - 1, waves.Length - 1)];
        StartCoroutine(SpawnWave(comp[0], comp[1]));
    }

    // Called by GameOverUI on victory/defeat so nothing else spawns.
    public void StopWaves()
    {
        stopped = true;
        StopAllCoroutines();
        spawning = false;
    }

    private IEnumerator SpawnWave(int redCount, int orangeCount)
    {
        WaveInProgress = true;
        spawning = true;

        List<bool> order = BuildOrder(redCount, orangeCount); // true = orange

        for (int i = 0; i < order.Count; i++)
        {
            bool useLeft = i % 2 == 0; // alternate so BOTH routes are used
            SpawnEnemy(order[i] ? strongEnemyPrefab : basicEnemyPrefab, useLeft ? leftPoints : rightPoints);
            yield return new WaitForSeconds(spawnInterval);
        }

        spawning = false;
    }

    private void SpawnEnemy(GameObject prefab, Transform[] route)
    {
        if (prefab == null || route.Length < 2)
        {
            Debug.LogError("WaveManager: missing enemy prefab or route");
            return;
        }

        GameObject enemy = Instantiate(prefab);
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError("Enemy prefab needs EnemyMovement: " + prefab.name);
            Destroy(enemy);
            return;
        }

        movement.SetRoute(route); // also places the enemy on the first waypoint
        aliveEnemies.Add(enemy);
    }

    private void Update()
    {
        if (!WaveInProgress || spawning)
            return;

        // Killed or breached enemies are destroyed, which makes them null in Unity.
        aliveEnemies.RemoveAll(e => e == null);
        if (aliveEnemies.Count == 0)
            WaveInProgress = false;
    }

    // Spreads orange enemies between the reds instead of all at the end.
    private static List<bool> BuildOrder(int red, int orange)
    {
        List<bool> order = new List<bool>();
        while (red > 0 || orange > 0)
        {
            if (red > 0) { order.Add(false); red--; }
            if (orange > 0) { order.Add(true); orange--; }
        }
        return order;
    }

    private static Transform[] GetWaypoints(Transform route)
    {
        if (route == null)
        {
            Debug.LogError("WaveManager: a route is not assigned");
            return new Transform[0];
        }

        Transform[] points = new Transform[route.childCount];
        for (int i = 0; i < route.childCount; i++)
            points[i] = route.GetChild(i);
        return points;
    }
}
