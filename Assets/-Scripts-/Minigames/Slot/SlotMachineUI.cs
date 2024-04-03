using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTryText;

    [SerializeField] private MinigameMenu winScreen;
    [SerializeField] private MinigameMenu loseScreen;

    public void UpdateRemainingTryText(int value)
    {
        remainingTryText.text = value.ToString();
    }

    public void ShowWin()
    {
        winScreen.gameObject.SetActive(true);
    }

    public void Showlose()
    {
        loseScreen.gameObject.SetActive(true);
    }
}
