using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyCharacter
{

    protected Condition attackCondition;

    protected Pivot pivot;

    #region Animation Variable
    private static string Y = "Y";
    #endregion
    override protected void Awake() 
    { 
        base.Awake(); 
        pivot = GetComponent<Pivot>(); 
    }


    public override void TargetSelection()
    {
        base.TargetSelection();
    }

    

    public override DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, attackCondition, true);
    }

    



    protected virtual void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat(Y, direction.y);
        Quaternion rotation = pivot.gameObject.transform.localRotation;
        if (direction.x > 0.5 && rotation.x > 0)
            rotation.x = 0;
        if (direction.x < -0.5 && rotation.x < 180)
            rotation.x = 180;

        pivot.gameObject.transform.localRotation = rotation;
    }

}
