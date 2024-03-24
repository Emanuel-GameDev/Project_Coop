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
    [Title("JUMP")]
    [Space]

    [SerializeField]
    private float jumpSpeed = 2f;
    public float JumpSpeed { get { return jumpSpeed; } private set { } }

    [SerializeField]
    private Vector2 jumpOffset;
    public Vector2 JumpOffset { get { return jumpOffset; } private set { } }

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
    private float bounceForce = 10f;
    public float BounceForce { get { return bounceForce; } private set { } }

    [SerializeField, Tooltip("la forza effettiva del rimbalzo sarà un random compreso tra jumpForce - randomBounceRange e jumpForce + randomBounceRange")]
    private float randBounceRange = 3f;
    public float RandBounceRange { get { return randBounceRange; } private set { } }

    [SerializeField, Tooltip("Tempo di attesa dopo la fine del giro durante PANIC per dare temo alle animazioni / migliorare la resa visiva")]
    private float timeAfterBreak = 1f;
    public float TimeAfterBreak { get { return timeAfterBreak; } private set { } }

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

    public void Attack()
    {
        // Spara una bomba ad un pg a caso

        // Prendo pg a caso
        int randCharID = UnityEngine.Random.Range(0, GameManager.Instance.coopManager.ActivePlayers.Count);

    }

    public void DoubleAttack()
    {
        // Spara 2 bombe a 2 pg a caso, non può essere lo stesso a venir targettato 2 volte
    }

}
