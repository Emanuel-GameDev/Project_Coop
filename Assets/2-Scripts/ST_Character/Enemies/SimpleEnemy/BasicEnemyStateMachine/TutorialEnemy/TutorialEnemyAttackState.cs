using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyAttackState : BasicMeleeEnemyAttackState
{

    public TutorialEnemyAttackState(TutorialEnemy basicEnemy) : base(basicEnemy)
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
                        stateMachine.SetState(meleeEnemy.actionState);
                        basicEnemy.StartCoroutine(meleeEnemy.Attack());
                        return;
                    }
                }
            }

            stateMachine.SetState(meleeEnemy.moveState);
        }
    }

}
