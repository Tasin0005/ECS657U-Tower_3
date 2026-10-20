using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//onyl currency right now
public class SkillPoints : MonoBehaviour
{
    public static SkillPoints Instance { get; private set; }

    public int CurrentPoints = 6; //Change however

    void Awake()
    {
        Instance = this;
    }

    public bool TrySpendPoints(int cost)
    {
        if (CurrentPoints < cost) return false;
        CurrentPoints -= cost;
        return true;
    }
}
