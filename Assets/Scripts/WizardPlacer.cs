using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//Pick a wizard in the panel, then click an empty pad to place it.
//Rejects occupied pads, clicks that miss pads, and unaffordable wizards.
public class WizardPlacer : MonoBehaviour
{
    public GameObject basicPrefab;
    public GameObject advancedPrefab;
    public WizardData basicData;
    public WizardData advancedData;
    public Camera gameCamera;

    private WizardType? selected; // null until a button is clicked
    private HashSet<Transform> occupiedPads = new HashSet<Transform>();

    public void SelectWizardType(WizardType type)
    {
        selected = type;
    }

    void Update()
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame || !selected.HasValue) return;

        //Click on the panel is a button press, not a placement
        if (EventSystem.current.IsPointerOverGameObject()) return;

        TryPlace(Mouse.current.position.ReadValue());
    }

    void TryPlace(Vector2 screenPosition)
    {
        Transform pad = FindPadUnderMouse(screenPosition);
        if (pad == null || occupiedPads.Contains(pad)) return;

        bool isBasic = selected.Value == WizardType.Basic;
        WizardData data = isBasic ? basicData : advancedData;
        GameObject prefab = isBasic ? basicPrefab : advancedPrefab;

        if (!SkillPoints.Instance.TrySpendPoints(data.cost)) return;

        Vector3 top = pad.position + Vector3.up * (pad.localScale.y / 2f);
        Instantiate(prefab, top, Quaternion.identity);
        occupiedPads.Add(pad);
    }

    // RaycastAll because enemy spheres have colliders and could block a plain Raycast
    Transform FindPadUnderMouse(Vector2 screenPosition)
    {
        Ray ray = gameCamera.ScreenPointToRay(screenPosition);
        foreach (RaycastHit hit in Physics.RaycastAll(ray))
        {
            if (hit.transform.name.StartsWith("Pad_")) return hit.transform;
        }
        return null;
    }
}