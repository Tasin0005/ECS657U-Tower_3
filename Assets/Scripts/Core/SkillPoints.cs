using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillId
{
    Basic,
    Physical,
    Mechanic,
    Magic,
    Imaginary,
    Ultimate
}

// Manages skill points (sole currency), 4 branch skill unlocking/relocking, and win/loss triggers
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
        unlockedSkills.Add(SkillId.Basic);
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
            case SkillId.Basic: return 0;
            case SkillId.Physical: return 6;
            case SkillId.Mechanic: return 10;
            case SkillId.Magic: return 10;
            case SkillId.Imaginary: return 12;
            case SkillId.Ultimate: return 20;
            default: return 0;
        }
    }

    // Only skills currently in the game can be unlocked. Mechanic, Mgic, and Imaginary stay unclickable for now
    public bool IsImplemented(SkillId skill)
    {
        return skill == SkillId.Basic || skill == SkillId.Physical || skill == SkillId.Ultimate;
    }

    public bool IsUnlocked(SkillId skill)
    {
        return unlockedSkills.Contains(skill);
    }

    public bool HasPrerequisites(SkillId skill)
    {
        switch (skill)
        {
            case SkillId.Basic:
                return true;
            case SkillId.Physical:
            case SkillId.Mechanic:
            case SkillId.Magic:
            case SkillId.Imaginary:
                return IsUnlocked(SkillId.Basic);
            case SkillId.Ultimate:
                return IsUnlocked(SkillId.Physical)
                    && IsUnlocked(SkillId.Mechanic)
                    && IsUnlocked(SkillId.Magic)
                    && IsUnlocked(SkillId.Imaginary);
            default:
                return false;
        }
    }

    public bool TryUnlock(SkillId skill)
    {
        if (!IsImplemented(skill) || IsUnlocked(skill) || !HasPrerequisites(skill))
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