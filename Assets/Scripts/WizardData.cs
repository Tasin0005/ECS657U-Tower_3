using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds the wizard's stats as an asset so team can tune values
[CreateAssetMenu(fileName = "WizardData", menuName = "Tower Defence/Wizard Data")]
public class WizardData : ScriptableObject
{
    public WizardType type;
    public int cost = 2;
    public float range = 5f;
    public float damagePerSecond = 5f;
    public Color colour = Color.blue;
}
