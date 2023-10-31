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
    
    [SerializeField] float healPerTik = 1;

    [SerializeField] float DOTPerTik = 1;
    
    [SerializeField] float speedPenalty = -1;
    
    [SerializeField] float damageIncrement = 1;


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
        if (other.gameObject.GetComponent<Character>() is PlayerCharacter)
        {
            characterInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());


            //regene amici
            DotEffect regeneEffect = other.gameObject.AddComponent<DotEffect>();
            regeneEffect.ApplyDOT(other.gameObject.GetComponent<PlayerCharacter>(), -healPerTik, tikPerSecond);

            statusEffectApplied.Add(regeneEffect);
        }


        //EnemyCharacter al posto di character
        if (other.gameObject.GetComponent<Character>() is PlayerCharacter)
        {
            characterInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());

            //danneggia nemici
            if(damage)
            {
                DotEffect dotEffect = other.gameObject.AddComponent<DotEffect>();
                dotEffect.ApplyDOT(other.gameObject.GetComponent<PlayerCharacter>(), DOTPerTik, tikPerSecond);

                statusEffectApplied.Add(dotEffect);
            }

            //rallenta nemici
            if(slow)
            {

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
        

        if(character.gameObject.GetComponent<DotEffect>())
            character.gameObject.GetComponent<DotEffect>().RemoveDOT();
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
