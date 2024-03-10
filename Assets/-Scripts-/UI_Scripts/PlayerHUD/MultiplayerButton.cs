using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerButton : MonoBehaviour
{
    MultiplayerConfirmationHandler multiplayerConfirmationHandler;

    public ePlayerID playerID { get; private set; }

    public bool ready { get; private set; } = false;

    public void SetReady(bool ready)
    {
        this.ready = ready;
        multiplayerConfirmationHandler.ChangeReady(this);
    }

    public void InitialSetup(MultiplayerConfirmationHandler multiplayerConfirmationHandler, ePlayerID playerID)
    {
        this.multiplayerConfirmationHandler = multiplayerConfirmationHandler;
        this.playerID = playerID;
    }

}
