using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlurryOfBlows : TutorialBossState
{
    public FlurryOfBlows(TutorialBossCharacter bossCharacter) : base(bossCharacter)
    {
    }

    public override void Enter()
    {
        base.Enter();
        bossCharacter.TargetSelection();
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
