using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//one button in the wizard panel, shows the wizard's name and price, selects it when clicked
[RequireComponent(typeof(Button))]
public class WizardShopButton : MonoBehaviour
{
    public WizardData data;
    public WizardPlacer placer;
    public TextMeshProUGUI label;

    void Start()
    {
        label.text = data.type + " Wizard\n" + data.cost + " pts";
        GetComponent<Button>().onClick.AddListener(() => placer.SelectWizardType(data.type));
    }
}