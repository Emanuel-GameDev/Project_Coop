using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
        pivot = GetComponentInChildren<Pivot>();
    }

    public override DamageData GetDamageData()
    {
        return new DamageData(damage, staminaDamage, this, attackCondition, true);
    }

    protected virtual void SetSpriteDirection(Vector2 direction)
    {
        if (direction.y != 0)
            animator.SetFloat(Y, direction.y);

        Vector3 scale = pivot.gameObject.transform.localScale;

        if ((direction.x > 0.5 && scale.x > 0) || (direction.x < -0.5 && scale.x < 0))
            scale.x *= -1;

        pivot.gameObject.transform.localScale = scale;

    }
}
