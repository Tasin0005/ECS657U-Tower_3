using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attacks the nearest enemy in range automatically (no manual shooting, per the plan).
public class WizardAttack : MonoBehaviour
{
    public WizardData data;

    private LineRenderer beam;

    void Awake()
    {
        beam = gameObject.AddComponent<LineRenderer>();
        beam.positionCount = 2;
        beam.startWidth = 0.1f;
        beam.endWidth = 0.1f;
        beam.material = new Material(Shader.Find("Sprites/Default"));
        beam.startColor = data.colour;
        beam.endColor = data.colour;
        beam.enabled = false;
    }

    void Update()
    {
        EnemyHealth target = FindClosestEnemyInRange();

        // No enemy in range means no attack and no beam
        if (target == null)
        {
            beam.enabled = false;
            return;
        }

        // Damage is per second, so scale by frame time to be framerate independent
        target.TakeDamage(data.damagePerSecond * Time.deltaTime);

        // The target may have just died and been destroyed this frame
        if (target == null)
        {
            beam.enabled = false;
            return;
        }

        beam.enabled = true;
        beam.SetPosition(0, transform.position + Vector3.up * 1.2f);
        beam.SetPosition(1, target.transform.position);
    }

    // Searches EnemyHealth directly because enemies have no tag
    EnemyHealth FindClosestEnemyInRange()
    {
        EnemyHealth closest = null;
        float best = data.range;

        foreach (EnemyHealth enemy in FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy.IsResolved) continue;

            float d = Vector3.Distance(transform.position, enemy.transform.position);
            if (d <= best)
            {
                best = d;
                closest = enemy;
            }
        }
        return closest;
    }
}
