using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class BasicRangedEnemyEscapeState : BasicRangedEnemyState
{
    int maxTry=10;
    int remainingTry;
    float maxTime=5;
    float currentTime;

    Vector2 pos;

    public BasicRangedEnemyEscapeState(RangedEnemy basicEnemy): base(basicEnemy)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        basicEnemy.canMove = true;
        basicEnemy.canAction = false;

        currentTime = maxTime;
        remainingTry = maxTry;

        basicEnemy.ActivateAgent();

        basicEnemy.GetAnimator().SetBool("isMoving", true);

    }

    
    public override void Update()
    {
        base.Update();

        currentTime-=Time.deltaTime;

        Debug.Log("sto cercando");

        

        Vector2 randomPos;

        NavMeshHit hit;

        Vector2 deltaVector = rangedEnemy.transform.position - rangedEnemy.target.transform.position;

        Vector2 xDirectionRand = new Vector2(deltaVector.x > 0 ? 0 : -1, deltaVector.x > 0 ? 1 : 0);
        Vector2 yDirectionRand = new Vector2(deltaVector.y > 0 ? 0 : -1, deltaVector.y > 0 ? 1 : 0);

        do
        {
            Vector2 randomVect = new Vector2(UnityEngine.Random.Range(xDirectionRand.x, xDirectionRand.y), UnityEngine.Random.Range(yDirectionRand.x, yDirectionRand.y));
            randomPos = basicEnemy.transform.position + ((Vector3)randomVect * rangedEnemy.searchRadious);
            pos = new Vector2(randomPos.x, randomPos.y);

            remainingTry--;

            //finire
        }
        while (!NavMesh.SamplePosition(pos, out hit, rangedEnemy.searchRadious, basicEnemy.Agent.areaMask) && remainingTry >= 0);

        basicEnemy.Agent.SetDestination(pos);

        if (currentTime <= 0 || remainingTry <= 0)
        {
            rangedEnemy.panicAttack = true;
            stateMachine.SetState(rangedEnemy.actionState);
        }

        remainingTry = maxTry;
    

    

        if (rangedEnemy.EscapeTrigger.GetPlayersCountInTrigger() == 0 || rangedEnemy.panicAttack)
        {
            
            rangedEnemy.stateMachine.SetState(rangedEnemy.actionState);
        }


        rangedEnemy.AwayPath();
    }



    public override void Exit()
    {
        base.Exit();
        
        basicEnemy.Agent.isStopped=true;
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
