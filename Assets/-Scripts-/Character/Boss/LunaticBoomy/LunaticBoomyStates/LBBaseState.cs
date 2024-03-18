using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LBBaseState : State<LBBaseState>
{
    protected LunaticBoomy bossCharacter;

    protected LBBaseState(LunaticBoomy bossCharacter)
    {
        this.bossCharacter = bossCharacter;
    }

}
