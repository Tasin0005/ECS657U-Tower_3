using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Attached to each node in the horizontal Skill Tree. Handles clicks, colours, and hover tooltips
[RequireComponent(typeof(Button))]
public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillId skillId;
    public string displayTitle;
    [TextArea(2, 4)]
    public string description;

    [Header("Node Colours")]
    public Color unlockedColour = new Color(0.22f, 0.72f, 0.42f, 1f);
    public Color availableColour = new Color(0.85f, 0.88f, 0.92f, 1f);
    public Color lockedColour = new Color(0.30f, 0.30f, 0.30f, 1f);

    private Button button;
    private Image circleImage;

    public RectTransform RectTransform { get; private set; }

    private void Awake()
    {
        button = GetComponent<Button>();
        circleImage = GetComponent<Image>();
        RectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        button.onClick.AddListener(OnNodeClicked);
        RefreshVisual();
    }

    private void OnNodeClicked()
    {
        if (SkillPoints.Instance == null)
            return;

        if (SkillPoints.Instance.TryUnlock(skillId))
        {
            if (SkillTreeUI.Instance != null)
                SkillTreeUI.Instance.ShowTooltip(this);
        }
    }

    public void RefreshVisual()
    {
        if (circleImage == null || SkillPoints.Instance == null)
            return;

        SkillPoints sp = SkillPoints.Instance;

        if (sp.IsUnlocked(skillId))
        {
            circleImage.color = unlockedColour;
        }
        else if (sp.IsImplemented(skillId) && sp.HasPrerequisites(skillId))
        {
            circleImage.color = availableColour;
        }
        else
        {
            circleImage.color = lockedColour;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SkillTreeUI.Instance != null)
            SkillTreeUI.Instance.ShowTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SkillTreeUI.Instance != null)
            SkillTreeUI.Instance.HideTooltip();
    }
}