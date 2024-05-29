using System;
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

    [Header("Fury Charge Settings")]
    public float FCtargetReachDistance = 5f;
    public float searchRadius = 10f;

    [HideInInspector]
    public LodonState selectedAttack;
    [HideInInspector]
    public bool animationMustFollowTarget = false;

    #region Proprieties
    public Animator Animator => animator;
    public PlatformsGenerator Generator => generator;
    #endregion

    public string lastAnimationEnd = string.Empty;

    #region AnimationVariable
    public readonly string EMERGE = "Emerge";
    public readonly string FURY_CHARGE = "FuryCharge";
    public readonly string MOVING = "Moving";
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

