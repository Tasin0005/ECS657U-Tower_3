using UnityEngine;

// Moves an active enemy from waypoint to waypoint until it reaches the fortress
[RequireComponent(typeof(EnemyHealth))]
public class EnemyMovement : MonoBehaviour
{
    public EnemyData data;
    public Transform[] waypoints;

    private const float WaypointReachThresholdSqr = 0.02f * 0.02f;

    private EnemyHealth health;
    private int nextPoint = 1;
    private bool finished;
    private bool routeSet;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    public void SetRoute(Transform[] newRoute)
    {
        waypoints = newRoute;
        nextPoint = 1;
        finished = false;
        routeSet = true;

        if (data == null)
        {
            Debug.LogError("EnemyMovement: missing EnemyData on " + name);
            enabled = false;
            return;
        }

        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("Enemy needs at least 2 waypoints");
            enabled = false;
            return;
        }

        Vector3 start = waypoints[0].position;
        transform.position = new Vector3(start.x, data.travelHeight, start.z);
    }

    private void Start()
    {
        if (!routeSet && waypoints != null && waypoints.Length >= 2)
            SetRoute(waypoints);
    }

    private void Update()
    {
        if (finished || data == null || waypoints == null || waypoints.Length < 2)
            return;

        Vector3 target = waypoints[nextPoint].position;
        target.y = data.travelHeight;

        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction);

        transform.position = Vector3.MoveTowards(
            transform.position, target, data.moveSpeed * Time.deltaTime);

        if ((transform.position - target).sqrMagnitude < WaypointReachThresholdSqr)
        {
            nextPoint++;
            if (nextPoint >= waypoints.Length)
            {
                finished = true;
                if (health != null)
                    health.ReachFortress();
#if UNITY_EDITOR
                Debug.Log(name + " reached the fortress");
#endif
            }
        }
    }
}