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

        base.Update();

       

        if (meleeEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0 )
        {
            foreach (PlayerCharacter player in meleeEnemy.AttackRangeTrigger.GetPlayersDetected())
            {
                if (player == meleeEnemy.currentTarget)
                    stateMachine.SetState(meleeEnemy.actionState);
            }

        }
        meleeEnemy.FollowPath();
    }



}
