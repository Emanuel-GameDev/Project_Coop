using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlurryOfBlows : TutorialBossState
{
    public FlurryOfBlows(TutorialBossCharacter bossCharacter) : base(bossCharacter)
    {
    }
    Vector3 destination;
    bool mustStop;
    float minDistance = 0.1f;
    int punchCount;
    public override void Enter()
    {
        base.Enter();
        bossCharacter.TargetSelection();
        CalculateTargetPosition();
        StartAgent();
        punchCount = 0;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        float dist = Vector3.Distance(destination, bossCharacter.transform.position);
        if (mustStop || dist <= minDistance)
        {
            bossCharacter.Agent.isStopped = true;
            punchCount++;
            if (punchCount >= 3)
            {
                stateMachine.SetState(new FlurryOfBlows(bossCharacter));
            }
            else 
            {
                CalculateTargetPosition();
                StartAgent();
            }
        }
    }

    void CalculateTargetPosition()
    {
        Vector2 direction = (Utility.ZtoY(bossCharacter.target.position) - Utility.ZtoY(bossCharacter.transform.position)).normalized;
        destination = Utility.YtoZ(Utility.ZtoY(bossCharacter.transform.position) + direction * bossCharacter.flurryDistance);
    }

    void StartAgent()
    {
        bossCharacter.Agent.isStopped = false;
        bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
        bossCharacter.Agent.SetDestination(destination);
    }

}
