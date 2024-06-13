using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemyState : BasicEnemyState
{
    protected BasicMeleeEnemy meleeEnemy;

    public BasicMeleeEnemyState(BasicMeleeEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
        meleeEnemy = basicEnemy;
    }
}
