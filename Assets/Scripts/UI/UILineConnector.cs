using UnityEngine;

// Automatically creates a line between two UI RectTransform (UI elements) nodes 
[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class UILineConnector : MonoBehaviour
{
    public RectTransform startNode;
    public RectTransform endNode;
    public float thickness = 6f;

    private RectTransform lineRect;

    private void OnEnable()
    {
        UpdateLine();
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdateLine();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
            UpdateLine();
    }
#endif

    public void UpdateLine()
    {
        if (startNode == null || endNode == null)
            return;

        if (lineRect == null)
            lineRect = GetComponent<RectTransform>();

        RectTransform parent = lineRect.parent as RectTransform;
        if (parent == null)
            return;

        Vector2 startPos = parent.InverseTransformPoint(startNode.TransformPoint(startNode.rect.center));
        Vector2 endPos = parent.InverseTransformPoint(endNode.TransformPoint(endNode.rect.center));
        Vector2 diff = endPos - startPos;

        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0.5f, 0.5f);
        lineRect.localPosition = new Vector3((startPos.x + endPos.x) * 0.5f, (startPos.y + endPos.y) * 0.5f, 0f);
        lineRect.sizeDelta = new Vector2(diff.magnitude, thickness);
        lineRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
    }
}