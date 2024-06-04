using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightHandler : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> perfectExecutionSounds = new();
    [SerializeField]
    BossCharacter bossCharacter;
    public BossCharacter BossCharacter => bossCharacter;

    private void OnEnable()
    {
        StartCoroutine(Utility.WaitForPlayers(ActivateBossfight));
    }

    private void OnDisable()
    {
        DeactivateBossfight();
    }

    private void SetPlayersBossfight(bool active)
    {
        foreach (PlayerCharacter playerCharacter in PlayerCharacterPoolManager.Instance.AllPlayerCharacters)
        {
            playerCharacter.SetIsInBossfight(active, active ? this : null);
        }
    }

    public void ActivateBossfight()
    {
        SetPlayersBossfight(true);
    }

    public void DeactivateBossfight()
    {
        SetPlayersBossfight(false);
    }

    public AudioClip GetPerfectExecutionSound(int index)
    {
        if(perfectExecutionSounds.Count == 0)
            return null;
        
        if (perfectExecutionSounds.Count > index)
        {
            return perfectExecutionSounds[index];
        }
        else
        {
            return perfectExecutionSounds[perfectExecutionSounds.Count - 1];
        }
    }



}
