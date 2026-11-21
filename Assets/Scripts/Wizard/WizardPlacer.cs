using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

// Picks a wizard in the shop panel, then places it on an empty pad when clicked
public class WizardPlacer : MonoBehaviour
{
    public GameObject basicPrefab;
    [FormerlySerializedAs("advancedPrefab")]
    public GameObject physicalPrefab;
    public WizardData basicData;
    [FormerlySerializedAs("advancedData")]
    public WizardData physicalData;
    public Camera gameCamera;

    public WizardType? SelectedType => selected;
    public event Action<WizardType?> OnWizardSelected;

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
        OnWizardSelected?.Invoke(selected);
    }

    private void HandleSkillRelocked(SkillId relocked)
    {
        if (relocked == SkillId.Physical && selected == WizardType.Physical)
            SelectWizardType(WizardType.Basic);
    }

    private void Update()
    {
        if ((SkillTreeUI.Instance != null && SkillTreeUI.Instance.IsOpen) ||
            (SettingsMenu.Instance != null && SettingsMenu.Instance.IsOpen))
            return;

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
        WizardData data = isBasic ? basicData : physicalData;
        GameObject prefab = isBasic ? basicPrefab : physicalPrefab;

        if (data == null || prefab == null || SkillPoints.Instance == null)
            return;

        if (!SkillPoints.Instance.TrySpendPoints(data.cost))
            return;

        Vector3 top = pad.position + Vector3.up * (pad.localScale.y / 2f);
        Instantiate(prefab, top, Quaternion.identity);
        occupiedPads.Add(pad);
    }

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