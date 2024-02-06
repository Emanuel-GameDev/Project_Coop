using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBCharacterSM : BossCharacter
{
    [Header("Generics")]
    public float minDistance = 0.1f;
    [Header("Target & Move Selection")]
    public float shortDistanceRange = 15f;
    [Range(0, 1f)]
    public float shortDistanceChancePercentage = 0.65f;
    [Range(0, 1f)]
    public float moveRepeatPercentage = 0.25f;
    [Header("Raffica Di Pugni")]
    public float flurryDistance;
    public float flurrySpeed;
    public int punchQuantity;
    [Header("Carica")]
    public float chargeTimer;
    public float chargeDuration;
    public float chargeSpeed;
    public float chargeDistance;
    [Header("Mvoimento")]
    public float moveDuration;


    bool selectFarPlayers = false;
    StateMachine<TutorialBossState> stateMachine = new();
    TutorialBossState forcedNextState = null;

    private void Start()
    {
        stateMachine.SetState(new TBStart(this));
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

    public override void TargetSelection()
    {
        List<PlayerCharacter> activePlayers = GameManager.Instance.coopManager.activePlayers;
        selectFarPlayers = Random.Range(0f, 1f) > shortDistanceChancePercentage;
        List<PlayerCharacter> selectedPlayers = new();
        foreach (PlayerCharacter player in activePlayers)
        {
            bool isFar = Utility.DistanceV3toV2(player.transform.position, transform.position) > shortDistanceRange;

            if ((selectFarPlayers && isFar) || (!selectFarPlayers && !isFar))
                selectedPlayers.Add(player);
        }

        if (selectedPlayers.Count == 0)
            selectedPlayers = activePlayers;

        if (selectedPlayers.Count == 1)
        {
            target = selectedPlayers[0].transform;
            return;
        }

        target = selectedPlayers[Random.Range(0, selectedPlayers.Count)].transform;

    }
    #region StateSelection
    public void StateSelection()
    {
        if (forcedNextState != null)
        {
            stateMachine.SetState(forcedNextState);
            forcedNextState = null;
            return;
        }

        if (selectFarPlayers)
            stateMachine.SetState(FarMoveSelection());
        else
            stateMachine.SetState(NearMoveSelection());
    }

    public TutorialBossState NearMoveSelection()
    {
        List<TutorialBossState> nearStates = new List<TutorialBossState>
        {
            new TBSlam(this),
            new TBFlurryOfBlows(this)
        };
        if (Random.Range(0f, 1f) > moveRepeatPercentage)
            nearStates.RemoveAll(state => state.GetType() == stateMachine.CurrentState.GetType());

        if (nearStates.Count == 0)
            Debug.LogWarning("No near states found");

        return nearStates[Random.Range(0, nearStates.Count)];
    }

    private TutorialBossState FarMoveSelection()
    {
        float range = Random.Range(0f, 1f);
        if (range < 0.5f) return new TBMove(this);
        else if (range < 0.75f) return new TBSlam(this);
        else return new TBCharge(this);
    }

    public void SetNexState(TutorialBossState state)
    {
        forcedNextState = state;
    }

    public void ChangeState()
    {
        TargetSelection();
        StateSelection();
    }

    #endregion



}
