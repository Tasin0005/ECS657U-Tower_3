using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Wizard card in the right sidebar. shows: name, SP cost, lock state, and yellow selection border.
[RequireComponent(typeof(Button))]
public class WizardShopButton : MonoBehaviour
{
    public WizardData data;
    public WizardPlacer placer;

    [Header("Card UI")]
    public TMP_Text nameLabel;
    public TMP_Text costLabel;
    public Image iconImage;
    public GameObject selectionBorder;

    [Header("Skill Lock")]
    public SkillId requiredSkill = SkillId.Basic;
    public Color lockedCardColour = new Color(0.16f, 0.20f, 0.26f, 1f);
    public Color lockedIconColour = new Color(0.35f, 0.38f, 0.44f, 1f);
    public string lockedText = "Unlock in\nSkill Tree";

    private Button button;
    private Image cardImage;
    private Color unlockedCardColour;
    private Color unlockedIconColour;

    private void Awake()
    {
        button = GetComponent<Button>();
        cardImage = GetComponent<Image>();

        if (cardImage != null)
            unlockedCardColour = cardImage.color;

        if (iconImage != null)
            unlockedIconColour = iconImage.color;

        if (selectionBorder != null)
            selectionBorder.SetActive(false);
    }

    private void Start()
    {
        if (data == null)
        {
            Debug.LogError("WizardShopButton: missing WizardData on " + name);
            return;
        }

        if (placer != null)
        {
            button.onClick.AddListener(() => placer.SelectWizardType(data.type));
            placer.OnWizardSelected += HandleWizardSelected;
        }

        if (SkillPoints.Instance != null)
        {
            SkillPoints.Instance.OnSkillUnlocked += HandleSkillChanged;
            SkillPoints.Instance.OnSkillRelocked += HandleSkillChanged;
        }

        RefreshState();
        HandleWizardSelected(placer != null ? placer.SelectedType : null);
    }

    private void OnDestroy()
    {
        if (placer != null)
            placer.OnWizardSelected -= HandleWizardSelected;

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

    private void HandleWizardSelected(WizardType? selectedType)
    {
        if (selectionBorder == null || data == null)
            return;

        bool isSelected = button.interactable && selectedType.HasValue && selectedType.Value == data.type;
        selectionBorder.SetActive(isSelected);
    }

    private void RefreshState()
    {
        bool unlocked = SkillPoints.Instance == null || SkillPoints.Instance.IsUnlocked(requiredSkill);

        button.interactable = unlocked;

        if (cardImage != null)
            cardImage.color = unlocked ? unlockedCardColour : lockedCardColour;

        if (iconImage != null)
            iconImage.color = unlocked ? unlockedIconColour : lockedIconColour;

        if (nameLabel != null)
            nameLabel.text = unlocked ? data.type + " Wizard" : lockedText;

        if (costLabel != null)
        {
            costLabel.gameObject.SetActive(unlocked);
            if (unlocked)
                costLabel.text = data.cost + " SP";
        }

        if (!unlocked && selectionBorder != null)
            selectionBorder.SetActive(false);
    }
}