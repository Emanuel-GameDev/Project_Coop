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

    List<EnemyCharacter> enemiesDetected;
    List<PlayerCharacter> playersDetected;
    int playersInside;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        playersDetected = new List<PlayerCharacter>();
        enemiesDetected = new List<EnemyCharacter>();
        if (playerInsideCount != null)
            playerInsideCount.gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.GetComponentInParent<PlayerCharacter>())
        {
            if (playersDetected.Contains(other.gameObject.GetComponentInParent<PlayerCharacter>()))
                return;

            playersDetected.Add(other.gameObject.GetComponentInParent<PlayerCharacter>());
            playersInside++;

            OnOnePlayerEnter?.Invoke();

            if (playerInsideCount != null)
            {
                playerInsideCount.gameObject.SetActive(true);
                playerInsideCount.text = $"{playersInside}/{GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>().Length}";
            }

            if (playersDetected.Count >= CoopManager.Instance.GetActiveHandlers().Count)
            {
                OnAllPlayerInside?.Invoke();

                if (playerInsideCount != null)
                    playerInsideCount.gameObject.SetActive(false);
            }
        }

        if (other.gameObject.GetComponent<EnemyCharacter>())
        {
            if (enemiesDetected.Contains(other.gameObject.GetComponent<EnemyCharacter>()))
                return;

            enemiesDetected.Add(other.gameObject.GetComponent<EnemyCharacter>());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInParent<PlayerCharacter>())
        {
            if(playersDetected.Contains(other.gameObject.GetComponentInParent<PlayerCharacter>()))
            {
                playersDetected.Remove(other.gameObject.GetComponentInParent<PlayerCharacter>());
                playersInside--;

                if (playerInsideCount != null)
                {
                    playerInsideCount.gameObject.SetActive(true);
                    playerInsideCount.text = $"{playersInside}/{GameManager.Instance.CoopManager.GetComponentsInChildren<PlayerInputHandler>().Length}";

                    if (playersDetected.Count >= 0)
                    {
                        playerInsideCount.gameObject.SetActive(false);
                    }
                }

            }
        }
        if (other.gameObject.GetComponent<EnemyCharacter>())
        {
            if (enemiesDetected.Contains(other.gameObject.GetComponent<EnemyCharacter>()))
                enemiesDetected.Remove(other.gameObject.GetComponent<EnemyCharacter>());
        }
    }

    public List<PlayerCharacter> GetPlayersDetected()
    {
        return playersDetected;
    }

    public List<EnemyCharacter> GetEnemiesDetected()
    {
        return enemiesDetected;
    }

    public int GetPlayersCountInTrigger()
    {
        return playersInside;
    }

    public void ClearList()
    {
        playersDetected.Clear();
        enemiesDetected.Clear();
        playersInside = 0;
    }
}
