using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyMovementState : BasicMeleeEnemyMoveState
{

    public TutorialEnemyMovementState(TutorialEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Update()
    {

        if (basicEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0 )
        {
            foreach (PlayerCharacter player in basicEnemy.AttackRangeTrigger.GetPlayersDetected())
            {
                //if(player == basicEnemy.currentTarget)
                   // stateMachine.SetState(basicEnemy.actionState);
            }

        }
        basicEnemy.FollowPath();
    }



}
