using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class BasicRangedEnemyEscapeState : BasicEnemyState
{
    int maxTry=10;
    int remainingTry;
    float maxTime=5;
    float currentTime;

    Vector2 pos;

    public BasicRangedEnemyEscapeState(BasicEnemy basicEnemy)
    {
        this.basicEnemy = basicEnemy;
        
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

        Debug.Log(" sto cercando");

        //TODO scappo dal tizio

        if (basicEnemy.EscapeTrigger.GetPlayersCountInTrigger() == 0 ) 
        {
            if(basicEnemy.Agent.remainingDistance <= basicEnemy.Agent.stoppingDistance)
            {
                stateMachine.SetState(basicEnemy.actionState);
            }
            
        }
        else 
        {
            if(basicEnemy.Agent.remainingDistance <= basicEnemy.Agent.stoppingDistance) 
            {
                List<PlayerCharacter> players;

                players = new List<PlayerCharacter>(basicEnemy.EscapeTrigger.GetPlayersDetected());

                PlayerCharacter target = players[0];

                for(int i=0; i < players.Count; i++)
                {
                    if( Mathf.Abs((basicEnemy.transform.position-target.transform.position).magnitude) >= Mathf.Abs((basicEnemy.transform.position - players[i].transform.position).magnitude)) 
                    {
                        target = players[i];
                    }
                }


                Vector2 randomPos;
                
                NavMeshHit hit;

                Vector2 deltaVector=basicEnemy.transform.position- target.transform.position;

                Vector2 xDirectionRand= new Vector2(deltaVector.x>0 ? 0 : -1, deltaVector.x > 0? 1 : 0);
                Vector2 yDirectionRand = new Vector2(deltaVector.y > 0 ? 0 : -1, deltaVector.y > 0 ? 1 : 0);

                do
                {
                    
                    Vector2 randomVect = new Vector2(UnityEngine.Random.Range(xDirectionRand.x, xDirectionRand.y), UnityEngine.Random.Range(yDirectionRand.x, yDirectionRand.y));
                    randomPos = basicEnemy.transform.position +  ((Vector3)randomVect * basicEnemy.searchRadious);;
                    pos = new Vector2(randomPos.x,randomPos.y);

                    remainingTry--;

                    //finire

                    
                }
                while (!NavMesh.SamplePosition(pos,out hit,basicEnemy.searchRadious,basicEnemy.Agent.areaMask) && remainingTry>=0);

                basicEnemy.Agent.SetDestination(pos);

                if(currentTime <= 0 || remainingTry<=0)
                {
                    basicEnemy.panicAttack = true;
                    stateMachine.SetState(basicEnemy.actionState);
                }

                remainingTry = maxTry;
            }

            

        }

        basicEnemy.FollowPosition(pos);
    }

    public override void Exit()
    {
        base.Exit();
        basicEnemy.Agent.isStopped=true;
        basicEnemy.GetAnimator().SetBool("isMoving", false);
    }
}
