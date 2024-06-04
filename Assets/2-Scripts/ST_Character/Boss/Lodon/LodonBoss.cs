using System;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class LodonBoss : BossCharacter
{
    StateMachine<LodonBaseState> stateMachine = new();

    [Header("Boss Behaviour")]
    [SerializeField, Range(0f, 1f), Tooltip("Indica in percentuale se deve preferire mosse a corto(0) o lungo raggio(1). \n" +
        "Più e bassa più preferirà mosse a corto raggio, più è alta più preferira mosse a lungo raggio.")]
    private float shortLongDistanceMoveSelectionPercetage = 0.35f;
    public float LongDistancePercentage => shortLongDistanceMoveSelectionPercetage;

    [Header("References")]
    [SerializeField] PlatformsGenerator generator;
    [SerializeField] Transform tridentPivot;
    [SerializeField] Damager damager;

    [Header("Fury Charge Settings")]
    public int FCDamage = 10;
    public int FCStaminaDamage = 10;
    public float FCtargetReachDistance = 5f;
    public float searchRadius = 10f;

    [Header("Trident Throwing Settings")]
    public int TTDamage = 10;
    public int TTStaminaDamage = 10;
    public float TTreachDistance = 0.1f;
    public float tridentVelocity = 50f;
    public LodonTrident Trident { get; private set; }

    [HideInInspector]
    public LodonState selectedAttack;
    [HideInInspector]
    public bool animationMustFollowTarget = false;
    [HideInInspector]
    public string lastAnimationEnd = string.Empty;


    #region Proprieties
    public Animator Animator => animator;
    public PlatformsGenerator Generator => generator;
    #endregion

    #region AnimationVariable
    public readonly string EMERGE = "Emerge";
    public readonly string FURY_CHARGE = "FuryCharge";
    public readonly string MOVING = "Moving";
    public readonly string TRIDENT_THROWING = "Throw";
    #endregion


    private void Start()
    {
        if(generator == null)
        {
            generator = (PlatformsGenerator)new GameObject("Platform Generator").AddComponent(typeof(PlatformsGenerator));    
        }
        RegisterStates();
        stateMachine.SetState(LodonState.TargetAttackSelection);
    }

    private void RegisterStates()
    {
        stateMachine.RegisterState(LodonState.Move, new LodonMove(this));
        stateMachine.RegisterState(LodonState.TargetAttackSelection, new LodonTargetAttackSelection(this));
        stateMachine.RegisterState(LodonState.TridentThrowing, new LodonTridentThrowing(this));
        stateMachine.RegisterState(LodonState.FuryCharge, new LodonFuryCharge(this));
        stateMachine.RegisterState(LodonState.OpenPlatforms, new LodonOpenPlatforms(this));
        stateMachine.RegisterState(LodonState.RocketHead, new LodonRocketHead(this));
        stateMachine.RegisterState(LodonState.LowerAnchor, new LodonLowerAnchor(this));
    }

    private void Update()
    {
        stateMachine.StateUpdate();
        SetAnimationDirection();
    }

    protected override void InitialSetup()
    {
        base.InitialSetup();
        Trident = GetComponentInChildren<LodonTrident>();
        if (Trident != null)
        {
            Trident.Inizialize(this);
        }
        else 
            Debug.LogError("TridentMissing");
        if (damager == null)
            pivot.GetComponentInChildren<Damager>();
        damager.source = this;
    }

    private void SetAnimationDirection()
    {
        Vector2 animationDirection;
        if (animationMustFollowTarget)
        {
            animationDirection = (target.position - transform.position).normalized; 
        }
        else
        {
            animationDirection = (agent.destination - transform.position).normalized;
        }

        SetSpriteDirection(animationDirection);
    }

    public void OnExitAnimation(string animationName)
    {
        Debug.Log("End Animation: " + animationName);
        if (lastAnimationEnd == animationName)
            return;

        lastAnimationEnd = animationName;
        stateMachine.CurrentState.EndAnimation();
    }

    public void ThrowTrident()
    {
        Trident.Throw(target.position, tridentPivot.position, tridentVelocity);
    }
    public void ReturnTrident()
    {
        if (stateMachine.CurrentState.GetType() == typeof(LodonTridentThrowing))
        {
            ((LodonTridentThrowing)stateMachine.CurrentState).tridentReturned = true;
        }
    }

    public override DamageData GetDamageData()
    {
        if (stateMachine.CurrentState.GetType() == typeof(LodonTridentThrowing))
        {
            DamageData damageData = new(TTDamage, TTStaminaDamage, this, true);
            return damageData;
        }

        if (stateMachine.CurrentState.GetType() == typeof(LodonTridentThrowing))
        {
            DamageData damageData = new(FCDamage, FCStaminaDamage, this, true);
            return damageData;
        }

        return base.GetDamageData();
    }

}


public enum LodonState
{
    Move,
    Wait,
    TargetAttackSelection,
    //Attacks
    TridentThrowing,
    FuryCharge,
    OpenPlatforms,
    RocketHead,
    LowerAnchor
}




//[Header("Short Distance Bheaviour")]
//[SerializeField, Range(0f, 1f)]
//private float shortFuryCharge;
//[SerializeField, Range(0f, 1f)]
//private float shortRocketHead;
//[SerializeField, Range(0f, 1f)]
//private float shortTridentThrowing;
//[SerializeField, Range(0f, 1f)]
//private float shortLowerAnchor;
//[SerializeField, Range(0f, 1f)]
//private float shortOpenPlatforms;

//[Header("Long Distance Bheaviour")]
//[SerializeField, Range(0f, 1f)]
//private float longFuryCharge;
//[SerializeField, Range(0f, 1f)]
//private float longRocketHead;
//[SerializeField, Range(0f, 1f)]
//private float longTridentThrowing;
//[SerializeField, Range(0f, 1f)]
//private float longLowerAnchor;
//[SerializeField, Range(0f, 1f)]
//private float longOpenPlatforms;

//private void OnValidate()
//{
//    ShortNormalize();
//    LongNormalize();
//}

//private void ShortNormalize()
//{
//    float total = shortFuryCharge + shortRocketHead + shortTridentThrowing + shortOpenPlatforms + shortLowerAnchor; 
//    if (total == 0f) return;

//    float scaleFactor = Mathf.Min(1 / total, 1f);
//    shortFuryCharge *= scaleFactor;
//    shortRocketHead *= scaleFactor;
//    shortTridentThrowing *= scaleFactor;
//    shortOpenPlatforms *= scaleFactor;
//    shortLowerAnchor *= scaleFactor;

//    // For real-time update in the editor
//    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
//}


//private void LongNormalize()
//{
//    float total = longFuryCharge + longRocketHead + longTridentThrowing + longLowerAnchor + longOpenPlatforms;
//    if (total == 0f) return;
//    float scaleFactor = Mathf.Min(1 / total, 1f);

//    longFuryCharge *= scaleFactor;
//    longRocketHead *= scaleFactor;
//    longTridentThrowing *= scaleFactor;
//    longOpenPlatforms *= scaleFactor;
//    longLowerAnchor *= scaleFactor;

//    // For real-time update in the editor
//    UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
//}

