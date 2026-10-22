using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Minimal skill tree UI to test SkillPoints
public class SkillTreeUI : MonoBehaviour
{
    public Button btnPhysicalMagic;
    public Button btnMagicTraining;
    public Button btnUltimate;

    //Wire buttons to SkillPoints
    void Start()
    {
        btnPhysicalMagic.onClick.AddListener(OnPhysicalMagic);
        btnMagicTraining.onClick.AddListener(OnMagicTraining);
        btnUltimate.onClick.AddListener(OnUltimate);
    }

    //PhysicalMagic button
    public void OnPhysicalMagic()
    {
        SkillPoints.Instance.TryUnlock(SkillId.PhysicalMagic);
    }

    //MagicTraining button
    public void OnMagicTraining()
    {
        SkillPoints.Instance.TryUnlock(SkillId.MagicTraining);
    }

    //Ultimate button
    public void OnUltimate()
    {
        SkillPoints.Instance.TryUnlock(SkillId.Ultimate);
    }
}