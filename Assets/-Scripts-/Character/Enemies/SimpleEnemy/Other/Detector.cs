using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    [SerializeField] UnityEvent OnOnePlayerEnter;
    [SerializeField] UnityEvent OnAllPlayerInside;
    [SerializeField] TextMeshProUGUI playerInsideCount;

    List<PlayerCharacter> playersDetected;
    int playersInside;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        playersDetected = new List<PlayerCharacter>();

        if (playerInsideCount != null)
            playerInsideCount.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter character))
        {
            playersDetected.Add(character);
            playersInside++;

            OnOnePlayerEnter?.Invoke();

            if (playerInsideCount != null)
            {
                playerInsideCount.gameObject.SetActive(true);
                playerInsideCount.text = $"{playersInside}/{GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>().Length}";
            }

            if (playersDetected.Count >= CoopManager.Instance.GetActiveHandlers().Count)
            {
                OnAllPlayerInside?.Invoke();

                if (playerInsideCount != null)
                    playerInsideCount.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter character))
        {
            if(playersDetected.Contains(character))
            {
                playersDetected.Remove(character);
                playersInside--;

                if (playerInsideCount != null)
                {
                    playerInsideCount.gameObject.SetActive(true);
                    playerInsideCount.text = $"{playersInside}/{GameManager.Instance.coopManager.GetComponentsInChildren<PlayerInputHandler>().Length}";

                    if (playersDetected.Count >= 0)
                    {
                        playerInsideCount.gameObject.SetActive(false);
                    }
                }

            }
        }
    }

    public List<PlayerCharacter> GetPlayersDetected()
    {
        return playersDetected;
    }

    public int GetPlayersCountInTrigger()
    {
        return playersInside;
    }

    public void ClearList()
    {
        playersDetected.Clear();
        playersInside = 0;
    }
}
