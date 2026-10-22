using System.Collections.Generic;
using UnityEngine;

//The four skills in the tree
public enum SkillId
{
    BasicMagic,
    PhysicalMagic,
    MagicTraining,
    Ultimate
}

//One currency and the skill tree
public class SkillPoints : MonoBehaviour
{
    public static SkillPoints Instance;

    public int CurrentPoints = 6;

    //Unlocked skills
    private List<SkillId> unlockedSkills = new List<SkillId>();

    //Unlock order for relocking
    private List<SkillId> unlockOrder = new List<SkillId>();

    //UI events
    public System.Action OnPointsChanged;
    public System.Action<SkillId> OnSkillUnlocked;
    public System.Action<SkillId> OnSkillRelocked;
    public System.Action OnVictory;
    public System.Action OnDefeat;

    //Setup
    void Awake()
    {
        Instance = this;
        unlockedSkills.Add(SkillId.BasicMagic);
    }

    //Subscribe to enemy events
    void OnEnable()
    {
        GameEvents.EnemyKilled += HandleEnemyKilled;
        GameEvents.EnemyBreached += HandleEnemyBreached;
    }

    //Unsubscribe
    void OnDisable()
    {
        GameEvents.EnemyKilled -= HandleEnemyKilled;
        GameEvents.EnemyBreached -= HandleEnemyBreached;
    }

    //Kill gives points
    void HandleEnemyKilled(int points)
    {
        AddPoints(points);
    }

    //Breach takes points
    void HandleEnemyBreached(int points)
    {
        LosePoints(points);
    }

    //Add points and refresh UI
    public void AddPoints(int amount)
    {
        if (amount <= 0)
            return;

        CurrentPoints = CurrentPoints + amount;

        if (OnPointsChanged != null)
            OnPointsChanged();
    }

    //Spend points if affordable
    public bool TrySpendPoints(int cost)
    {
        if (CurrentPoints < cost)
            return false;

        CurrentPoints = CurrentPoints - cost;

        if (OnPointsChanged != null)
            OnPointsChanged();

        return true;
    }

    //Lose points and relock if below zero
    public void LosePoints(int amount)
    {
        if (amount <= 0)
            return;

        CurrentPoints = CurrentPoints - amount;

        if (CurrentPoints < 0)
        {
            RelockNewestSkill();
        }
        else
        {
            if (OnPointsChanged != null)
                OnPointsChanged();
        }
    }

    //Cost of a skill
    public int GetCost(SkillId skill)
    {
        if (skill == SkillId.PhysicalMagic) return 6;
        if (skill == SkillId.MagicTraining) return 8;
        if (skill == SkillId.Ultimate) return 16;
        return 0;
    }

    //Check if skill is unlocked
    public bool IsUnlocked(SkillId skill)
    {
        return unlockedSkills.Contains(skill);
    }

    //Check if previous skill is unlocked
    private bool HasPrerequisites(SkillId skill)
    {
        if (skill == SkillId.BasicMagic)
            return true;

        if (skill == SkillId.PhysicalMagic)
            return IsUnlocked(SkillId.BasicMagic);

        if (skill == SkillId.MagicTraining)
            return IsUnlocked(SkillId.PhysicalMagic);

        if (skill == SkillId.Ultimate)
            return IsUnlocked(SkillId.MagicTraining);

        return false;
    }

    //Unlock a skill if allowed
    public bool TryUnlock(SkillId skill)
    {
        if (IsUnlocked(skill))
            return false;

        if (HasPrerequisites(skill) == false)
            return false;

        int cost = GetCost(skill);

        if (TrySpendPoints(cost) == false)
            return false;

        unlockedSkills.Add(skill);
        unlockOrder.Add(skill);

        if (OnSkillUnlocked != null)
            OnSkillUnlocked(skill);

        if (skill == SkillId.Ultimate)
        {
            if (OnVictory != null)
                OnVictory();
        }

        return true;
    }

    //Relock newest non-basic skill
    private void RelockNewestSkill()
    {
        for (int i = unlockOrder.Count - 1; i >= 0; i--)
        {
            if (unlockOrder[i] != SkillId.BasicMagic)
            {
                SkillId relocked = unlockOrder[i];

                unlockOrder.RemoveAt(i);
                unlockedSkills.Remove(relocked);

                CurrentPoints = 0;

                if (OnSkillRelocked != null)
                    OnSkillRelocked(relocked);

                if (OnPointsChanged != null)
                    OnPointsChanged();

                if (unlockOrder.Count == 0)
                {
                    if (OnDefeat != null)
                        OnDefeat();
                }

                return;
            }
        }

        CurrentPoints = 0;

        if (OnPointsChanged != null)
            OnPointsChanged();

        if (OnDefeat != null)
            OnDefeat();
    }

    //Reset everything
    public void ResetSkills()
    {
        CurrentPoints = 6;
        unlockedSkills.Clear();
        unlockedSkills.Add(SkillId.BasicMagic);
        unlockOrder.Clear();

        if (OnPointsChanged != null)
            OnPointsChanged();
    }
}