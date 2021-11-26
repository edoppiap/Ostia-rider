using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignBonusText : MonoBehaviour
{
    public void SetBonusText(string bonusText, string bonusNum)
    {
        foreach(Transform child in transform)
        {
            TextMeshProUGUI childText = child.GetComponent<TextMeshProUGUI>();
            if (child.name == "BonusPrefabText")
            {
                childText.SetText(bonusText);
            }else if (child.name == "BonusPrefagNumber")
            {
                childText.SetText(bonusNum);
            }

        }
    }
}
