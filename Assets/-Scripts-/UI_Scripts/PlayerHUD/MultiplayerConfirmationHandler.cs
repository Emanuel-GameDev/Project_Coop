using System;
using System.Collections;
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

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            PlaceButtons();
    }

    void PlaceButtons()
    {
        playerCount = CoopManager.Instance.GetActiveHandlers().Count;

        float panelWidth = confirmButton.GetComponent<RectTransform>().rect.width + spacing;

        float initialPos = -panelWidth * (playerCount - 1) / 2;

        for (int i = 0; i < playerCount; i++)
        {
            GameObject newButton = Instantiate(confirmButton);

            newButton.SetActive(true);

            newButton.GetComponent<MultiplayerButton>().SetMultiplayerConfirmationHandler(this);

            RectTransform buttonTransform = newButton.GetComponent<RectTransform>();

            buttonTransform.SetParent(confirmationButtonCenter);

            float xPos = initialPos + (i * panelWidth);

            buttonTransform.transform.localPosition = new Vector2(xPos, 0);

            
        }
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
}
