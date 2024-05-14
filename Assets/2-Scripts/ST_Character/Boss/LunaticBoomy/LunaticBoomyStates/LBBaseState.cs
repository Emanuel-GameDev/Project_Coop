using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LBBaseState : State<LBBaseState>
{
    protected LunaticBoomyBossCharacter bossCharacter;

    protected LBBaseState(LunaticBoomyBossCharacter bossCharacter)
    {
        this.bossCharacter = bossCharacter;
    }

}
