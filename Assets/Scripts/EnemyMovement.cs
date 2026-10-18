using UnityEngine;

// Moves a sphere from one waypoint to the next
// Unity needs MonoBehaviour for Update() to run each frame
public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 2.5f;
    public float travelHeight = 0.5f;
    public Transform[] waypoints;

    private int nextPoint = 1;
    private bool finished = false;

    // The spawner can give an enemy either route
    public void SetRoute(Transform[] newRoute)
    {
        waypoints = newRoute;
        nextPoint = 1;
        finished = false;

        if (waypoints == null || waypoints.Length < 2)
        {
            Debug.LogError("Enemy needs at least 2 waypoints");
            enabled = false;
            return;
        }

        Vector3 start = waypoints[0].position;
        transform.position = new Vector3(start.x, travelHeight, start.z);
    }

    private void Start()
    {
        if (waypoints != null && waypoints.Length >= 2)
            SetRoute(waypoints);
    }

    private void Update()
    {
        if (finished || waypoints == null || waypoints.Length < 2)
            return;

        Vector3 target = waypoints[nextPoint].position;
        target.y = travelHeight;

        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(direction);

        transform.position = Vector3.MoveTowards(
            transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.02f)
        {
            nextPoint++;
            if (nextPoint >= waypoints.Length)
            {
                finished = true;

                // Remove the enemy and apply the breach penalty.
                GetComponent<EnemyHealth>().ReachFortress();
                Debug.Log(name + " reached the fortress");
            }
        }
    }
}
