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

    [Header("GENERAL")]

    [SerializeField]
    private List<LBPhase> bossPhases;
    public List<LBPhase> BossPhases { get { return bossPhases; } private set { } }

    [Header("JUMP")]

    [SerializeField]
    private List<TrumpOline> trumps;

    [SerializeField]
    private float jumpSpeed = 2f;
    public float JumpSpeed { get { return jumpSpeed; } private set { } }

    [SerializeField]
    private Vector2 jumpOffset;
    public Vector2 JumpOffset { get { return jumpOffset; } private set { } }

    [SerializeField]
    private float timeBetweenJumps = 1.5f;
    public float TimeBetweenJumps { get { return timeBetweenJumps; } private set { } }

    [Header("CARROT_BREAK")]

    [SerializeField, Tooltip("Durata della pausa carota")]
    private float breakTime;
    public float BreakTime { get { return breakTime; } private set { } }

    [SerializeField, Tooltip("Potenza del rimbalzo")]
    private float bounceForce = 10f;
    public float BounceForce { get { return bounceForce; } private set { } }

    [SerializeField, Tooltip("la forza effettiva del rimbalzo sarà un random compreso tra jumpForce - randomBounceRange e jumpForce + randomBounceRange")]
    private float randBounceRange = 3f;
    public float RandBounceRange { get { return randBounceRange; } private set { } }

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

}
