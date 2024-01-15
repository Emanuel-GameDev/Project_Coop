using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtectPlayers : MonoBehaviour
{
    public List<PlayerCharacter> characterList = new List<PlayerCharacter>();
    private BoxCollider coll;

    private void OnEnable()
    {
        coll = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerCharacter>(out var playerToProtect))
        {
            characterList.Add(playerToProtect);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out var playerToProtect))
        {
            playerToProtect.protectedByTank = false;
            characterList.Remove(playerToProtect);
        }
    }

    public void SetPlayersProtected(bool variable)
    {
        foreach(PlayerCharacter character in characterList)
        {
            character.protectedByTank = variable;
        }
    }

}
