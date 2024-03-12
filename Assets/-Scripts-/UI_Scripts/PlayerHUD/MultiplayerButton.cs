using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class MultiplayerButton : MonoBehaviour
{
    [SerializeField]
    private Color ReadyColor = Color.green;
    [SerializeField]
    private Color NotReadyColor = Color.red;
    [SerializeField]
    string defaultText = "Not Ready";
    [SerializeField]
    string readyText = "Ready!";

    MultiplayerConfirmationHandler multiplayerConfirmationHandler;
    Image buttonImage;
    TMP_Text buttonText;
    
    public ePlayerID playerID { get; private set; }

    public bool ready { get; private set; } = false;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>();
        ChangeToNotReady();
    }

    public void SetReady(bool ready)
    {
        if (ready == this.ready)
            return;

        this.ready = ready;
        
        if (ready)
            ChangeToReady();
        else
            ChangeToNotReady();
        
        multiplayerConfirmationHandler.ChangeReady(this);
    }

    private void ChangeToNotReady()
    {
        buttonImage.color = NotReadyColor;
        buttonText.text = defaultText;
    }

    private void ChangeToReady()
    {
        buttonImage.color = ReadyColor;
        buttonText.text = readyText;
    }

    public void InitialSetup(MultiplayerConfirmationHandler multiplayerConfirmationHandler, ePlayerID playerID)
    {
        this.multiplayerConfirmationHandler = multiplayerConfirmationHandler;
        this.playerID = playerID;
    }

}
