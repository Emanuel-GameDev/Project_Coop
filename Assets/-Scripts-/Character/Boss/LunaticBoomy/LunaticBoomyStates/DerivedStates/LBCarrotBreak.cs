using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBCarrotBreak : LBBaseState
{
    private TrumpOline trump;
    
    private bool bouncing = false;

    public LBCarrotBreak(LunaticBoomyBossCharacter bossCharacter, TrumpOline trump) : base(bossCharacter)
    {
        this.trump = trump;
    }

    public override void Enter()
    {
        base.Enter();

        // Da in serir nel' update
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 500f, ForceMode2D.Impulse);

        //trump.OnTriggerEnterEvent += OnCollisionWithTrump;
        //trump.OnTriggerStayEvent += OnCollisionWithTrump;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    private void OnCollisionWithTrump(Collider2D collision)
    {
        Debug.Log("rimbalzo");

        if (bouncing) return;

        bouncing = true;
        bossCharacter.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 100f, ForceMode2D.Impulse);
    }
}
