using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[Serializable]
public class LBPhase
{
    public int phaseNum;
    public int minJumps;
    public int maxJumps;
    public float jumpSpeed;
    
    public bool active = false;

    [HideInInspector]
    public int numJumps => UnityEngine.Random.Range(minJumps, maxJumps + 1);
}

public class LunaticBoomyBossCharacter : BossCharacter
{
    StateMachine<LBBaseState> stateMachine = new();

    #region Variables

    [Space]
    [Title("GENERAL")]  
    [Space]

    [SerializeField]
    private List<LBPhase> bossPhases;
    public List<LBPhase> BossPhases => bossPhases;


    [SerializeField]
    private List<TrumpOline> trumps;

    [Space]
    [Title("ATTACK")]
    [Space]

    [SerializeField]
    private GameObject projectilePrefab;

    [SerializeField]
    private int poolSize = 3;

    [SerializeField]
    private float bombSpeed = 10f;
    public float BombSpeed => bombSpeed;


    private GameObject[] projectilePool;
    public GameObject[] ProjectilePool => projectilePool;

    [Space]
    [Title("JUMP")]

    [SerializeField, Tooltip("Di quanto deve essere spostato il centro del salto da un trampolino all'altro")]
    private Vector2 jumpOffset;
    public Vector2 JumpOffset => jumpOffset;

    [Space]

    [SerializeField, Min(0.04f), Tooltip("un tempo di attesa tra un salto e l'altro")]
    private float jumpStep = 1.5f;
    public float JumpStep => jumpStep;

    [SerializeField, Tooltip("Attiva o disattiva il jump step, se è disattivato significa che tra un salto e l'altro non c'è attesa")]
    private bool activateJumpStep = true;
    public bool ActivateJumpStep => activateJumpStep;

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterJump = 1f;
    public float TimeAfterJump => timeAfterJump;

    [Space]
    [Title("CARROT_BREAK")]
    [Space]

    [SerializeField, Tooltip("Durata della pausa carota")]
    private float breakTime;
    public float BreakTime => breakTime;

    [SerializeField, Tooltip("Potenza del rimbalzo")]
    private float bounceForce = 5f;
    public float BounceForce => bounceForce;

    [SerializeField, Tooltip("la forza effettiva del rimbalzo sarà un random compreso tra: \nbounceForce - randomBounceRange \nbounceForce + randomBounceRange")]
    private float randBounceRange = 3f;
    public float RandBounceRange => randBounceRange;

    [Space]
    [Title("PANIC")]
    [Space]

    [SerializeField]
    private List<TrumpOline> route1;
    public List<TrumpOline> Route1 => route1;

    [SerializeField]
    private List<TrumpOline> route2;
    public List<TrumpOline> Route2 => route2;

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterPanic = 1f;
    public float TimeAfterPanic => timeAfterPanic;

    #endregion

    private void Start()
    {
        Agent.updateRotation = false;

        InitializePhases();
        InitializePool();

        stateMachine.SetState(new LBStart(this));
    }

    private void InitializePhases()
    {
        // Magar la funz serve la questo for è sbagliato
        //for (int i = 0; i < bossPhases.Count; i++)
        //{
        //    int rand = UnityEngine.Random.Range(bossPhases[i].minJumps, bossPhases[i].maxJumps + 1);
        //    bossPhases[i].numJumps = rand;
        //}
    }

    #region Projectiles

    private void InitializePool()
    {
        projectilePool = new GameObject[poolSize];

        for (int i = 0; i < projectilePool.Length; i++)
        {
            projectilePool[i] = Instantiate(projectilePrefab,
                                                   gameObject.transform.position, Quaternion.identity);
            projectilePool[i].transform.SetParent(transform.GetChild(0));
            projectilePool[i].SetActive(false);
        }
    }

    internal GameObject GetPooledProjectile()
    {
        for (int i = 0; i < projectilePool.Length; i++)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }
        return null;
    }

    #endregion

    private void Update()
    {
        stateMachine.StateUpdate();
    }

    public List<TrumpOline> GetTrumps()
    {
        if (trumps.Count <= 0)
        {
            Debug.LogError("Error in TrumpOline List");
            return null;    
        }

        return trumps;
    }

    public void TriggerAgent(bool mode)
    {
        if (mode)
            GetComponent<NavMeshAgent>().enabled = true;
        else
            GetComponent<NavMeshAgent>().enabled = false;
    }

    public List<TrumpOline> FindPanicRoute(TrumpOline trump)
    {
        for (int i = 0; i < route1.Count; i++)
        {
            if (trump == route1[i])
                return route1;
            else if (trump == route2[i])
                return route2;
        }

        return null;
    }

    public void DoubleAttack()
    {
        // Spara 2 bombe a 2 pg a caso, non può essere lo stesso a venir targettato 2 volte
    }

    public LBPhase GetActivePhase()
    {
        foreach (LBPhase phase in bossPhases)
        {
            if (phase.active)
            {
                return phase;
            }
        }

        return null;
    }

    internal PlayerCharacter GetRandomPlayer()
    {
        int randPlayerID = UnityEngine.Random.Range(0, PlayerCharacterPoolManager.Instance.ActivePlayerCharacters.Count);

        return PlayerCharacterPoolManager.Instance.ActivePlayerCharacters[randPlayerID];
    }
}
