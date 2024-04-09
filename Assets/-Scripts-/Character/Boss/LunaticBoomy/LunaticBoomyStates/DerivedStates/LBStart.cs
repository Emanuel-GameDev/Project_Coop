using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBStart : LBBaseState
{
    public LBStart(LunaticBoomyBossCharacter bossCharacter) : base(bossCharacter)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateMachine.SetState(new LBSearchTrump(bossCharacter));
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }
}
