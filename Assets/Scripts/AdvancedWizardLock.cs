using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Member 4 - the Advanced Wizard shop button only works once Magic Training is unlocked.
// Put this on the Advanced Wizard button in the shop panel.
// Works alongside Member 2's WizardShopButton and Member 3's SkillPoints without changing them.
[RequireComponent(typeof(Button))]
public class AdvancedWizardLock : MonoBehaviour
{
    public WizardPlacer placer;   // drag in the WizardPlacer object

    [Header("Look while locked (same grey as the skill tree)")]
    public Color lockedColour = new Color(0.3f, 0.3f, 0.3f, 1f);
    public string lockedText = "Advanced Wizard\nLocked: needs Magic Training";

    private Button button;
    private Image image;
    private TMP_Text label;
    private Color unlockedColour;
    private string unlockedText;
    private bool wasUnlocked;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        label = GetComponentInChildren<TMP_Text>();
        if (image != null) unlockedColour = image.color;
    }

    private void Update()
    {
        if (SkillPoints.Instance == null)
            return;

        bool unlocked = SkillPoints.Instance.IsUnlocked(SkillId.MagicTraining);

        // Unclickable and grey until Magic Training is bought.
        button.interactable = unlocked;
        if (image != null)
            image.color = unlocked ? unlockedColour : lockedColour;

        if (label != null)
        {
            // Remember the shop's own text ("Advanced Wizard\n5 pts") the first time we see it.
            if (unlockedText == null && label.text != lockedText)
                unlockedText = label.text;

            if (!unlocked)
                label.text = lockedText;
            else if (unlockedText != null)
                label.text = unlockedText;
        }

        // If Magic Training gets relocked after a breach while Advanced is selected,
        // switch the selection back to Basic so no more Advanced wizards can be placed.
        if (wasUnlocked && !unlocked && placer != null)
            placer.SelectWizardType(WizardType.Basic);

        wasUnlocked = unlocked;
    }
}
