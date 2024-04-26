using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class ReadyButton : MonoBehaviour
{
    [SerializeField]
    private Color ReadyColor = Color.green;
    [SerializeField]
    private Color NotReadyColor = Color.red;
    [SerializeField]
    LocalizedString pressToReadyTextAsset;
    [SerializeField]
    LocalizedString readyTextAsset;


    MultiplayerConfirmationHandler multiplayerConfirmationHandler;
    [SerializeField]
    Image buttonImage;
    LocalizeStringEvent localizeStringEvent;
    
    public PlayerInputHandler player { get; private set; }

    public bool ready { get; private set; } = false;

    private void Start()
    {
        if(buttonImage == null)
            buttonImage = GetComponent<Image>();
        localizeStringEvent = GetComponentInChildren<LocalizeStringEvent>();
        ChangeToNotReady();
    }

    public void SetReady()
    {
        SetReady(!ready);
        Utility.DebugTrace(ready.ToString());
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
        if(buttonImage == null)
            buttonImage = GetComponent<Image>();
        
        buttonImage.color = NotReadyColor;
        localizeStringEvent.StringReference = pressToReadyTextAsset;
    }

    private void ChangeToReady()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        buttonImage.color = ReadyColor;
        localizeStringEvent.StringReference = readyTextAsset;
    }

    public void InitialSetup(MultiplayerConfirmationHandler multiplayerConfirmationHandler, PlayerInputHandler player)
    {
        this.multiplayerConfirmationHandler = multiplayerConfirmationHandler;
        this.player = player;
    }

    public void ResetButton()
    {
        this.multiplayerConfirmationHandler = null;
        this.player = null;
        ready = false;
        ChangeToNotReady();
    }
}
