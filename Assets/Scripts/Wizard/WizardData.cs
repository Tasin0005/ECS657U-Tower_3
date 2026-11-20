using UnityEngine;

public enum WizardType
{
    Basic,
    Advanced
}

// Holds a wizard's shared stats as a reusable ScriptableObject asset so team can tweak values
[CreateAssetMenu(fileName = "WizardData", menuName = "Tower Defence/Wizard Data")]
public class WizardData : ScriptableObject
{
    public WizardType type;
    public int cost = 2;
    public float range = 5f;
    public float damagePerSecond = 5f;
    public Color colour = Color.blue;
}