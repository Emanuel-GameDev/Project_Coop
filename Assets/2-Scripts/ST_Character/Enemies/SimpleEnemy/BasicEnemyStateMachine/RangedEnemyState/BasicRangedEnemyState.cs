using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRangedEnemyState : BasicEnemyState
{
    protected RangedEnemy rangedEnemy;

    public BasicRangedEnemyState(RangedEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
        rangedEnemy = basicEnemy;
    }
}
