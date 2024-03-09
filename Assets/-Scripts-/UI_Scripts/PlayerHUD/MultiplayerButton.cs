using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerButton : MonoBehaviour
{
    MultiplayerConfirmationHandler multiplayerConfirmationHandler;

    public bool ready { get; private set; } = false;

    public void SetReady(bool ready)
    {
        this.ready = ready;
        multiplayerConfirmationHandler.ChangeReady(this);
    }

    public void SetMultiplayerConfirmationHandler(MultiplayerConfirmationHandler multiplayerConfirmationHandler)
    {
        this.multiplayerConfirmationHandler = multiplayerConfirmationHandler;
    }

}
