using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBCharge : TutorialBossState
{
    public TBCharge(TBCharacterSM bossCharacter) : base(bossCharacter)
    {
    }

    public override void Enter()
    {
        base.Enter();
        CalculateTargetPosition(bossCharacter.chargeDistance);
        StartAgent(bossCharacter.chargeSpeed);
    }

    public override void Update()
    {
        float dist = Vector2.Distance(Utility.ZtoY(destination), Utility.ZtoY(bossCharacter.transform.position));
        if (mustStop || dist <= MinDistance)
        {
            bossCharacter.ChangeState();
        }
    }
}
