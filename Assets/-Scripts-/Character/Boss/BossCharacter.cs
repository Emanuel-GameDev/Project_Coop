using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyCharacter
{
    BossCharacterClass bossCharacterClass;
    public void Attack1() => bossCharacterClass.Attack1();
    public void Attack2() => bossCharacterClass.Attack2();
    public void Attack3() => bossCharacterClass.Attack3();
}
