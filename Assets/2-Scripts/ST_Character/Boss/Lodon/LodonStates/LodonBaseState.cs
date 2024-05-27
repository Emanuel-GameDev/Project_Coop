using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LodonBaseState : State<LodonBaseState>
{
    protected LodonBoss lodonBossCharacter;

    public LodonBaseState(LodonBoss lodonBossCharacter)
    {
        this.lodonBossCharacter = lodonBossCharacter;
    }
}