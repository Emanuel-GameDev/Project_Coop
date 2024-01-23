using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Detector : MonoBehaviour
{
    List<PlayerCharacter> playersDetected;

    private void Awake()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        playersDetected = new List<PlayerCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter character))
        {
            playersDetected.Add(character);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerCharacter>(out PlayerCharacter character))
        {
            if(playersDetected.Contains(character))
            {
                playersDetected.Remove(character);
            }
        }
    }

    public List<PlayerCharacter> GetPlayersDetected()
    {
        return playersDetected;
    }
}
