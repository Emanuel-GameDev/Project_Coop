using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI coinValueText;
    [SerializeField] TextMeshProUGUI keyValueText;

    public void SetUIValues(int coinValue, int keyValue)
    {
        coinValueText.text = coinValue.ToString();
        keyValueText.text = keyValue.ToString();   
    }
}
