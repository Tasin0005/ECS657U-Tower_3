using UnityEngine;
using UnityEngine.UI;

// Handles skill tree button actions and updates locked/unlocked colours via events
public class SkillTreeUI : MonoBehaviour
{
    [Header("Skill Buttons")]
    public Button btnPhysicalMagic;
    public Button btnMagicTraining;
    public Button btnUltimate;

    [Header("Locked Appearance")]
    public Color lockedColour = new Color(0.3f, 0.3f, 0.3f, 1f);

    private Image physicalImage, trainingImage, ultimateImage;
    private Color physicalColour, trainingColour, ultimateColour;

    private void Awake()
    {
        CacheButtonImage(btnPhysicalMagic, out physicalImage, out physicalColour);
        CacheButtonImage(btnMagicTraining, out trainingImage, out trainingColour);
        CacheButtonImage(btnUltimate, out ultimateImage, out ultimateColour);
    }

    private void Start()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked += HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked += HandleSkillChanged;
        }

        RefreshColours();
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked -= HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked -= HandleSkillChanged;
        }
    }

    public void OnPhysicalMagic()
    {
        if (SkillPoints.Instance != null)
            SkillPoints.Instance.TryUnlock(SkillId.PhysicalMagic);
    }

    public void OnMagicTraining()
    {
        if (SkillPoints.Instance != null)
            SkillPoints.Instance.TryUnlock(SkillId.MagicTraining);
    }

    public void OnUltimate()
    {
        if (SkillPoints.Instance != null)
            SkillPoints.Instance.TryUnlock(SkillId.Ultimate);
    }

    private void HandleSkillChanged(SkillId _)
    {
        RefreshColours();
    }

    private void RefreshColours()
    {
        if (SkillPoints.Instance == null)
            return;

        SetColour(physicalImage, SkillId.PhysicalMagic, physicalColour);
        SetColour(trainingImage, SkillId.MagicTraining, trainingColour);
        SetColour(ultimateImage, SkillId.Ultimate, ultimateColour);
    }

    private void SetColour(Image image, SkillId skill, Color unlockedColour)
    {
        if (image != null)
            image.color = SkillPoints.Instance.IsUnlocked(skill) ? unlockedColour : lockedColour;
    }

    private static void CacheButtonImage(Button button, out Image image, out Color originalColour)
    {
        image = button != null ? button.GetComponent<Image>() : null;
        originalColour = image != null ? image.color : Color.white;
    }
}