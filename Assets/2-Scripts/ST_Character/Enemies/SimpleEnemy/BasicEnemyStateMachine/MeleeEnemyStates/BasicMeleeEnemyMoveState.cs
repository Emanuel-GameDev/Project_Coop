using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyMoveState : BasicMeleeEnemyState
{
    public BasicMeleeEnemyMoveState(BasicMeleeEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {

        meleeEnemy.canMove = true;
        meleeEnemy.canAction = false;

        meleeEnemy.GetAnimator().SetBool("isMoving", true);

    }

    public override void Update()
    {
        base.Update();
        
        if (meleeEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0)
        {
            meleeEnemy.SetTarget(meleeEnemy.AttackRangeTrigger.GetPlayersDetected()[0].transform);
            stateMachine.SetState(meleeEnemy.actionState);
        }
        meleeEnemy.FollowPath();
    }

    public override void Exit()
    {
        base.Exit();

        meleeEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
