using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
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

    private bool damage = false;
    private bool slow = false;
    private bool debilitate = false;

    public void Initialize(float expireTime,float tikPerSecond, float radius, bool damage, bool slow, bool debilitate)
    {
        this.expireTime = expireTime;
        this.tikPerSecond = tikPerSecond;
        this.radius = radius;
        this.damage = damage;
        this.slow = slow;
        this.debilitate = debilitate;

        characterInArea = new List<Character>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Character>())
            characterInArea.Add(other.gameObject.GetComponent<Character>());

        if (other.gameObject.GetComponent<Character>() is PlayerCharacter)
        {
            //regene amici
            DotEffect regeneEffect = other.gameObject.AddComponent<DotEffect>();
            regeneEffect.ApplyDOT(-healPerTik, tikPerSecond);

            statusEffectApplied.Add(regeneEffect);
        }


        //EnemyCharacter al posto di playerCharacter
        if (other.gameObject.GetComponent<Character>() is PlayerCharacter)
        {
            //danneggia nemici
            if(damage)
            {
                DotEffect dotEffect = other.gameObject.AddComponent<DotEffect>();
                dotEffect.ApplyDOT(DOTPerTik, tikPerSecond);

                statusEffectApplied.Add(dotEffect);
            }

            //rallenta nemici
            if(slow)
            {
                other.gameObject.GetComponent<Character>().AddPowerUp(slowDown);
            }

            //indebolisci nemici
            if (debilitate)
            {

            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (characterInArea.Contains(other.gameObject.GetComponent<Character>()))
        {
            if (other.gameObject.GetComponent<DotEffect>())
                RemoveEffects(other.gameObject.GetComponent<Character>());

            characterInArea.Remove(other.gameObject.GetComponent<Character>());
        }
        //Deregistrati a lista character
    }

    private void RemoveEffects(Character character)
    {
        foreach (StatusEffectBehaviour effect in character.gameObject.GetComponents<StatusEffectBehaviour>()) 
        {
            if(statusEffectApplied.Contains(effect))
                effect.RemoveDOT();

            statusEffectApplied.Remove(effect);
        }

        character.RemovePowerUp(slowDown);
    }

    private void Start()
    {
        transform.localScale=new Vector3(radius,radius,radius);
    }


    private void Update()
    {
        ExpireCountdown();
    }

    private void ExpireCountdown()
    {
        if (timer >= expireTime)
        {
            foreach (Character character in characterInArea)
            {
                RemoveEffects(character);
            }

            Destroy(gameObject);

            timer = 0;
        }

        timer += Time.deltaTime;
    }
}
