using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayersReady : MinigameMenu
{
    [SerializeField]
    MultiplayerConfirmationHandler multiplayerConfirmationHandler;
    [SerializeField]
    int waitSeconds = 3;
    [SerializeField]
    TMP_Text countdownText;
    [SerializeField]
    UnityEvent onCooldownEnded;

    bool countdownStarted = false;

    public override void Inizialize(ePlayerID activeReceiver)
    {
        if (!countdownStarted)
        {
            base.Inizialize(activeReceiver);
            multiplayerConfirmationHandler.PlaceButtons();
        }
    }

    public override void SubmitButton(ePlayerID player)
    {
        if (!countdownStarted)
        {
            base.SubmitButton(player);
            multiplayerConfirmationHandler.GiveInput(player, true);
        }
    }

    public override void CancelButton(ePlayerID player)
    {
        if (!countdownStarted)
        {
            base.CancelButton(player);
            multiplayerConfirmationHandler.GiveInput(player, false);
        }
    }

    public override void MenuButton(ePlayerID player)
    {
        if (!countdownStarted)
        {
            base.MenuButton(player);
        }
    }

    public override void NavigateButton(Vector2 direction, ePlayerID player)
    {
        if (!countdownStarted)
        {
            base.NavigateButton(direction, player);
        }
    }

    public void StartCountdown()
    {
        if (!countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(CountdownCoroutine(waitSeconds));
        }
    }

    IEnumerator CountdownCoroutine(float duration)
    {
        float startTime = Time.realtimeSinceStartup;
        float timeLeft = duration + 0.99f;

        while (timeLeft >= 0)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;

            timeLeft -= elapsedTime;
            int secondsLeft = Mathf.FloorToInt(timeLeft);
            countdownText.text = secondsLeft.ToString();
            startTime = Time.realtimeSinceStartup;
            yield return null;
            //Debug.Log(timeLeft);
        }

        onCooldownEnded?.Invoke();
        countdownText.text = "";
        multiplayerConfirmationHandler.ResetButtons();
        countdownStarted = false;
    }

}

