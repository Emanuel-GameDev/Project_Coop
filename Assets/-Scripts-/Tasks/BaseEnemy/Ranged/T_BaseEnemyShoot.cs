using MBT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    namespace MBTExample
{
    [AddComponentMenu("")]
    [MBTNode("EnemyBase/Shoot")]
    public class T_BaseEnemyShoot : Leaf
    {
        public TransformReference targetTransform;
        public GameObjectReference parentGameObject;

        public override NodeResult Execute()
        {
            throw new System.NotImplementedException();
        }

        public override void OnEnter()
        {
            Debug.Log("daglie roma");
        }
    }
}
