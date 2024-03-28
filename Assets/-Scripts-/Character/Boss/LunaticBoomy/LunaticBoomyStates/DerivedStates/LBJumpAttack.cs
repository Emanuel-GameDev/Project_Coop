using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LBJumpAttack : LBBaseState
{
    // Bezier Related
    private TrumpOline currTrump;
    private TrumpOline nextTrump;
    private Vector2 controlPoint;

    // Attack related
    private List<PlayerCharacter> targetsHit;

    // Others
    private LBPhase activePhase = null;

    private bool canJump = false;
    private bool canAttack = true;
    private float startTime = 0f;
    private int jumpCount = 0;
    private int maxJumps;

    public LBJumpAttack(LunaticBoomyBossCharacter bossCharacter, TrumpOline firstTrump) : base(bossCharacter)
    {
        currTrump = firstTrump;
    }

    public override void Enter()
    {
        base.Enter();

        // Spengo agent
        if (bossCharacter.gameObject.GetComponent<NavMeshAgent>().enabled)
        {
            bossCharacter.Agent.isStopped = true;
            bossCharacter.Agent.ResetPath();
            bossCharacter.TriggerAgent(false);
        }

        // Imposto speed del salto in base alla fase
        activePhase = bossCharacter.GetActivePhase();
        maxJumps = activePhase.numJumps;

        targetsHit = new List<PlayerCharacter>();

        StartJump();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (canJump)
        {
            // Calcolo la distanza percorsa dall'inizio del movimento
            float distCovered = (Time.time - startTime) * activePhase.jumpSpeed;

            // Calcolo la percentuale completata del movimento
            float fracJourney = distCovered / Vector2.Distance(currTrump.gameObject.transform.position, nextTrump.gameObject.transform.position);

            // Utilizzo la curva di Bezier per ottenere la posizione intermedia
            bossCharacter.gameObject.transform.position = CalculateBezierPoint(fracJourney, currTrump.gameObject.transform.position,
                                                                                            controlPoint,
                                                                                            nextTrump.gameObject.transform.position);

            // =========== ATTACCO ===========
            if (fracJourney >= 0.5f)
            {
                if (canAttack)
                {
                    targetsHit.Clear();
                    canAttack = false;

                    // Ho scritto 3-1 per evitare fraintendimenti poiché jumpCount viene incrementatao dopo questo
                    // controllo, avrei potuto scrivere tranquillamente 2
                    if (activePhase.phaseNum == 3 && jumpCount == (3 - 1))
                    {
                        if (PlayerCharacterPoolManager.Instance.ActivePlayerCharacters.Count > 1)
                        {
                            Attack();
                            Attack();
                        }
                        else
                        {
                            RandomShoot();
                            Attack();
                        }

                        Debug.Log("Attacco 3à fase");
                        Attack();
                        Attack();
                    }
                    else
                    {
                        Debug.Log("Attacco normale");
                        Attack();
                    }

                }
            }

            // =========== SALTO ===========
            if (fracJourney >= 1.0f)
            {
                // Temporaneo, controllo se il trampolino dove sono atterrato è distrutto
                if (nextTrump.destroyed)
                {
                    stateMachine.SetState(new LBPanic(bossCharacter, nextTrump));
                    return;
                }

                // Aggiorno la destinazione del salto
                currTrump = nextTrump;

                // Controllo la quantità dei salti in base alla fase corrente
                jumpCount++;
                if (jumpCount == maxJumps)
                {
                    stateMachine.SetState(new LBCarrotBreak(bossCharacter, currTrump));
                }

                // Reimposto il canAttack così può sparare quando arriva metà del prossimo salto
                canAttack = true;

                // Avvio l'attesa tra un salto e l'altro
                if (bossCharacter.ActivateJumpStep)
                    currTrump.StartCoroutine(WaitBeforeJump(bossCharacter.JumpStep));
            }
        }
    }

    #region Jump

    private void StartJump()
    {
        // Prendo reference al punto di arrivo, mi assicuro che il trampolino non sia distrutto
        bool trumpAccepted = false;

        do
        {
            nextTrump = GetRandomTrump(bossCharacter.GetTrumps());

            if (nextTrump.destroyed)
            {
                float randomValue = UnityEngine.Random.value;
                bool canJumpOnDestroyed = randomValue < bossCharacter.DestroyedJumpProb;

                if (canJumpOnDestroyed)
                    trumpAccepted = true;
                else
                    trumpAccepted = false;
            }
            else
                trumpAccepted = true;

        } while (!trumpAccepted);

        canJump = true;
        startTime = Time.time;

        // Calcolo del control point dinamico
        controlPoint = CalculateControlPoint();
    }

    private IEnumerator WaitBeforeJump(float waitTime)
    {
        canJump = false;
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        yield return new WaitForSeconds(waitTime);

        bossCharacter.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        StartJump();
    }

    #region Bezier


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

    #endregion

    #endregion

    // DA TESTARE

    #region JumpAttack

    private void Attack()
    {
        GameObject projectile = bossCharacter.GetPooledProjectile();

        // Prendo un personaggio random, se non è già stato colpito
        PlayerCharacter randCharacter = null;

        do
        {
            randCharacter = bossCharacter.GetRandomPlayer();

        } while (targetsHit.Contains(randCharacter));

        if (projectile != null)
        {
            // Set pos del proiettile
            projectile.transform.position = bossCharacter.gameObject.transform.position;

            // La direzione di sparo
            Vector2 direction = (randCharacter.gameObject.transform.position - bossCharacter.gameObject.transform.position).normalized;

            projectile.SetActive(true);

            // Set velocità sul proiettile
            projectile.GetComponent<Rigidbody2D>().velocity = direction * bossCharacter.ProjectileSpeed;
        }
        else
            Debug.LogError("Error: projectile is null");

        targetsHit.Add(randCharacter);
    }

    private void RandomShoot()
    {
        // Devo prendere una posizione random nell'arena

        Vector2 randPoint = bossCharacter.GetRandomPointArena();

        // Creo il proiettile

        GameObject projectile = bossCharacter.GetPooledProjectile();

        if (projectile != null)
        {
            // Set pos del proiettile
            projectile.transform.position = bossCharacter.gameObject.transform.position;

            // Initializee set a true
            projectile.GetComponent<LBProjectile>().Initialize(bossCharacter);
            projectile.SetActive(true);

            // Avvia la coroutine per muovere il proiettile alla posizione casuale
            bossCharacter.StartCoroutine(MoveProjectile(projectile, randPoint));

        }
        else
            Debug.LogError("Error: projectile is null");
    }

    private IEnumerator MoveProjectile(GameObject projectile, Vector2 targetPosition)
    {
        Vector2 startPos = projectile.transform.position;

        float distance = Vector2.Distance(startPos, targetPosition);
        float duration = distance / bossCharacter.ProjectileSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < 1)
        {
            elapsedTime += Time.deltaTime / duration;
            projectile.transform.position = Vector2.Lerp(startPos, targetPosition, elapsedTime);

            yield return null;
        }
    }

    #endregion

    #region Utility

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


    #endregion

}
