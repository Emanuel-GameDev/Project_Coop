using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBSlam : TutorialBossState
{
    public TBSlam(TBCharacterSM bossCharacter) : base(bossCharacter)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        bossCharacter.ChangeState();
    }
}
