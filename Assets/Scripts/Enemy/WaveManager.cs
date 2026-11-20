using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns waves on both routes and tracks wave completion via events instead of polling per frame.
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Enemy prefabs")]
    public GameObject basicEnemyPrefab;
    public GameObject strongEnemyPrefab;

    [Header("Routes - drag in the LeftRoute / RightRoute parents")]
    public Transform leftRoute;
    public Transform rightRoute;

    public enum SpawnSide { Both, Left, Right, Random }

    [Serializable]
    public struct Wave
    {
        public int red;
        public int orange;
        public SpawnSide side;

        public Wave(int red, int orange, SpawnSide side = SpawnSide.Both)
        {
            this.red = red;
            this.orange = orange;
            this.side = side;
        }
    }

    [Header("Tuning")]
    public float spawnInterval = 1.5f;

    public Wave[] waves =
    {
        new Wave(6, 0),
        new Wave(6, 2),
        new Wave(8, 3),
        new Wave(8, 5),
        new Wave(10, 6),
    };

    public int CurrentWave { get; private set; }
    public bool WaveInProgress { get; private set; }

    public event Action OnWaveStateChanged;

    private Transform[] leftPoints, rightPoints;
    private int aliveEnemyCount;
    private bool spawning;
    private bool stopped;

    private void Awake()
    {
        Instance = this;
        leftPoints = GetWaypoints(leftRoute);
        rightPoints = GetWaypoints(rightRoute);
    }

    private void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyResolved;
        GameEvents.EnemyBreached += HandleEnemyResolved;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyResolved;
        GameEvents.EnemyBreached -= HandleEnemyResolved;
    }

    public void StartNextWave()
    {
        if (WaveInProgress || stopped || waves.Length == 0)
            return;

        CurrentWave++;
        Wave wave = waves[Mathf.Min(CurrentWave - 1, waves.Length - 1)];
        StartCoroutine(SpawnWave(wave.red, wave.orange, wave.side));
    }

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
        OnWaveStateChanged?.Invoke();

        List<bool> order = BuildOrder(redCount, orangeCount);
        WaitForSeconds wait = new WaitForSeconds(spawnInterval);

        int redsSpawned = 0, orangesSpawned = 0;

        for (int i = 0; i < order.Count; i++)
        {
            int countOfThisColour = order[i] ? orangesSpawned++ : redsSpawned++;

            bool useLeft;
            if (side == SpawnSide.Left) useLeft = true;
            else if (side == SpawnSide.Right) useLeft = false;
            else if (side == SpawnSide.Random) useLeft = UnityEngine.Random.value < 0.5f;
            else useLeft = countOfThisColour % 2 == 0;

            SpawnEnemy(order[i] ? strongEnemyPrefab : basicEnemyPrefab, useLeft ? leftPoints : rightPoints);
            yield return wait;
        }

        spawning = false;
        CheckWaveComplete();
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

        movement.SetRoute(route);
        aliveEnemyCount++;
    }

    private void HandleEnemyResolved(int _)
    {
        if (aliveEnemyCount > 0)
            aliveEnemyCount--;

        CheckWaveComplete();
    }

    private void CheckWaveComplete()
    {
        if (WaveInProgress && !spawning && aliveEnemyCount <= 0)
        {
            WaveInProgress = false;
            OnWaveStateChanged?.Invoke();
        }
    }

    private static List<bool> BuildOrder(int red, int orange)
    {
        List<bool> order = new List<bool>(red + orange);
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