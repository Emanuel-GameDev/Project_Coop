using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_KIllAllInTimerBoss : C_KillAllInTimer
{
    public override ChallengeName challengeNameEnum => challengeEnumName;
    [Header("EnumName Boss")]
        public ChallengeName challengeEnumName;

}
