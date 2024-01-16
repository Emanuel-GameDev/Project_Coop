using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TBStart : TutorialBossState
{
    public TBStart(TBCharacterSM bossCharacter) : base(bossCharacter)
    {
    }

    float waitTime = 5f;

    public override void Enter()
    {
        waitTime = 5f;
    }

    public override void Update()
    {
        waitTime -= Time.deltaTime;
        if(waitTime <= 0)
            bossCharacter.ChangeState();
    }
}
    

