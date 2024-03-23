using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LBCarrotBreak : LBBaseState
{
    private TrumpOline trump;
    private Rigidbody2D bossRB;
    
    private bool listenerAdded = false;
    private bool canJump = false;
    private float timer;

    public LBCarrotBreak(LunaticBoomyBossCharacter bossCharacter, TrumpOline trump) : base(bossCharacter)
    {
        this.trump = trump;
    }

    public override void Enter()
    {
        base.Enter();

        bossRB = bossCharacter.gameObject.GetComponent<Rigidbody2D>();
        timer = bossCharacter.BreakTime;

        canJump = true;
        Jump();
    }

    void Jump()
    {
        if (!canJump)
        {
            // Cambio stato 

            // Temp
            trump.gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
            Debug.Log("CambioStato");
            return;
        }

        // Applica una forza verso l'alto per simulare il salto
        bossRB.velocity = Vector2.zero;

        float randForce = Random.Range(bossCharacter.BounceForce - bossCharacter.RandBounceRange,
                                                   bossCharacter.BounceForce + bossCharacter.RandBounceRange);

        bossRB.AddForce(Vector2.up * randForce, ForceMode2D.Impulse);

        if (!listenerAdded)
        {
            trump.OnTriggerEnterEvent += OnCollisionWithTrump;
            listenerAdded = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (canJump)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
                canJump = false;
        }
    }

    private void OnCollisionWithTrump(Collider2D collision)
    {
        Jump();
    }
}
