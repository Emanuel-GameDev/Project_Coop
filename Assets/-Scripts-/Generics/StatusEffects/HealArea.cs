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
    
    [SerializeField] public float damageIncrement = 1;
    [SerializeField] public PowerUp slowDown;

    [SerializeField] List<StatusEffectBehaviour> statusEffectApplied;

    private List<Character> characterInArea;
    
    float timer=0;
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
        transform.localScale = new Vector3(radius, radius, radius);
        DOTTimer = countdown;
    }

    //private void Awake()
    //{
    //    source ??= GetComponentInParent<IDamager>();
    //    source ??= GetComponent<IDamager>();
    //    source ??= GetComponentInChildren<IDamager>();
    //}


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Character>())
        {
            characterInArea.Add(other.gameObject.GetComponent<Character>());

            //Sostituire playercharacter con enemycharacter
            if (/*slow && */other.gameObject.GetComponent<Character>() is PlayerCharacter)
            {
                other.gameObject.GetComponent<PlayerCharacter>().AddPowerUp(slowDown);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (characterInArea.Contains(other.gameObject.GetComponent<Character>()))
        {
            characterInArea.Remove(other.gameObject.GetComponent<Character>());

            other.gameObject.GetComponent<Character>().RemovePowerUp(slowDown);
        }
        //Deregistrati a lista character
    }

    public void ApplyDOT()
    {
        foreach (Character c in characterInArea)
        {
            if (c is PlayerCharacter)
            {
                //regene amici
                c.TakeDamage(healPerTik, null);
            }


            //EnemyCharacter al posto di playerCharacter
            if (c is PlayerCharacter)
            {
                //danneggia nemici
                if (damage)
                {
                    c.TakeDamage(DOTPerTik, null);
                }

                ////rallenta nemici
                //if (slow)
                //{
                //    c.AddPowerUp(slowDown);
                //}

                ////indebolisci nemici
                //if (debilitate)
                //{

                //}
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
            Destroy(gameObject);

            timer = 0;
        }

        timer += Time.deltaTime;
    }

    
}
