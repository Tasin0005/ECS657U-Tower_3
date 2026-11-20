using System.Collections.Generic;
using UnityEngine;

// Automatically attacks the nearest enemy in range and draws a shared material beam.
public class WizardAttack : MonoBehaviour
{
    public WizardData data;
    public float physicalMagicBonusDps = 2f;

    private static Material sharedBeamMaterial;

    private LineRenderer beam;
    private float currentDamagePerSecond;
    private float rangeSqr;

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError("WizardAttack: missing WizardData on " + name);
            enabled = false;
            return;
        }

        rangeSqr = data.range * data.range;
        currentDamagePerSecond = data.damagePerSecond;

        if (sharedBeamMaterial == null)
            sharedBeamMaterial = new Material(Shader.Find("Sprites/Default"));

        beam = gameObject.AddComponent<LineRenderer>();
        beam.positionCount = 2;
        beam.startWidth = 0.1f;
        beam.endWidth = 0.1f;
        beam.sharedMaterial = sharedBeamMaterial;
        beam.startColor = data.colour;
        beam.endColor = data.colour;
        beam.enabled = false;
    }

    private void Start()
    {
        if (data.type == WizardType.Basic && SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked += HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked += HandleSkillChanged;
            RefreshDamage();
        }
    }

    private void OnDestroy()
    {
        if (data != null && data.type == WizardType.Basic && SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked -= HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked -= HandleSkillChanged;
        }
    }

    private void HandleSkillChanged(SkillId skill)
    {
        if (skill == SkillId.PhysicalMagic)
            RefreshDamage();
    }

    private void RefreshDamage()
    {
        bool boosted = SkillPoints.Instance != null && SkillPoints.Instance.IsUnlocked(SkillId.PhysicalMagic);
        currentDamagePerSecond = boosted
            ? data.damagePerSecond + physicalMagicBonusDps
            : data.damagePerSecond;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        EnemyHealth target = FindClosestEnemyInRange();

        if (target == null)
        {
            beam.enabled = false;
            return;
        }

        target.TakeDamage(currentDamagePerSecond * Time.deltaTime);

        if (target == null || target.IsResolved)
        {
            beam.enabled = false;
            return;
        }

        beam.enabled = true;
        beam.SetPosition(0, transform.position + Vector3.up * 1.2f);
        beam.SetPosition(1, target.transform.position);
    }

    private EnemyHealth FindClosestEnemyInRange()
    {
        EnemyHealth closest = null;
        float bestSqr = rangeSqr;
        Vector3 myPos = transform.position;

        List<EnemyHealth> enemies = EnemyHealth.ActiveEnemies;
        for (int i = 0; i < enemies.Count; i++)
        {
            EnemyHealth enemy = enemies[i];
            if (enemy == null || enemy.IsResolved)
                continue;

            float distSqr = (enemy.transform.position - myPos).sqrMagnitude;
            if (distSqr <= bestSqr)
            {
                bestSqr = distSqr;
                closest = enemy;
            }
        }

        return closest;
    }
}