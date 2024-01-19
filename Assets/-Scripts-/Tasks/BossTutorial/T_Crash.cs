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
        }

        public override NodeResult Execute()
        {

            //Play animazione con tutta la gestione, devo chiamare da qualche parte bossCharacter.SetCrashWaveDamageData();

            //
            if (tempTimer > bossCharacter.crashTimer)
            {
                Debug.Log("Schianto Finito");
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