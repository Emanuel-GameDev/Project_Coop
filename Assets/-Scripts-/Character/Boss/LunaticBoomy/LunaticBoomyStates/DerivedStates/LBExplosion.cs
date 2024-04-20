using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBExplosion : LBBaseState
{
    private TrumpOline currTrump;

    private float timer;
    public LBExplosion(LunaticBoomyBossCharacter bossCharacter, TrumpOline currTrump) : base(bossCharacter)
    {
        this.currTrump = currTrump;
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("BOOOOOOOOOOOOM");

        timer = 0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        timer += Time.deltaTime;

        if (timer >= bossCharacter.StunTime)
        {
            Debug.Log("stun finito");
            stateMachine.SetState(new LBPanic(bossCharacter, currTrump));
        }
    }
}
