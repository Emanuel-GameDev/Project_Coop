using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

    [SerializeField, Tooltip("Lista per avere tutte le fasi del boss modificabili, non settare più di una fase ad active atrimenti verrà considerata attiva la più vicina all'inizio")]
    private List<LBPhase> bossPhases;
    public List<LBPhase> BossPhases => bossPhases;

    [SerializeField]
    private List<TrumpOline> trumps;

    [Space]

    [SerializeField, Tooltip("Il bordo dell'arena, serve a determinare il punto di sparo random durante la terza fase")]
    private Collider2D arenaCollider;

    [Space]
    [Title("ATTACK")]
    [Space]

    [SerializeField, Tooltip("Il prefab del proiettile")]
    private GameObject projectilePrefab;

    [SerializeField]
    private int poolSize = 3;

    [SerializeField]
    private float projectileSpeed = 10f;
    public float ProjectileSpeed => projectileSpeed;

    [SerializeField, Tooltip("Un'oggetto che avrà i proiettili come figli, se non è inserito nel prefab bisogna creare un'empty e metterlo qua")]
    private GameObject projectilePoolGO;

    private GameObject[] projectilePool;


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

    [SerializeField, Range(0f, 1f), Tooltip("La probabilità che il boss salti su un trampolino distrutto, si applica ad ogni salto")]
    private float destroyedJumpProb = 0.2f;
    public float DestroyedJumpProb => destroyedJumpProb;

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

    [SerializeField]
    private float panicSpeed = 2.5f;
    public float PanicSpeed => panicSpeed;

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterPanic = 1f;
    public float TimeAfterPanic => timeAfterPanic;

    #endregion

    #region Projectiles

    private void InitializePool()
    {
        projectilePool = new GameObject[poolSize];

        for (int i = 0; i < projectilePool.Length; i++)
        {
            projectilePool[i] = Instantiate(projectilePrefab, gameObject.transform.position, Quaternion.identity);
            projectilePool[i].transform.SetParent(projectilePoolGO.transform);
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

    #region Arena

    internal Vector2 GetRandomPointArena()
    {
        Vector2 randomPoint = Vector2.zero;

        // Generare un punto casuale all'interno del collider dell'arena
        randomPoint.x = UnityEngine.Random.Range(arenaCollider.bounds.min.x, arenaCollider.bounds.max.x);
        randomPoint.y = UnityEngine.Random.Range(arenaCollider.bounds.min.y, arenaCollider.bounds.max.y);

        // Assicurarsi che il punto generato sia effettivamente sulla superficie dell'arena
        while (!IsPointOnColliderSurface(randomPoint))
        {
            randomPoint.x = UnityEngine.Random.Range(arenaCollider.bounds.min.x, arenaCollider.bounds.max.x);
            randomPoint.y = UnityEngine.Random.Range(arenaCollider.bounds.min.y, arenaCollider.bounds.max.y);
        }

        return randomPoint;
    }

    bool IsPointOnColliderSurface(Vector2 point)
    {
        // Controlla se il punto si trova all'interno del collider
        if (!arenaCollider.OverlapPoint(point))
            return false;

        return true;
    }

    #endregion

    #region UnityRelated

    private void Start()
    {
        Agent.updateRotation = false;

        InitializePool();

        stateMachine.SetState(new LBStart(this));
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

    #endregion

    #region Getters

    internal List<TrumpOline> GetTrumps()
    {
        if (trumps.Count <= 0)
        {
            Debug.LogError("Error in TrumpOline List");
            return null;
        }

        return trumps;
    }

    internal List<TrumpOline> GetPanicRoute(TrumpOline trump)
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

    internal LBPhase GetActivePhase()
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

    #endregion

    #region Setters

    internal void TriggerAgent(bool mode)
    {
        if (mode)
            GetComponent<NavMeshAgent>().enabled = true;
        else
            GetComponent<NavMeshAgent>().enabled = false;
    }

    internal void IncreasePhase(int actualPhaseNum)
    {
        for (int i = 0; i < bossPhases.Count; i++)
        {
            if (bossPhases[i].phaseNum == actualPhaseNum)
                bossPhases[i].active = true;
            else
                bossPhases[i].active = false;
        }
    }

    #endregion

}
