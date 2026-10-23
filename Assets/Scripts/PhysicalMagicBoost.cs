using UnityEngine;

// Member 4 - Physical Magic makes basic (blue) wizards stronger.
// Put this on the SkillPoints object (or any object in the scene).
// Works alongside Member 2's WizardAttack and Member 3's SkillPoints without changing them:
// it gives basic wizards a boosted copy of their stats while Physical Magic is unlocked,
// and switches them back if the skill is relocked. The original WizardData asset is never edited.
public class PhysicalMagicBoost : MonoBehaviour
{
    public WizardData basicData;          // drag in Assets/WizardData/BasicWizardData
    public float bonusDamagePerSecond = 2f;

    private WizardData boostedData;

    private void Awake()
    {
        if (basicData == null)
        {
            Debug.LogError("PhysicalMagicBoost: drag BasicWizardData into Basic Data");
            enabled = false;
            return;
        }

        // A runtime copy, so the real asset keeps its normal values.
        boostedData = Instantiate(basicData);
        boostedData.damagePerSecond = basicData.damagePerSecond + bonusDamagePerSecond;
    }

    private void Update()
    {
        if (SkillPoints.Instance == null)
            return;

        bool boosted = SkillPoints.Instance.IsUnlocked(SkillId.PhysicalMagic);
        WizardData wanted = boosted ? boostedData : basicData;

        foreach (WizardAttack wizard in FindObjectsByType<WizardAttack>(FindObjectsSortMode.None))
        {
            // Only basic wizards are affected.
            if (wizard.data == basicData || wizard.data == boostedData)
                wizard.data = wanted;
        }
    }
}
