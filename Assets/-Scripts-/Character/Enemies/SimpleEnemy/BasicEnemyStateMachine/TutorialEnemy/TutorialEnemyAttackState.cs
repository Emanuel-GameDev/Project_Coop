using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyAttackState : BasicMeleeEnemyAttackState
{
    public TutorialEnemyAttackState(BasicEnemy basicEnemy) : base(basicEnemy)
    {
    }


    public override void Update()
    {

        if (!basicEnemy.isActioning)
        {
            if (basicEnemy.AttackRangeTrigger.GetPlayersCountInTrigger() <= 0)
            {

                foreach (PlayerCharacter player in basicEnemy.AttackRangeTrigger.GetPlayersDetected())
                {
                    if (player == basicEnemy.currentTarget)
                    {
                        //stateMachine.SetState(basicEnemy.actionState);
                        basicEnemy.StartCoroutine(basicEnemy.Attack());
                        return;
                    }
                }
            }

            stateMachine.SetState(basicEnemy.idleState);

        }
    }

}
