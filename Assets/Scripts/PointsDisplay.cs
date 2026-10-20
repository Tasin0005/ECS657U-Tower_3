using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Shows the current points in the top right of the screen
public class PointsDisplay : MonoBehaviour
{
    public TextMeshProUGUI label;

    void Update()
    {
        label.text = "Points: " + SkillPoints.Instance.CurrentPoints;
    }
}
