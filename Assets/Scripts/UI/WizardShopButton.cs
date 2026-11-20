using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shop button for a wizard type
[RequireComponent(typeof(Button))]
public class WizardShopButton : MonoBehaviour
{
    public WizardData data;
    public WizardPlacer placer;
    public TextMeshProUGUI label;

    [Header("Skill Lock")]
    public SkillId requiredSkill = SkillId.BasicMagic;
    public Color lockedColour = new Color(0.3f, 0.3f, 0.3f, 1f);
    public string lockedText = "Advanced Wizard\nLocked: needs Magic Training";

    private Button button;
    private Image image;
    private Color unlockedColour;
    private string unlockedText;

    private void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        if (image != null)
            unlockedColour = image.color;
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("WizardShopButton: missing WizardData on " + name);
            return;
        }

        unlockedText = data.type + " Wizard\n" + data.cost + " pts";

        if (placer != null)
            button.onClick.AddListener(() => placer.SelectWizardType(data.type));

        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked += HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked += HandleSkillChanged;
        }

        RefreshState();
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked -= HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked -= HandleSkillChanged;
        }
    }

    private void HandleSkillChanged(SkillId _)
    {
        RefreshState();
    }

    private void RefreshState()
    {
        bool unlocked = SkillPoints.Instance == null || SkillPoints.Instance.IsUnlocked(requiredSkill);

        button.interactable = unlocked;

        if (image != null)
            image.color = unlocked ? unlockedColour : lockedColour;

        if (label != null)
            label.text = unlocked ? unlockedText : lockedText;
    }
}