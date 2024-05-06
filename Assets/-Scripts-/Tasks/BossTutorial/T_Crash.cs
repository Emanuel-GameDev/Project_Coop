using MBT;
using UnityEngine;

namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("Custom Taks/Attacco Schianto ")]
    public class T_Crash : Leaf
    {
        public GameObjectReference parentGameObject;
        public BoolReference parriedBool;
        private KerberosBossCharacter bossCharacter;

        private float tempTimer;
        

        public override void OnEnter()
        {

            bossCharacter = parentGameObject.Value.GetComponent<KerberosBossCharacter>();
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


                if (bossCharacter.parried)
                {                    
                    bossCharacter.anim.SetTrigger("Parried");
                    parriedBool.Value = true;
                    StartCoroutine(bossCharacter.UnstunFromParry());
                    return NodeResult.success;
                }

                else
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
            }
            return NodeResult.running;
        }

    }

}