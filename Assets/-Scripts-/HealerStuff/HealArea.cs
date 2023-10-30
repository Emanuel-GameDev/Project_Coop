using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HealArea : MonoBehaviour
{

    private float healPerSecond = 1;
    private float radius = 1;
    private float expireTime = 10;

    private bool damage=false;
    private bool slow = false;
    private bool debilitate = false;

    private List<Character> characterInArea;

    public DotEffect dotEffect;

    public void Initialize(float healPerSecond, float radius, bool damage, bool slow, bool debilitate)
    {
        this.healPerSecond = healPerSecond;
        this.radius = radius;
        this.damage = damage;
        this.slow = slow;
        this.debilitate = debilitate;

        characterInArea=new List<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Character>() is Character)
        {
            characterInArea.Add(other.gameObject.GetComponent<Character>());
            dotEffect.CharacterDOTApply(other.gameObject.GetComponent<Character>(), 1);
        }

        //if (other.gameObject.GetComponent<Character>() && !characterInArea.Contains(other.gameObject.GetComponent<Character>()))
        //{
        //    characterInArea.Add(other.gameObject.GetComponent<Character>());
        //}
        //Registrati a lista character
    }

    private void OnTriggerExit(Collider other)
    {
        if (characterInArea.Contains( other.gameObject.GetComponent<Character>()))
        {
            characterInArea.Remove(other.gameObject.GetComponent<Character>());
            RemoveEffect(other.gameObject.GetComponent<Character>());
           
        }
        //Deregistrati a lista character
    }

    private void RemoveEffect(Character character)
    {
        
        dotEffect.CharacterDOTRemove(character, 1);
    }

    private void Start()
    {
        StartCoroutine(ExpireCooldown());

        transform.localScale=new Vector3(radius,radius,radius);
    }

    private void Update()
    {
        //foreach (Character character in characterInArea)
        //{
        //    if(character is PlayerCharacter)
        //    {
        //        Debug.Log("Cura");
        //    }
        //    //Cura

        //    //Da cambiare con enemyCharacter
        //    if(character is Character) 
        //    {
        //        if(damage)
        //        {
        //            Debug.Log("danneggia");
        //            //Danneggia
        //        }

        //        if(slow)
        //        {
        //            Debug.Log("slow");
        //            //Rallenta
        //        }

        //        if(debilitate)
        //        {
        //            Debug.Log("debilita");
        //            //Indebolisci
        //        }
        //    }
        //}
    }

    IEnumerator ExpireCooldown()
    {
        yield return new WaitForSeconds(expireTime);

        foreach (Character character in characterInArea)
        {
            RemoveEffect (character);
        }
        Destroy(gameObject);
    }
}
