using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

// Picks wizard in the shop panel, then places it on an empty pad when clicked
public class WizardPlacer : MonoBehaviour
{
    public GameObject basicPrefab;
    public GameObject advancedPrefab;
    public WizardData basicData;
    public WizardData advancedData;
    public Camera gameCamera;

    private WizardType? selected;
    private readonly HashSet<Transform> occupiedPads = new HashSet<Transform>();
    private readonly RaycastHit[] raycastHits = new RaycastHit[16];

    private void Start()
    {
        if (SkillPoints.Instance != null)
            SkillPoints.Instance.OnSkillRelocked += HandleSkillRelocked;
    }

    private void OnDestroy()
    {
        if (SkillPoints.Instance != null)
            SkillPoints.Instance.OnSkillRelocked -= HandleSkillRelocked;
    }

    public void SelectWizardType(WizardType type)
    {
        selected = type;
    }

    private void HandleSkillRelocked(SkillId relocked)
    {
        if (relocked == SkillId.MagicTraining && selected == WizardType.Advanced)
            SelectWizardType(WizardType.Basic);
    }

    private void Update()
    {
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame || !selected.HasValue)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        TryPlace(Mouse.current.position.ReadValue());
    }

    private void TryPlace(Vector2 screenPosition)
    {
        Transform pad = FindPadUnderMouse(screenPosition);
        if (pad == null || occupiedPads.Contains(pad))
            return;

        bool isBasic = selected.Value == WizardType.Basic;
        WizardData data = isBasic ? basicData : advancedData;
        GameObject prefab = isBasic ? basicPrefab : advancedPrefab;

        if (data == null || prefab == null || SkillPoints.Instance == null)
            return;

        if (!SkillPoints.Instance.TrySpendPoints(data.cost))
            return;

        Vector3 top = pad.position + Vector3.up * (pad.localScale.y / 2f);
        Instantiate(prefab, top, Quaternion.identity);
        occupiedPads.Add(pad);
    }

    // RaycastNonAlloc avoids per-click array allocations while still passing through enemy colliders.
    private Transform FindPadUnderMouse(Vector2 screenPosition)
    {
        if (gameCamera == null)
            return null;

        Ray ray = gameCamera.ScreenPointToRay(screenPosition);
        int hitCount = Physics.RaycastNonAlloc(ray, raycastHits);

        for (int i = 0; i < hitCount; i++)
        {
            Transform hitTransform = raycastHits[i].transform;
            if (hitTransform.name.StartsWith("Pad_"))
                return hitTransform;
        }

        return null;
    }
}