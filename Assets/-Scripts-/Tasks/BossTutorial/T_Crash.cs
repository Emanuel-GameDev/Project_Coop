using MBT;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Attacco Schianto ")]
    public class T_Crash : Leaf
    {       
        public GameObjectReference parentGameObject;
        private TutorialBossCharacter bossCharacter;
        private float tempTimer;

        public override void OnEnter()
        {
          
            bossCharacter = parentGameObject.Value.GetComponent<TutorialBossCharacter>();
            tempTimer = 0;
            bossCharacter.parried = false;

            bossCharacter.SetCrashDirectDamageData();           
            bossCharacter.anim.SetTrigger("Crash");
            bossCharacter.anim.ResetTrigger("Return");
        }

        public override NodeResult Execute()
        {

            if (!bossCharacter.isDead)
            {
                
                if (tempTimer > bossCharacter.crashTimer)
                {

                    bossCharacter.anim.SetTrigger("Return");

                    return NodeResult.success;
                }

                else
                {
                    tempTimer += Time.deltaTime;
                }

                return NodeResult.running;
            }
            return NodeResult.running;
        }

    }
}