using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LBProjectile : MonoBehaviour, IDamager
{
    [SerializeField]
    private float aliveTime = 2f;

    // Da inserire boss transform o proiettile transform?
    public Transform dealerTransform => bossDealer.gameObject.transform;


    private float timer = 0f;

    private BossCharacter bossDealer;

    public DamageData GetDamageData()
    {
        // é giusto così?
        return new DamageData(bossDealer.Damage, bossDealer.StaminaDamage, this, true);
    }

    internal void Initialize(BossCharacter boss)
    {
        bossDealer = boss;
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return;

        timer += Time.deltaTime;

        if (timer > aliveTime)
        {
            gameObject.SetActive(false);
            timer = 0f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerCharacter charHit = collision.GetComponent<PlayerCharacter>();

        if (charHit == null) return;

        if (PlayerCharacterPoolManager.Instance.ActivePlayerCharacters.Contains(charHit))
        {
            charHit.TakeDamage(GetDamageData());
        }
    }

    public void OnParryNotify(Character whoParried)
    {
        throw new System.NotImplementedException();
    }
}
