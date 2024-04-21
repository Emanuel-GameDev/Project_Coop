using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LBJumpAttackV2 : LBBaseState
{
    private TrumpOline currTrump;
    private TrumpOline nextTrump;

    // Jump Related
    private Vector2 currPos;
    private float landingDis;
    private bool canJump = false;
    private bool onTrump = true;
    private float timeElapsed;
    private Keyframe lastKeyframe;

    // Attack Related
    private List<PlayerCharacter> targetsHit;
    private bool canAttack = true;

    // General
    private LBPhase activePhase;
    private int jumpCount = 0;
    private int maxJumps;

    public LBJumpAttackV2(LunaticBoomyBossCharacter bossCharacter, TrumpOline firstTrump) : base(bossCharacter)
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

        activePhase = bossCharacter.GetActivePhase();
        maxJumps = activePhase.numJumps;

        targetsHit = new List<PlayerCharacter>();

        lastKeyframe = bossCharacter.CurveY[bossCharacter.CurveY.length - 1];

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
            HandleJump();

            HandleAttack();

            if (timeElapsed >= lastKeyframe.time)
                CheckJumpOver();
        }
    }

    #region Jump

    private void HandleJump()
    {
        if (onTrump)
        {
            currPos = bossCharacter.GetRigidBody().position;
            landingDis = Vector2.Distance(currPos, nextTrump.gameObject.transform.position);
            timeElapsed = 0f;
            onTrump = false;
        }
        else
        {
            timeElapsed += Time.deltaTime * activePhase.jumpSpeed / landingDis;
            if (timeElapsed <= lastKeyframe.time)
            {
                currPos = Vector2.MoveTowards(currPos, nextTrump.gameObject.transform.position, Time.deltaTime * activePhase.jumpSpeed);
                bossCharacter.GetRigidBody().MovePosition(new Vector2(currPos.x, currPos.y + bossCharacter.CurveY.Evaluate(timeElapsed)));
            }
        }
    }

    private void CheckJumpOver()
    {
        if (nextTrump.destroyed)
        {
            stateMachine.SetState(new LBExplosion(bossCharacter, nextTrump));
            return;
        }

        // Aggiorno la destinazione del salto
        currTrump = nextTrump;
        onTrump = true;

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
    }

    #endregion

    #region JumpAttack

    private void HandleAttack()
    {
        if (timeElapsed >= (lastKeyframe.time / 2))
        {
            // Metà salto
            if (canAttack)
            {
                targetsHit.Clear();
                canAttack = false;

                // Ho scritto 3-1 per evitare fraintendimenti poiché jumpCount viene incrementatao dopo questo
                // controllo, avrei potuto scrivere tranquillamente 2
                if (activePhase.phaseNum == 3 && jumpCount == (3 - 1))
                {
                    // Attacco Terza fase
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
                }
                else
                {
                    Debug.Log("Attacco normale");
                    Attack();
                }
            }
        }
    }
    
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

            projectile.GetComponent<LBProjectile>().Initialize(bossCharacter);

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
