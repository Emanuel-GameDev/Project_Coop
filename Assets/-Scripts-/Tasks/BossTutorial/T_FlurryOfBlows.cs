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

        private int attackCount;
        

        public override void OnEnter()
        {
            
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            bossCharacter.Agent.isStopped = false;
            bossCharacter.previewStarted = false;
            bossCharacter.canLastAttackPunch = false;


            bossCharacter.SetCanShowPreview();

            attackCount = 0;
            bossCharacter.SetFlurryOfBlowsDamageData(attackCount);
            bossCharacter.Agent.speed = bossCharacter.flurrySpeed;
            bossCharacter.canAttackAnim = false;


            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), (direction.y * bossCharacter.flurryDistance), 0) + bossCharacter.transform.position;

            bossCharacter.anim.ResetTrigger("Return");
        }

        public override NodeResult Execute()
        {
            if (!bossCharacter.isDead) 
            { 
            float dist = Vector2.Distance(targetPosition, bossCharacter.transform.position);

                //esci da attacco
                if (attackCount >= bossCharacter.punchQuantity && bossCharacter.canLastAttackPunch)
                {
                    bossCharacter.anim.SetTrigger("Return");
                    return NodeResult.success;
                }

                else
                {
                    //attacco
                    if (bossCharacter.canAttackAnim)
                    {

                        attackCount++;
                        bossCharacter.SetFlurryOfBlowsDamageData(attackCount);

                        Debug.Log("inizio attacco " + attackCount);

                        Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;

                        targetPosition = new Vector3((direction.x * bossCharacter.flurryDistance), (direction.y * bossCharacter.flurryDistance), 0) + bossCharacter.transform.position;

                        bossCharacter.canAttackAnim = false;

                        bossCharacter.anim.SetTrigger("FlurryOfBlows" + attackCount);

                        bossCharacter.Agent.SetDestination(targetPosition);

                        return NodeResult.running;
                    }

                    //preview attacco
                    else
                    {
                        if (!bossCharacter.previewStarted)
                        {
                            NextAttackPreview();

                        }
                    }

                }
            }

            return NodeResult.running;
        }

        public void NextAttackPreview()
        {                       
            bossCharacter.previewStarted = true;
            bossCharacter.previewArrow.SetActive(true);
            StartCoroutine(bossCharacter.StartAttackPunch());
        }



    }


}
