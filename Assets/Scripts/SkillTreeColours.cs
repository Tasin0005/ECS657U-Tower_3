using UnityEngine;
using UnityEngine.UI;

// Member 4 - skill tree buttons are grey until bought, then show their colour.
// Put this on the SkillsTree object and drag in each skill button's Image.
// Reads Member 3's SkillPoints, so relocked skills turn grey again automatically.
public class SkillTreeColours : MonoBehaviour
{
    [Header("Skill button images (drag in the buttons)")]
    public Image physicalMagic;
    public Image magicTraining;
    public Image ultimate;

    [Header("Colour for skills not bought yet")]
    public Color lockedColour = new Color(0.3f, 0.3f, 0.3f, 1f);

    private Color physicalColour, trainingColour, ultimateColour;

    private void Awake()
    {
        // Remember each button's own colour to show once it is bought.
        if (physicalMagic != null) physicalColour = physicalMagic.color;
        if (magicTraining != null) trainingColour = magicTraining.color;
        if (ultimate != null) ultimateColour = ultimate.color;
    }

    private void Update()
    {
        if (SkillPoints.Instance == null)
            return;

        SetColour(physicalMagic, SkillId.PhysicalMagic, physicalColour);
        SetColour(magicTraining, SkillId.MagicTraining, trainingColour);
        SetColour(ultimate, SkillId.Ultimate, ultimateColour);
    }

    private void SetColour(Image image, SkillId skill, Color boughtColour)
    {
        if (image == null)
            return;

        image.color = SkillPoints.Instance.IsUnlocked(skill) ? boughtColour : lockedColour;
    }
}
