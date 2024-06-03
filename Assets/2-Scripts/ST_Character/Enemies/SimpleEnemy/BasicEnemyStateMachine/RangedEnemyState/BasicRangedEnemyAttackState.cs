public class BasicRangedEnemyAttackState : BasicRangedEnemyState
{

    public BasicRangedEnemyAttackState(RangedEnemy basicEnemy) : base(basicEnemy)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //basicEnemy.StartCoroutine(basicEnemy.Attack());

        basicEnemy.canMove = false;
        basicEnemy.canAction = true;


        basicEnemy.ActivateObstacle();

    }

    public override void Update()
    {
        base.Update();


        if (!basicEnemy.isActioning)
        {
            if (rangedEnemy.EscapeTrigger.GetPlayersCountInTrigger() == 0 || rangedEnemy.panicAttack)
            {
                rangedEnemy.SetActionCoroutine(rangedEnemy.StartCoroutine(rangedEnemy.Attack()));
            }
            else if (rangedEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() == 0)
            {
                stateMachine.SetState(rangedEnemy.moveState);
            }
            else
            {
                stateMachine.SetState(rangedEnemy.escapeState);
            }


        }


    }

    public override void Exit()
    {
        base.Exit();
    }
}
