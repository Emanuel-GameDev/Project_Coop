using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class MultiplayerConfirmationHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject readyButtonPrefab;

    [SerializeField]
    private RectTransform readyButtonsPosition;

    [SerializeField]
    private UnityEvent onAllReady;

    [SerializeField]
    private GameObject backgroundImage;

    [Header("Countdown Settings")]

    [SerializeField, Tooltip("Durata del countdown in secondi"), Range(0f, 10f)]
    private int countdownDuration = 3;

    [SerializeField]
    private TMP_Text countdownText;

    [SerializeField]
    private UnityEvent onCooldownEnded;

    private bool countdownStarted = false;

    private int readyCount = 0;

    private int playerCount = 0;

    List<ReadyButton> readyButtons = new();

    public void PlaceButtons()
    {
        SetBackgroundActive(true);

        playerCount = CoopManager.Instance.GetActiveHandlers().Count;
        readyCount = 0;
        foreach (PlayerInputHandler player in CoopManager.Instance.GetActiveHandlers())
        {
            GameObject newButton = Instantiate(readyButtonPrefab, readyButtonsPosition);

            newButton.SetActive(true);

            ReadyButton readyButton = newButton.GetComponent<ReadyButton>();
            readyButton.InitialSetup(this, player);

            player.SetPlayerActiveMenu(newButton, newButton);

            readyButtons.Add(readyButton);
        }
    }


    public void ChangeReady(ReadyButton button)
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
        foreach (ReadyButton b in readyButtons)
        {
            b.player.SetPlayerActiveMenu(null, null);
            Destroy(b.gameObject);
        }
        readyButtons.Clear();
    }

    public void StartCountdown()
    {
        if (!countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(CountdownCoroutine(countdownDuration));
        }
    }

    IEnumerator CountdownCoroutine(float duration)
    {
        float startTime = Time.realtimeSinceStartup;
        float timeLeft = duration + 0.99f;

        while (timeLeft > 0)
        {
            float elapsedTime = Time.realtimeSinceStartup - startTime;

            timeLeft -= elapsedTime;
            int secondsLeft = Mathf.FloorToInt(timeLeft);
            secondsLeft = secondsLeft < 0 ? 0 : secondsLeft;
            countdownText.text = secondsLeft.ToString();
            startTime = Time.realtimeSinceStartup;
            yield return null;
        }

        onCooldownEnded?.Invoke();
        countdownText.text = "";

        countdownStarted = false;
    }

    public void SetBackgroundActive(bool active)
    {
        backgroundImage.SetActive(active);
    }

}
