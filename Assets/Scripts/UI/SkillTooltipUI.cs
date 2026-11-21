using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Floating tooltip box shown when hovering over a SkillNodeUI node
[RequireComponent(typeof(CanvasGroup))]
public class SkillTooltipUI : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public TMP_Text costText;
    public TMP_Text statusText;

    [Header("Positioning")]
    public Vector2 offset = new Vector2(0f, 110f);
    public float edgePadding = 16f;

    [Header("Status Colours")]
    public Color unlockedColour = new Color(0.30f, 0.85f, 0.45f, 1f);
    public Color availableColour = new Color(1f, 0.84f, 0.25f, 1f);
    public Color lockedColour = new Color(0.90f, 0.35f, 0.35f, 1f);
    public Color comingSoonColour = new Color(0.65f, 0.68f, 0.72f, 1f);

    private RectTransform tooltipRect;
    private RectTransform parentRect;

    private void Awake()
    {
        tooltipRect = GetComponent<RectTransform>();
        parentRect = tooltipRect.parent as RectTransform;

        // Prevent tooltip from blocking pointer hover on nodes.
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        Hide();
    }

    public void Show(SkillNodeUI node)
    {
        if (node == null || SkillPoints.Instance == null)
            return;

        SkillPoints sp = SkillPoints.Instance;
        SkillId id = node.skillId;
        int cost = sp.GetCost(id);

        if (titleText != null)
            titleText.text = node.displayTitle;

        if (descriptionText != null)
            descriptionText.text = node.description;

        if (costText != null)
            costText.text = cost > 0 ? "Cost: " + cost + " SP" : "Cost: Free";

        if (statusText != null)
        {
            if (sp.IsUnlocked(id))
            {
                statusText.text = "Unlocked";
                statusText.color = unlockedColour;
            }
            else if (!sp.IsImplemented(id))
            {
                statusText.text = "Coming Soon";
                statusText.color = comingSoonColour;
            }
            else if (sp.HasPrerequisites(id) && sp.CurrentPoints >= cost)
            {
                statusText.text = "Available";
                statusText.color = availableColour;
            }
            else
            {
                statusText.text = "Locked";
                statusText.color = lockedColour;
            }
        }

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipRect);
        PositionNearNode(node.RectTransform);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void PositionNearNode(RectTransform nodeRect)
    {
        if (parentRect == null || nodeRect == null)
            return;

        tooltipRect.pivot = new Vector2(0.5f, 0.5f);

        Vector3 nodeWorldCenter = nodeRect.TransformPoint(nodeRect.rect.center);
        Vector2 localPos = (Vector2)parentRect.InverseTransformPoint(nodeWorldCenter) + offset;

        // Flip below the node if placing above would clip the top of the window.
        float halfW = tooltipRect.rect.width * 0.5f;
        float halfH = tooltipRect.rect.height * 0.5f;
        Rect bounds = parentRect.rect;

        if (localPos.y + halfH > bounds.yMax - edgePadding)
            localPos.y = ((Vector2)parentRect.InverseTransformPoint(nodeWorldCenter)).y - offset.y;

        localPos.x = Mathf.Clamp(localPos.x, bounds.xMin + halfW + edgePadding, bounds.xMax - halfW - edgePadding);
        localPos.y = Mathf.Clamp(localPos.y, bounds.yMin + halfH + edgePadding, bounds.yMax - halfH - edgePadding);

        tooltipRect.localPosition = new Vector3(localPos.x, localPos.y, 0f);
    }
}