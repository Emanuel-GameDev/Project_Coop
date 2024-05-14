using TMPro;
using UnityEngine;

public class SlotMachineUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI remainingTryText;

    //DA RIVEDERE #MODIFICATO

    //[SerializeField] private MinigameMenu winScreen;
    //[SerializeField] private MinigameMenu loseScreen;

    public void UpdateRemainingTryText(int value)
    {
        remainingTryText.text = value.ToString();
    }

    public void ShowWin()
    {
        // winScreen.gameObject.SetActive(true);
    }

    public void Showlose()
    {
        //loseScreen.gameObject.SetActive(true);
    }
}
