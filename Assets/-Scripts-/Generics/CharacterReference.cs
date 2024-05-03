using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterReference : MonoBehaviour
{
    [SerializeField] private ePlayerCharacter character;
    public ePlayerCharacter Character => character;
}
