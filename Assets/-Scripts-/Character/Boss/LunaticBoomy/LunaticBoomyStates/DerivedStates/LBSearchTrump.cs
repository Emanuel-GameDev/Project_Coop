using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBSearchTrump : LBBaseState
{
    // Lo stato che si occupa di cercare il primo trampolino libero
    TrumpOline nearestTrump = null;
    public LBSearchTrump(LunaticBoomyBossCharacter bossCharacter) : base(bossCharacter)
    {
    }

    public override void Enter()
    {
        base.Enter();

        SearchTrump(bossCharacter.GetTrumps());
    }

    private void SearchTrump(List<TrumpOline> trumps)
    {
        float minDist = Mathf.Infinity;

        foreach (TrumpOline trump in trumps)
        {
            float dist = Vector2.Distance(bossCharacter.transform.position, trump.gameObject.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                nearestTrump = trump;
            }
        }

        if (nearestTrump != null)
        {
            bossCharacter.Agent.SetDestination(nearestTrump.transform.position);
        }
        else
        {
            Debug.LogError("NO Trump was found");
        }
    }

    public override void Exit()
    {
        base.Exit();

        nearestTrump = null;

        bossCharacter.Agent.isStopped = true;
        bossCharacter.Agent.ResetPath();
        bossCharacter.TriggerAgent(false);
    }

    public override void Update()
    {
        base.Update();

        if (bossCharacter.Agent.remainingDistance <= bossCharacter.Agent.stoppingDistance)
        {
            if (nearestTrump != null)
            {
                stateMachine.SetState(new LBJumpAttack(bossCharacter, nearestTrump));
            }
        }
    }
}
