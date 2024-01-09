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
        if(other.GetComponent<PlayerCharacter>())
        {
            characterList.Add(other.GetComponent<PlayerCharacter>());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerCharacter>())
        {
            other.GetComponent<PlayerCharacter>().protectedByTank = false;
            characterList.Remove(other.GetComponent<PlayerCharacter>());
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
