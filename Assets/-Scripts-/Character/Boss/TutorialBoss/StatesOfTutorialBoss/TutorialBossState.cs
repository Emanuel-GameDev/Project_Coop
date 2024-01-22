using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialBossState : State<TutorialBossState>
{
    protected TBCharacterSM bossCharacter;
    protected bool mustStop;
    protected float MinDistance => bossCharacter.minDistance;
    protected Vector3 destination;

    protected TutorialBossState(TBCharacterSM bossCharacter)
    {
        this.bossCharacter = bossCharacter;
    }

    protected void CalculateTargetPosition(float distance)
    {
        Vector2 direction = (Utility.ZtoY(bossCharacter.target.position) - Utility.ZtoY(bossCharacter.transform.position)).normalized;
        destination = Utility.YtoZ(Utility.ZtoY(bossCharacter.transform.position) + direction * distance);
    }

    protected void StartAgent(float speed)
    {
        bossCharacter.Agent.isStopped = false;
        bossCharacter.Agent.speed = speed;
        bossCharacter.Agent.SetDestination(destination);
    }

    public override void Exit()
    {
        bossCharacter.Agent.isStopped = true;
    }
}
