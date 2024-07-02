using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyMovementState : TutorialEnemyState
{

    public TutorialEnemyMovementState(TutorialEnemy basicEnemy) : base(basicEnemy)
    {
    }
    public override void Enter()
    {
        base.Enter();
        tutorialEnemy.PlayWalkSound();

        tutorialEnemy.canMove = true;
        tutorialEnemy.canAction = false;

        tutorialEnemy.GetAnimator().SetBool("isMoving", true);
        tutorialEnemy.StartStopMovementCountdownCoroutine(true);
    }

    public override void Update()
    {
        base.Update();


        if (tutorialEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0)
        {
            foreach (PlayerCharacter player in tutorialEnemy.AttackRangeTrigger.GetPlayersDetected())
            {
                if (player == tutorialEnemy.currentTarget)
                    stateMachine.SetState(tutorialEnemy.actionState);
            }

        }
        tutorialEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();

        tutorialEnemy.GetAnimator().SetBool("isMoving", false);
        tutorialEnemy.StopWalkSound();
    }
   

}
