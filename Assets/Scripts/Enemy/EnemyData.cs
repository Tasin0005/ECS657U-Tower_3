using UnityEngine;

public enum EnemyType
{
    Basic,
    Strong
}

// Holds an enemys shared stats as a reusable ScriptableObject asset so team members can easily tweak
[CreateAssetMenu(fileName = "EnemyData", menuName = "Tower Defence/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public EnemyType type;
    public float maxHealth = 20f;
    public int killReward = 2;
    public int breachPenalty = 1;
    public float moveSpeed = 2.5f;
    public float travelHeight = 0.5f;
}