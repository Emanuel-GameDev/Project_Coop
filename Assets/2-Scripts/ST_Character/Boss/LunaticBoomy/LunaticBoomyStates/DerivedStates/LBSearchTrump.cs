using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

        if (!bossCharacter.gameObject.GetComponent<NavMeshAgent>().enabled)
        {
            bossCharacter.TriggerAgent(true);
            bossCharacter.Agent.isStopped = false;
            bossCharacter.Agent.ResetPath();
        }

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
    }

    public override void Update()
    {
        base.Update();

        if (bossCharacter.Agent.remainingDistance <= bossCharacter.Agent.stoppingDistance)
        {
            if (nearestTrump != null)
            {
                stateMachine.SetState(new LBJumpAttackV2(bossCharacter, nearestTrump));
            }
        }
    }
}
