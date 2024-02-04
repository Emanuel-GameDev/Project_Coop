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

            bossCharacter.SetCrashDirectDamageData();
            Debug.Log("Schianto Iniziato");
            bossCharacter.anim.SetTrigger("Crash");
        }

        public override NodeResult Execute()
        {
            
         
            if (tempTimer > bossCharacter.crashTimer)
            {
                Debug.Log("Schianto Finito");
                bossCharacter.anim.SetTrigger("Return");
                return NodeResult.success;
            }

            else
            {
                tempTimer += Time.deltaTime;
            }

            return NodeResult.running;
        }

    }
}