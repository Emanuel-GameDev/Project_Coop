using MBT;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Raffica Di Colpi ")]
    public class T_RafficaDiColpi : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;
        public float minDistance = 0.1f;
        public int attacksNumber;

        private TutorialBossCharacter bossCharacter;
        private Vector3 targetPosition;
        private bool mustStop = false;
        private int attackCount;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            targetPosition = targetTransform.Value.position;
            attackCount = 0;


        }

        public override NodeResult Execute()
        {
            float dist = Vector3.Distance(targetPosition, bossCharacter.transform.position);
            if (mustStop || dist <= minDistance)
            {
                bossCharacter.Agent.isStopped = true;
                attackCount++;

                if (attackCount >= attacksNumber)
                {
                    return NodeResult.success;
                }
                else
                {

                }

            }


            return NodeResult.running;
        }

    }


}
