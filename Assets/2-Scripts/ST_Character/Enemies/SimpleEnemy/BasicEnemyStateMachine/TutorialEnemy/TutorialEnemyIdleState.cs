using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyIdleState : TutorialEnemyState
{
    public TutorialEnemyIdleState(TutorialEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        basicEnemy.canSee = true;
        basicEnemy.canMove = false;
        basicEnemy.canAction = false;

        basicEnemy.StartStopMovementCountdownCoroutine(false);

        tutorialEnemy.PlayIdleSound();
    }

    public override void Update()
    {
        if (tutorialEnemy.viewTrigger.GetPlayersCountInTrigger() > 0 && basicEnemy.canSee)
        {
            if (basicEnemy.target == null)
            {
                basicEnemy.SetTarget(basicEnemy.viewTrigger.GetPlayersDetected()[0].transform);
            }

            stateMachine.SetState(tutorialEnemy.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        tutorialEnemy.StopIdleSound();
    }

}
