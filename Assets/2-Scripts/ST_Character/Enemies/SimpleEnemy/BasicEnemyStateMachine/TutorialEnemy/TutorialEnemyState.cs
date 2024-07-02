using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyState : BasicMeleeEnemyState
{
    protected TutorialEnemy tutorialEnemy;

    public TutorialEnemyState(TutorialEnemy basicEnemy) : base(basicEnemy)
    {
        this.basicEnemy = basicEnemy;
        tutorialEnemy = basicEnemy;
    }
}
