using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LunaticBoomy : BossCharacter
{
    StateMachine<LBBaseState> stateMachine = new();

    #region Variables

    [SerializeField]
    List<TrumpOline> trumps = new List<TrumpOline>();

    TrumpOline currTrump;
    TrumpOline nextTrump;

    #endregion

    private void Start()
    {
        stateMachine.SetState(new LBStart(this));
    }

    private void Update()
    {
        stateMachine.StateUpdate();
    }

}
