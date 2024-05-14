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

    private bool inizialized = false;
    
    public PlayerInputHandler player { get; private set; }

    public bool ready { get; private set; } = false;

    private void Start()
    {
        Inizialize();
        if(multiplayerConfirmationHandler != null)
            SetReady(false);
        ChangeToNotReady();
    }

    private void Inizialize()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();
        if (localizeStringEvent == null)
            localizeStringEvent = GetComponentInChildren<LocalizeStringEvent>();
        inizialized = true;
    }

    public void SetReady()
    {
        SetReady(!ready);
        Utility.DebugTrace(ready.ToString());
    }

    public void SetReady(bool ready)
    {
        if (!inizialized)
            return;
        
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
        localizeStringEvent.StringReference = pressToReadyTextAsset;
    }

    private void ChangeToReady()
    {
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
