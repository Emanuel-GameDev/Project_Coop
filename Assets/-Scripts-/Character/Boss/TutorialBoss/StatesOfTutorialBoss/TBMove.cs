using System;
using System.Collections.Generic;
using UnityEngine;

public class TBMove : TutorialBossState
{
    public TBMove(TutorialBossCharacter bossCharacter) : base(bossCharacter)
    {
    }
    float duration;
    public override void Enter()
    {
        destination = bossCharacter.target.position;
        StartAgent(bossCharacter.MoveSpeed);
        duration = bossCharacter.moveDuration;
    }

    public override void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            stateMachine.SetState(new TBCharge(bossCharacter));
        }
        else
        {
            CheckPlayersDistances();
        }

        bossCharacter.Agent.SetDestination(bossCharacter.target.transform.position);
    }

    private void CheckPlayersDistances()
    {
        List<PlayerCharacter> activePlayers = GameManager.Instance.coopManager.activePlayers;
        foreach (PlayerCharacter player in activePlayers)
        {
            bool isNear = Utility.DistanceV3toV2(player.transform.position, bossCharacter.transform.position) < bossCharacter.shortDistanceRange;
            if (isNear)
            {
                bossCharacter.target = player.transform;
                stateMachine.SetState(bossCharacter.NearMoveSelection());
            }
        }
    }
}

