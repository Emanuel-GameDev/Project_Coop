using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] Character character;

    [SerializeField] CharacterData player1;
    [SerializeField] CharacterData player2;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            character.SetCharacterData(player1);
        }
    }
}
