using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCharacter : EnemyCharacter
{
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
