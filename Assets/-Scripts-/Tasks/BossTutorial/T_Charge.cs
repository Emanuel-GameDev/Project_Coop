using MBT;
using UnityEngine;
using UnityEngine.AI;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Attacco Carica ")]
    public class T_Charge : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private KerberosBossCharacter bossCharacter;
        private bool started = false;
        private bool mustStop = false;
        private float tempTimer;
        private Vector3 targetPosition;


        public override void OnEnter()
        {

            bossCharacter = parentGameObject.Value.GetComponent<KerberosBossCharacter>();

            started = false;
            mustStop = false;
            bossCharacter.parried = false;
            bool temp = false;
            int i = 0;


            Vector3 direction = (targetTransform.Value.position - bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.chargeDistance), (direction.y * bossCharacter.chargeDistance), 0) + bossCharacter.transform.position;

            while (!temp)
            {
                i++;
                Vector3 offset = new Vector3((i / 10f) * direction.x, (i / 10f) * direction.y, 0);
                Vector3 newTargetPosition = targetPosition - offset;

                if (NavMesh.SamplePosition(newTargetPosition, out NavMeshHit hit, bossCharacter.chargeDistance, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                    temp = true;
                }
                
            }




            //Setto il danno
            bossCharacter.SetChargeDamageData();
            bossCharacter.anim.SetTrigger("PrepCharge");
            ShowAttackPreview(true);
            tempTimer = 0;
            bossCharacter.anim.ResetTrigger("Return");

        }



        public override NodeResult Execute()
        {
            if (!bossCharacter.isDead)
            {

                if (bossCharacter.parried)
                {
                    mustStop = true;

                    //funzione player spinta inetro                    
                    bossCharacter.whoParried.StartCoroutine(bossCharacter.whoParried.PushCharacter(bossCharacter.transform.position,
                        bossCharacter.ChargeOnParryPushForce, bossCharacter.ChargeOnParryDuration));
                }

                if (tempTimer > bossCharacter.chargeTimer)
                {
                    if (!started)
                    {
                        ShowAttackPreview(false);
                       
                        bossCharacter.Agent.isStopped = false;
                        bossCharacter.Agent.speed = bossCharacter.chargeSpeed;
                        bossCharacter.Agent.SetDestination(targetPosition);
                        bossCharacter.anim.SetTrigger("Charge");
                        started = true;

                    }

                    float dist = Vector2.Distance(targetPosition, bossCharacter.transform.position);

                    if (mustStop || dist <= bossCharacter.minDistance)
                    {

                        bossCharacter.Agent.isStopped = true;
                        bossCharacter.anim.SetTrigger("Return");

                        return NodeResult.success;
                    }
                }

                else
                {
                    tempTimer += Time.deltaTime;
                }

                return NodeResult.running;
            }
            return NodeResult.running;
        }

        public void ShowAttackPreview(bool value)
        {
            bossCharacter.canShowPreview = value;
            bossCharacter.pivotPreviewArrow.SetActive(value);

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPosition, 1);
        }
    }
    
}