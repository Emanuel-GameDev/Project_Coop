using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerConfirmationHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject confirmButton;

    [SerializeField]
    private RectTransform confirmationButtonCenter;

    [SerializeField, Tooltip("Distanza tra gli elementi")]
    float spacing = 10f;

    public UnityEvent onAllReady;

    private int readyCount = 0;

    private int playerCount = 0;

    List<MultiplayerButton> buttons = new();

    public void PlaceButtons()
    {
        playerCount = CoopManager.Instance.GetActiveHandlers().Count;
        readyCount = 0;
        float panelWidth = confirmButton.GetComponent<RectTransform>().rect.width + spacing;
        float initialPos = -panelWidth * (playerCount - 1) / 2;
        int i = 0;
        foreach (PlayerInputHandler player in CoopManager.Instance.GetActiveHandlers())
        {
            GameObject newButton = Instantiate(confirmButton);

            newButton.SetActive(true);

            MultiplayerButton button = newButton.GetComponent<MultiplayerButton>();
            button.InitialSetup(this, player.playerID);
            buttons.Add(button);

            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();

            buttonTransform.SetParent(confirmationButtonCenter);

            float xPos = initialPos + (i * panelWidth);

            buttonTransform.localPosition = new Vector2(xPos, 0);

            i++;
        }

    }

    public void GiveInput(ePlayerID playerID, bool isReady)
    {
        MultiplayerButton button = buttons.Find(x => x.playerID == playerID);
        if(button != null)
            button.SetReady(isReady);
    }


    public void ChangeReady(MultiplayerButton button)
    {
        if (button.ready)
        {
            readyCount++;
        }
        else
        {
            readyCount--;
        }

        if (readyCount >= playerCount)
        {
            onAllReady?.Invoke();
        }
    }

    public void ResetButtons()
    {
        foreach (MultiplayerButton b in buttons)
        {
            b.SetReady(false);
        }
    }
}
