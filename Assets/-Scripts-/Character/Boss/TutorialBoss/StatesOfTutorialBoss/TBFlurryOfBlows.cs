using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBFlurryOfBlows : TutorialBossState
{
    public TBFlurryOfBlows(TutorialBossCharacter bossCharacter) : base(bossCharacter)
    {
    }
    
    int punchCount;
    public override void Enter()
    {
        base.Enter();
        CalculateTargetPosition(bossCharacter.flurryDistance);
        StartAgent(bossCharacter.flurrySpeed);
        punchCount = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        float dist = Vector2.Distance(Utility.ZtoY(destination), Utility.ZtoY(bossCharacter.transform.position));
        if (mustStop || dist <= MinDistance)
        {
            bossCharacter.Agent.isStopped = true;
            punchCount++;
            if (punchCount >= 3)
            {
                stateMachine.SetState(new TBStart(bossCharacter));
                Debug.Log("Flurry of blows done");
            }
            else 
            {
                CalculateTargetPosition(bossCharacter.flurryDistance);
                StartAgent(bossCharacter.flurrySpeed);
            }
        }
    }

   

}
