using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : InputReceiver
{
    public PlayerCharacter ActualCharacter => actualCharacter;
    private PlayerCharacter actualCharacter;
}
