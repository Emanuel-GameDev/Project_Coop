using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Detector : MonoBehaviour
{
    public UnityEvent OnAllPlayerInside;

    List<PlayerCharacter> playersDetected;
    int playersInside;

    private void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
        playersDetected = new List<PlayerCharacter>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter character))
        {
            playersDetected.Add(character);
            playersInside++;

            if (playersDetected.Count >= CoopManager.Instance.GetActiveHandlers().Count)
                OnAllPlayerInside?.Invoke();
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
