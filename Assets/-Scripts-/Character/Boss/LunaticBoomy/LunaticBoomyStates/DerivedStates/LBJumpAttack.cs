using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBJumpAttack : LBBaseState
{
    // Bezier Related
    private TrumpOline currTrump;
    private TrumpOline nextTrump;
    private Vector2 controlPoint;

    private bool canJump = false;
    private bool canAttack = true;
    private float startTime = 0f;
    private int jumpCount = 0;

    public LBJumpAttack(LunaticBoomyBossCharacter bossCharacter, TrumpOline firstTrump) : base(bossCharacter)
    {
        currTrump = firstTrump;
    }

    public override void Enter()
    {
        base.Enter();

        // Azzero la gravity scale per evitare spasmi sul trampolino
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;

        StartJump();
    }

    private void StartJump()
    {
        // Prendo reference al punto di arrivo
        nextTrump = GetRandomTrump(bossCharacter.GetTrumps());

        canJump = true;
        startTime = Time.time;

        // Calcolo del control point dinamico
        controlPoint = CalculateControlPoint();
    }

    private TrumpOline GetRandomTrump(List<TrumpOline> trumpOlines)
    {
        int randTrumpID = UnityEngine.Random.Range(0, trumpOlines.Count);
        List<TrumpOline> tempTrumps = new List<TrumpOline>(trumpOlines);

        while (tempTrumps[randTrumpID] == currTrump)
        {
            tempTrumps.RemoveAt(randTrumpID);
            randTrumpID = UnityEngine.Random.Range(0, tempTrumps.Count);
        }

        return tempTrumps[randTrumpID];
    }

    public override void Exit()
    {
        base.Exit();

        bossCharacter.gameObject.GetComponent<Rigidbody2D>().gravityScale = 1f;
    }

    public override void Update()
    {
        base.Update();

        if (canJump)
        {
            // Calcoliamo la distanza percorsa dall'inizio del movimento
            float distCovered = (Time.time - startTime) * bossCharacter.JumpSpeed;

            // Calcoliamo la percentuale completata del movimento
            float fracJourney = distCovered / Vector2.Distance(currTrump.gameObject.transform.position, nextTrump.gameObject.transform.position);

            // Utilizziamo la curva di Bezier per ottenere la posizione intermedia
            bossCharacter.gameObject.transform.position = CalculateBezierPoint(fracJourney, currTrump.gameObject.transform.position, 
                                                                                            controlPoint,
                                                                                            nextTrump.gameObject.transform.position);

            if (fracJourney >= 0.5f)
            {
                if (canAttack)
                {
                    // Debug.Log("Attacco);
                    nextTrump.StartCoroutine(JumpAttack(1f));
                }
            }
            
            if (fracJourney >= 1.0f)
            {
                //Debug.Log("Arrivato");
                currTrump = nextTrump;

                jumpCount++;
                CheckPhase();

                currTrump.StartCoroutine(WaitBeforeJump(bossCharacter.TimeBetweenJumps));
            }
        }
    }

    private void CheckPhase()
    {
        LBPhase activePhase = null;

        foreach (LBPhase phase in bossCharacter.BossPhases)
        {
            if (phase.active)
            {
                activePhase = phase;
                break;
            }
        }

        if (jumpCount == activePhase.numJumps)
            stateMachine.SetState(new LBCarrotBreak(bossCharacter, currTrump));
    }

    private IEnumerator JumpAttack(float temp)
    {
        canAttack = false;

        yield return new WaitForSeconds(temp);
        
        canAttack = true;
    }

    //private void JumpAttack()
    //{
    //    // Reference ad un personaggio casuale
    //    int randPlayerID = UnityEngine.Random.Range(0, CoopManager.Instance.ActivePlayers.Count);

    //    canAttack = false;
    //}

    private IEnumerator WaitBeforeJump(float waitTime)
    {
        canJump = false;

        yield return new WaitForSeconds(waitTime);

        StartJump();
    }

    private Vector2 CalculateControlPoint()
    {
        // Trovo il punto intermedio tra partenza e arrivo
        Vector2 center = (currTrump.gameObject.transform.position + nextTrump.gameObject.transform.position) / 2f;

        // Trovo l'angolo tra partenza e arrivo
        float angle = Mathf.Atan2(nextTrump.gameObject.transform.position.y - currTrump.gameObject.transform.position.y,
                                    nextTrump.gameObject.transform.position.x - currTrump.gameObject.transform.position.x) * Mathf.Rad2Deg;

        // Ruoto l'offset in base all'angolo
        Vector2 rotatedOffset = Quaternion.Euler(0f, 0f, angle) * bossCharacter.JumpOffset;

        return center + rotatedOffset;
    }

    /*
    Una curva di Bézier è definita da un insieme di punti di controllo che determinano la forma e il percorso della curva stessa. 
    Questi punti di controllo influenzano la direzione e l'ampiezza della curva. 
    La forma esatta della curva dipende dall'interpolazione dei punti di controllo. 
    Le curve di Bézier possono essere di diversi ordini, a seconda del numero di punti di controllo utilizzati.
        - Curve di Bézier quadratiche: Queste curve sono definite da tre punti di controllo: un punto di partenza, un punto di controllo e un punto di arrivo. 
          La curva segue la traiettoria definita da questi tre punti.
        - Curve di Bézier cubiche: Queste curve sono definite da quattro punti di controllo: un punto di partenza, due punti intermedi di controllo e un punto di arrivo. 
          La curva segue una traiettoria più flessibile rispetto alle curve quadratiche, poiché i punti intermedi di controllo permettono di definire curve più complesse.
    */

    Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector2 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }
}
