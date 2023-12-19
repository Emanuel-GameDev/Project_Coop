using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCharacter : Character
{
    Dummy characterClass;
    public void ChangeBehaviour()
    {
        Dummy dummy = characterClass;
        dummy.ChangeBehaviour();
    }
}
