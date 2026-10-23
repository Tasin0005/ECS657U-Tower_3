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

    public enum SpawnSide { Both, Left, Right, Random }

    [System.Serializable]
    public struct Wave
    {
        public int red;          // basic enemies
        public int orange;       // strong enemies
        public SpawnSide side;   // which path(s) this wave uses
        public Wave(int red, int orange, SpawnSide side = SpawnSide.Both)
        {
            this.red = red; this.orange = orange; this.side = side;
        }
    }

    [Header("Tuning")]
    public float spawnInterval = 1.5f;

    // Wave compositions, editable in the Inspector.
    // After the last entry, the last wave repeats until the game ends.
    public Wave[] waves =
    {
        new Wave(6, 0),    // Wave 1
        new Wave(6, 2),    // Wave 2
        new Wave(8, 3),    // Wave 3
        new Wave(8, 5),    // Wave 4
        new Wave(10, 6),   // Wave 5 (repeats)
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
        if (WaveInProgress || stopped || waves.Length == 0)
            return;

        CurrentWave++;
        Wave wave = waves[Mathf.Min(CurrentWave - 1, waves.Length - 1)];
        StartCoroutine(SpawnWave(wave.red, wave.orange, wave.side));
    }

    // Called by GameOverUI on victory/defeat so nothing else spawns.
    public void StopWaves()
    {
        stopped = true;
        StopAllCoroutines();
        spawning = false;
    }

    private IEnumerator SpawnWave(int redCount, int orangeCount, SpawnSide side)
    {
        WaveInProgress = true;
        spawning = true;

        List<bool> order = BuildOrder(redCount, orangeCount); // true = orange

        int redsSpawned = 0, orangesSpawned = 0;

        for (int i = 0; i < order.Count; i++)
        {
            // Each colour alternates sides on its own, so oranges use both paths too.
            int countOfThisColour = order[i] ? orangesSpawned++ : redsSpawned++;

            bool useLeft;
            if (side == SpawnSide.Left) useLeft = true;
            else if (side == SpawnSide.Right) useLeft = false;
            else if (side == SpawnSide.Random) useLeft = UnityEngine.Random.value < 0.5f;
            else useLeft = countOfThisColour % 2 == 0; // Both: alternate left and right
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
