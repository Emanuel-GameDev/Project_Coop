using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class LBCarrotBreak : LBBaseState
{
    private TrumpOline trump;
    private Rigidbody2D bossRB;
    
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
        bossRB.bodyType = RigidbodyType2D.Dynamic;
        bossRB.constraints = RigidbodyConstraints2D.FreezePositionX;

        timer = bossCharacter.BreakTime;

        canJump = true;
        trump.OnTriggerEnterEvent += OnTrumpTriggered;
    }

    void Jump()
    {
        if (!canJump)
        {
            // Cambio stato 
            stateMachine.SetState(new LBJumpAttack(bossCharacter, trump));

            return;
        }
        
        // Applica una forza verso l'alto per simulare il salto
        bossRB.velocity = Vector2.zero;

        float randForce = Random.Range(bossCharacter.BounceForce - bossCharacter.RandBounceRange,
                                                   bossCharacter.BounceForce + bossCharacter.RandBounceRange);

        bossRB.AddForce(new Vector2(0f, randForce), ForceMode2D.Impulse);
    }

    public override void Exit()
    {
        base.Exit();

        trump.OnTriggerEnterEvent -= OnTrumpTriggered;
        trump.ClearEventData();
        trump = null;

        bossRB.constraints = RigidbodyConstraints2D.None;
        bossRB.constraints = RigidbodyConstraints2D.FreezeRotation;

        bossRB.bodyType = RigidbodyType2D.Kinematic;
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

    private void OnTrumpTriggered(Collider2D collision)
    {
        if (trump.destroyed)
        {
            // Cambio stato
            stateMachine.SetState(new LBPanic(bossCharacter, trump));

            return;
        }

        Jump();
    }
}
