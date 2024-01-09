using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialBossState : State<TutorialBossState>
{
    protected TutorialBossCharacter bossCharacter;

    protected TutorialBossState(TutorialBossCharacter bossCharacter)
    {
        this.bossCharacter = bossCharacter;
    }
}
