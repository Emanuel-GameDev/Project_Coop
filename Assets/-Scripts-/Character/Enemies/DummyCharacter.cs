using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCharacter : Character
{
    public void ChangeBehaviour()
    {
        Dummy dummy = (Dummy)CharacterClass;
        dummy.ChangeBehaviour();
    }
}
