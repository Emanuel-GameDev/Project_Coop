using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBPanic : LBBaseState
{
    private TrumpOline startTrump;
    private List<TrumpOline> trumpRoute;
    private List<TrumpOline> trumps;

    private int currPathID;
    private int startPointID;
    private bool fullRouteActive = false;

    public LBPanic(LunaticBoomyBossCharacter bossCharacter, TrumpOline startTrump) : base(bossCharacter)
    {
        this.startTrump = startTrump;
    }

    public override void Enter()
    {
        base.Enter();

        bossCharacter.TriggerAgent(true);
        bossCharacter.Agent.isStopped = false;
        bossCharacter.Agent.ResetPath();

        // Prende reference alla route che dovrà percorrere il boss
        trumpRoute = bossCharacter.GetPanicRoute(startTrump);

        // Prendo reference alla lista totale per il percorso verde
        trumps = bossCharacter.GetTrumps();

        if (trumpRoute == null)
            Debug.LogError("Error: no panic route found");

        currPathID = trumpRoute.FindIndex(x => x == startTrump);
        startPointID = currPathID;

        // Set sgent speed
        bossCharacter.Agent.speed = bossCharacter.PanicSpeed;

        SetDestinationToNextPoint(trumpRoute);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (bossCharacter.Agent.remainingDistance <= 1f && !bossCharacter.Agent.pathPending)
        {
            if (!fullRouteActive)
            {
                if (currPathID == startPointID)
                {
                    if (!trumpRoute[currPathID].destroyed)
                    {
                        stateMachine.SetState(new LBSearchTrump(bossCharacter));

                        return;
                    }
                    else
                    {
                        currPathID = trumps.FindIndex(x => x == trumpRoute[currPathID]);
                        fullRouteActive = true;

                        return;
                    }
                }

                SetDestinationToNextPoint(trumpRoute);
            }
            else
            {
                if (!trumps[currPathID].destroyed)
                {
                    stateMachine.SetState(new LBSearchTrump(bossCharacter));

                    return;
                }

                SetDestinationToNextPoint(trumps);
            }
        }
    }

    private void SetDestinationToNextPoint(List<TrumpOline> path)
    {
        // Aggiorna ID
        currPathID = (currPathID + 1) % path.Count;

        // Imposta destinazione
        bossCharacter.Agent.SetDestination(path[currPathID].gameObject.transform.position);
    }
}
