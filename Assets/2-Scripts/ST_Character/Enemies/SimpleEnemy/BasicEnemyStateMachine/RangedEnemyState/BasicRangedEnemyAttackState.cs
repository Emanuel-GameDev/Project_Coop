

using UnityEngine;

public class BasicRangedEnemyAttackState : BasicRangedEnemyState
{

    public BasicRangedEnemyAttackState(RangedEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //basicEnemy.StartCoroutine(basicEnemy.Attack());

        rangedEnemy.canMove = false;
        rangedEnemy.canAction = true;

        rangedEnemy.ResetVelocity();


    }

    public override void Update()
    {
        base.Update();

        rangedEnemy.SetSpriteDirection(rangedEnemy.target.position-rangedEnemy.transform.position);

        if (!basicEnemy.isActioning)
        {
            if(rangedEnemy.EscapeTrigger.GetPlayersCountInTrigger() > 0 && !rangedEnemy.panicAttack)
            {
                stateMachine.SetState(rangedEnemy.escapeState);
            }
            else if (rangedEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() > 0 || rangedEnemy.panicAttack)
            {
                rangedEnemy.SetActionCoroutine(rangedEnemy.StartCoroutine(rangedEnemy.Attack()));
            }
            else if (rangedEnemy.currentTarget != null)
            {
                stateMachine.SetState(rangedEnemy.moveState);
            }
            else
            {
                stateMachine.SetState(rangedEnemy.idleState);
            }


        }


    }

    public override void Exit()
    {
        base.Exit();
    }
}
