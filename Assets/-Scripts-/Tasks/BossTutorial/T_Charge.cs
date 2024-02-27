using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Attacco Carica ")]
    public class T_Charge : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        private TutorialBossCharacter bossCharacter;
        private bool started = false;
        private bool mustStop = false;
        private float tempTimer;        
        private Vector3 targetPosition;
        private IDamager bossDamager;

        public override void OnEnter()
        {
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
           
            started = false;
            mustStop = false;

            Vector3 direction = (targetTransform.Value.position -bossCharacter.transform.position).normalized;
            targetPosition = new Vector3((direction.x * bossCharacter.chargeDistance), (direction.y * bossCharacter.chargeDistance),0) + bossCharacter.transform.position; 

           
            //Setto il danno
            bossCharacter.SetChargeDamageData();

            Debug.Log("Start Charge Timer");
            ShowAttackPreview(true);
            tempTimer = 0;

        }
        
           

        public override NodeResult Execute()
        {           
            
            
            if(tempTimer > bossCharacter.chargeTimer)
            {
                if (!started)
                {
                    ShowAttackPreview(false);
                    Debug.Log("partito");
                    bossCharacter.Agent.isStopped = false;
                    bossCharacter.Agent.speed = bossCharacter.chargeSpeed;
                    bossCharacter.Agent.SetDestination(targetPosition);
                    bossCharacter.anim.SetTrigger("Charge");
                    started = true;

                }

                float dist = Vector2.Distance(targetPosition, bossCharacter.transform.position);
               
                if(mustStop || dist <= bossCharacter.minDistance)
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

        public void ShowAttackPreview(bool value)
        {
            bossCharacter.canShowPreview = value;           
            bossCharacter.previewArrow.SetActive(value);
            
        }
    }
}