using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Minimal skill tree UI to test SkillPoints
public class SkillTreeUI : MonoBehaviour
{
    public TextMeshProUGUI pointsLabel;
    public Button btnPhysicalMagic;
    public Button btnMagicTraining;
    public Button btnUltimate;
    public Button btnTestBreach;

    //Wire buttons to SkillPoints
    void Start()
    {
        btnPhysicalMagic.onClick.AddListener(OnPhysicalMagic);
        btnMagicTraining.onClick.AddListener(OnMagicTraining);
        btnUltimate.onClick.AddListener(OnUltimate);
        btnTestBreach.onClick.AddListener(OnTestBreach);

        SkillPoints.Instance.OnPointsChanged += RefreshLabel;
        RefreshLabel();
    }

    //Refresh label
    public void RefreshLabel()
    {
        pointsLabel.text = "Points: " + SkillPoints.Instance.CurrentPoints;
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

    //Test breach button
    public void OnTestBreach()
    {
        GameEvents.RaiseEnemyBreached(20);
    }
}