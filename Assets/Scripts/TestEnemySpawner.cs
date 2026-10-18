using UnityEngine;

// Temporary enemy spawning for testing. FUTURE: remove this and replace with a proper spawner wave manager that uses waves and timers
// Unity needs MonoBehaviour to run Start() and show Inspector fields.
public class TestEnemySpawner : MonoBehaviour
{
    public GameObject basicEnemyPrefab;
    public GameObject strongEnemyPrefab;
    public Transform leftRoute;
    public Transform rightRoute;
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnEnemy(basicEnemyPrefab, leftRoute);
            SpawnEnemy(strongEnemyPrefab, rightRoute);
        }
    }

    // Copies the waypoint objects into a simple array.
    public void SpawnEnemy(GameObject prefab, Transform route)
    {
        if (prefab == null || route == null || route.childCount < 2)
        {
            Debug.LogError("Missing enemy prefab or route");
            return;
        }

        Transform[] points = new Transform[route.childCount];
        for (int i = 0; i < route.childCount; i++)
            points[i] = route.GetChild(i);

        GameObject enemy = Instantiate(prefab);
        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();

        if (movement == null)
        {
            Debug.LogError("Enemy prefab needs EnemyMovement");
            Destroy(enemy);
            return;
        }

        movement.SetRoute(points);
    }


    // Testing functions for spawning enemies (remove later)
    [ContextMenu("TEST: Basic on left")]
    private void SpawnBasicLeft()
    {
        SpawnEnemy(basicEnemyPrefab, leftRoute);
    }

    [ContextMenu("TEST: Basic on right")]
    private void SpawnBasicRight()
    {
        SpawnEnemy(basicEnemyPrefab, rightRoute);
    }

    [ContextMenu("TEST: Strong on left")]
    private void SpawnStrongLeft()
    {
        SpawnEnemy(strongEnemyPrefab, leftRoute);
    }

    [ContextMenu("TEST: Strong on right")]
    private void SpawnStrongRight()
    {
        SpawnEnemy(strongEnemyPrefab, rightRoute);
    }
}
