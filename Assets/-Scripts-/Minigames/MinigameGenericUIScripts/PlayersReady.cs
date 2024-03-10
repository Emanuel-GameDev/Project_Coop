using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersReady : MinigameMenu
{
    [SerializeField]
    MultiplayerConfirmationHandler multiplayerConfirmationHandler;

    public override void Inizialize(InputReceiver activeReceiver)
    {
        base.Inizialize(activeReceiver);
        multiplayerConfirmationHandler.PlaceButtons();
    }

    public override void SubmitButton(InputReceiver player)
    {
        base.SubmitButton(player);
        multiplayerConfirmationHandler.GiveInput(player, true);
    }

    public override void CancelButton(InputReceiver player)
    {
        base.CancelButton(player);
        multiplayerConfirmationHandler.GiveInput(player, false);
    }

}
