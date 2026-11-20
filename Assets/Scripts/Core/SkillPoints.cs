using System;
using System.Collections.Generic;
using UnityEngine;

// four skills in the prototype tree.
public enum SkillId
{
    BasicMagic,
    PhysicalMagic,
    MagicTraining,
    Ultimate
}

// Manages skill points (the sole currency), skill unlocking/relocking, and win/loss triggers.
public class SkillPoints : MonoBehaviour
{
    public static SkillPoints Instance { get; private set; }

    public int CurrentPoints = 6;

    private readonly HashSet<SkillId> unlockedSkills = new HashSet<SkillId>();
    private readonly Stack<SkillId> unlockOrder = new Stack<SkillId>();

    public Action OnPointsChanged;
    public Action<SkillId> OnSkillUnlocked;
    public Action<SkillId> OnSkillRelocked;
    public Action OnVictory;
    public Action OnDefeat;

    private void Awake()
    {
        Instance = this;
        unlockedSkills.Add(SkillId.BasicMagic);
    }

    private void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyKilled;
        GameEvents.EnemyBreached += HandleEnemyBreached;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyKilled;
        GameEvents.EnemyBreached -= HandleEnemyBreached;
    }

    private void HandleEnemyKilled(int points)
    {
        AddPoints(points);
    }

    private void HandleEnemyBreached(int points)
    {
        LosePoints(points);
    }

    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        CurrentPoints += amount;
        OnPointsChanged?.Invoke();
    }

    public bool TrySpendPoints(int cost)
    {
        if (CurrentPoints < cost)
            return false;

        CurrentPoints -= cost;
        OnPointsChanged?.Invoke();
        return true;
    }

    public void LosePoints(int amount)
    {
        if (amount <= 0)
            return;

        CurrentPoints -= amount;

        if (CurrentPoints < 0)
        {
            RelockNewestSkill();
        }
        else
        {
            OnPointsChanged?.Invoke();
        }
    }

    public int GetCost(SkillId skill)
    {
        switch (skill)
        {
            case SkillId.PhysicalMagic: return 6;
            case SkillId.MagicTraining: return 8;
            case SkillId.Ultimate: return 16;
            default: return 0;
        }
    }

    public bool IsUnlocked(SkillId skill)
    {
        return unlockedSkills.Contains(skill);
    }

    private bool HasPrerequisites(SkillId skill)
    {
        switch (skill)
        {
            case SkillId.BasicMagic: return true;
            case SkillId.PhysicalMagic: return IsUnlocked(SkillId.BasicMagic);
            case SkillId.MagicTraining: return IsUnlocked(SkillId.PhysicalMagic);
            case SkillId.Ultimate: return IsUnlocked(SkillId.MagicTraining);
            default: return false;
        }
    }

    public bool TryUnlock(SkillId skill)
    {
        if (IsUnlocked(skill) || !HasPrerequisites(skill))
            return false;

        int cost = GetCost(skill);
        if (!TrySpendPoints(cost))
            return false;

        unlockedSkills.Add(skill);
        unlockOrder.Push(skill);

        OnSkillUnlocked?.Invoke(skill);

        if (skill == SkillId.Ultimate)
            OnVictory?.Invoke();

        return true;
    }

    private void RelockNewestSkill()
    {
        if (unlockOrder.Count > 0)
        {
            SkillId relocked = unlockOrder.Pop();
            unlockedSkills.Remove(relocked);
            CurrentPoints = 0;

            OnSkillRelocked?.Invoke(relocked);
            OnPointsChanged?.Invoke();

            if (unlockOrder.Count == 0)
                OnDefeat?.Invoke();

            return;
        }

        CurrentPoints = 0;
        OnPointsChanged?.Invoke();
        OnDefeat?.Invoke();
    }
}