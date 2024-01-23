using MBT;
using System.Collections.Generic;
using UnityEngine;

public enum PhaseRainOfAss
{
    damageable,
    follow,
    slam

}
namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("PrisonerEros/PioggiaDiNatiche")]
    public class T_RainOfAss : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private PrisonErosBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private bool mustStop = false;

        private PhaseRainOfAss phaseAttack;
        private int attackCount;
        private float tempTimerDamageable;
        private float tempTimerFollow;
        private bool canFollow;
        

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<PrisonErosBossCharacter>();
            mustStop = false;
            bossCharacter.Agent.isStopped = false;
            attackCount = 0;
            tempTimerDamageable = 0;
            tempTimerFollow = 0;
            phaseAttack = PhaseRainOfAss.follow;
            

            //sale su con animazione
            bossCharacter.GetComponentInChildren<Collider>().enabled = false;
            bossCharacter.GetComponentInChildren<SpriteRenderer>().enabled = false;
            bossCharacter.objectShadowSlam.SetActive(true);
            


        }

        public override NodeResult Execute()
        {

            switch (phaseAttack) 
            {
                case PhaseRainOfAss.follow:
                    break;

                case PhaseRainOfAss.damageable:
                    break;

                    case PhaseRainOfAss.slam:
                    break;

            }




            if (attackCount <= bossCharacter.slamsQuantity)
            {
                attackCount++;

                //Inizio inseguimento player
                if (canFollow && bossCharacter.followDuration >= tempTimerFollow)
                {
                    tempTimerFollow += Time.deltaTime;
                    //Follow target
                    targetPosition = targetTransform.Value.position;
                    bossCharacter.Agent.speed = bossCharacter.walkSpeed;
                    bossCharacter.Agent.SetDestination(targetPosition);
                }
                else
                {
                    canFollow = false;
                    //Inizio schianto
                    Debug.Log("BOOOM");
                    

                }
            }
            else
            {
                return NodeResult.success;
            }

            
            return NodeResult.running;

        }
           
    }


}
