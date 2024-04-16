using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartLabirint : LabirintState
{
    public override void Enter()
    {
        base.Enter();
        LabirintManager.Instance.StartCoroutine(WaitForPlayers());
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    IEnumerator WaitForPlayers()
    {
        yield return new WaitUntil(() => CoopManager.Instance.GetActiveHandlers() != null && CoopManager.Instance.GetActiveHandlers().Count > 0);
        //MinigameMenuManager.Instance.StartFirstMenu();
        LabirintManager.Instance.dialogueObject.SetActive(true);
    }

}
