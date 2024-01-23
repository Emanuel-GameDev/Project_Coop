using MBT;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Kerberos/Raffica Di Colpi")]
    public class T_FlurryOfBlows : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private TutorialBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private bool mustStop = false;

        private int attackCount;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();            
            mustStop = false;
            bossCharacter.Agent.isStopped = false;
            attackCount = 0;

            Debug.Log("primo attacco");
            
            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), 0, (direction.z * bossCharacter.flurryDistance)) + bossCharacter.transform.position;
            bossCharacter.SetFlurryOfBlowsDamageData(attackCount);

            bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
            bossCharacter.Agent.SetDestination(targetPosition);



        }

        public override NodeResult Execute()
        {
            
            float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);

            if (mustStop || dist <= bossCharacter.minDistance)
            {
                attackCount++;
                bossCharacter.SetFlurryOfBlowsDamageData(attackCount);


                if (attackCount >= bossCharacter.punchQuantity)
                {
                    bossCharacter.Agent.isStopped = true;
                    return NodeResult.success;
                }

                else
                {
                    
                    Debug.Log("inizio attacco " + attackCount);

                    
                    Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
                    targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), 0, (direction.z * bossCharacter.flurryDistance)) + bossCharacter.transform.position;

                    bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
                    bossCharacter.Agent.SetDestination(targetPosition);
                    
                    return NodeResult.running;

                }

            }


            return NodeResult.running;
        }
        

    }


}
