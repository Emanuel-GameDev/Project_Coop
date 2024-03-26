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
    public int numJumps;
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
    public List<LBPhase> BossPhases { get { return bossPhases; } private set { } }


    [SerializeField]
    private List<TrumpOline> trumps;

    [Space]
    [Title("ATTACK")]
    [Space]

    [SerializeField]
    private GameObject projectilePrefab;
    public GameObject ProjectilePrefab { get { return projectilePrefab; } private set { } }

    [SerializeField]
    private float bombSpeed = 10f;
    public float BombSpeed { get { return bombSpeed; } set { } }

    [Space]
    [Title("JUMP")]

    [SerializeField, Tooltip("Di quanto deve essere spostato il centro del salto da un trampolino all'altro")]
    private Vector2 jumpOffset;
    public Vector2 JumpOffset { get { return jumpOffset; } private set { } }

    [Space]

    [SerializeField, Min(0.04f), Tooltip("un tempo di attesa tra un salto e l'altro")]
    private float jumpStep = 1.5f;
    public float JumpStep { get { return jumpStep; } private set { } }

    [SerializeField, Tooltip("Attiva o disattiva il jump step, se è disattivato significa che tra un salto e l'altro non c'è attesa")]
    private bool activateJumpStep = true;
    public bool ActivateJumpStep { get { return activateJumpStep; } private set { } }

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterJump = 1f;
    public float TimeAfterJump { get { return timeAfterJump; } private set { } }

    [Space]
    [Title("CARROT_BREAK")]
    [Space]

    [SerializeField, Tooltip("Durata della pausa carota")]
    private float breakTime;
    public float BreakTime { get { return breakTime; } private set { } }

    [SerializeField, Tooltip("Potenza del rimbalzo")]
    private float bounceHeight = 10f;
    public float BounceHeight { get { return bounceHeight; } private set { } }

    [SerializeField, Tooltip("la forza effettiva del rimbalzo sarà un random compreso tra: \njumpForce - randomBounceRange \njumpForce + randomBounceRange")]
    private float randBounceRange = 3f;
    public float RandBounceRange { get { return randBounceRange; } private set { } }

    [Space]
    [Title("PANIC")]
    [Space]

    [SerializeField]
    private List<TrumpOline> route1;
    public List<TrumpOline> Route1 { get { return route1; } private set { } }

    [SerializeField]
    private List<TrumpOline> route2;
    public List<TrumpOline> Route2 { get { return route2; } private set { } }

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterPanic = 1f;
    public float TimeAfterPanic { get { return timeAfterPanic; } private set { } }

    #endregion

    private void Start()
    {
        Agent.updateRotation = false;

        InitializePhases();

        stateMachine.SetState(new LBStart(this));
    }

    private void InitializePhases()
    {
        for (int i = 0; i < bossPhases.Count; i++)
        {
            int rand = UnityEngine.Random.Range(bossPhases[i].minJumps, bossPhases[i].maxJumps + 1);
            bossPhases[i].numJumps = rand;
        }
    }

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

}
