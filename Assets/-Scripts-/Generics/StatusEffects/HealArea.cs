using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

public class HealArea : MonoBehaviour
{
    [SerializeField] float expireTime = 10;

    [SerializeField] float radius = 1;
    [SerializeField] float tikPerSecond = 1;
    
    [SerializeField] public float healPerTik = 1;

    [SerializeField] public float DOTPerTik = 1;
    
    [SerializeField] public float damageIncrementPercentage = 1;
    [SerializeField] public PowerUp slowDown;


    private List<Character> characterInArea;
    
    float timer = 0;
    float DOTTimer = 0;
    float countdown = 1;

    private bool damage = false;
    private bool slow = false;
    private bool debilitate = false;


    public void Initialize(GameObject spawner, float expireTime,float tikPerSecond, float radius, bool damage, bool slow, bool debilitate)
    {
        this.expireTime = expireTime;
        this.tikPerSecond = tikPerSecond;
        this.radius = radius;
        this.damage = damage;
        this.slow = slow;
        this.debilitate = debilitate;

        characterInArea = new List<Character>();

        if (spawner != null && spawner.GetComponentInParent<PlayerCharacter>() != null)
        {
            transform.SetParent(spawner.transform);
            characterInArea.Add(spawner.GetComponentInParent<PlayerCharacter>());

        }

        countdown = 1 / tikPerSecond;
        transform.localScale = new Vector3(radius, radius/2, radius);
        DOTTimer = countdown;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Character>())
        {
            characterInArea.Add(other.gameObject.GetComponent<Character>());

            //Sostituire character con enemycharacter
            if (slow && other.gameObject.GetComponent<Character>() is EnemyCharacter)
            {
                other.gameObject.GetComponent<Character>().AddPowerUp(slowDown);
            }

            //indebolisci nemici
            if (debilitate && other.gameObject.GetComponent<Character>() is EnemyCharacter)
            {
                other.gameObject.GetComponent<Character>().damageReceivedMultiplier = damageIncrementPercentage;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (characterInArea.Contains(other.gameObject.GetComponent<Character>()))
        {
            characterInArea.Remove(other.gameObject.GetComponent<Character>());
            other.gameObject.GetComponent<Character>().damageReceivedMultiplier = 1f;
            other.gameObject.GetComponent<Character>().RemovePowerUp(slowDown);
        }
        //Deregistrati a lista character
    }

    public void ApplyDOT()
    {
        foreach (Character c in characterInArea)
        {
            //regene amici
            if (c is PlayerCharacter friends)
            {
                friends.CharacterClass.currentHp += healPerTik;
            }

            //EnemyCharacter al posto di dummy
            if (c is EnemyCharacter)
            {
                //danneggia nemici
                if (damage)
                {
                    c.TakeDamage(new DamageData(DOTPerTik, null));
                }
            }
        }
    }



    private void Update()
    {
        ElapseDOTTimer();
        ExpireCountdown();
    }

    private void ElapseDOTTimer()
    {
        if (DOTTimer >= countdown)
        {
            ApplyDOT();
            DOTTimer = 0;
        }

        DOTTimer += Time.deltaTime;
    }

    private void ExpireCountdown()
    {
        if (timer >= expireTime)
        {
            foreach(Character c in characterInArea)
            {
                c.damageReceivedMultiplier = 1f;
                c.RemovePowerUp(slowDown);
            }
            Destroy(gameObject);

            timer = 0;
        }

        timer += Time.deltaTime;
    }

    
}
