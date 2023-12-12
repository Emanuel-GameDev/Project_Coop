using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LandMine : MonoBehaviour, IDamager
{
    [SerializeField] Character owner;

    [SerializeField] float landMineDamage = 0;

    

    private void Update()
    {
        
    }

    public void Initialize(Character owner,float radius,float damage,LayerMask layer)
    {
        this.owner = owner;
        GetComponent<SphereCollider>().radius = radius;
        this.landMineDamage = damage;
        gameObject.layer = layer;
    }

    public void PickUpLandmine()
    {

        owner.gameObject.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);

        owner.gameObject.GetComponentInChildren<Ranged>().RecoverLandMine();
       
        Destroy(gameObject);
    }
    /*
    private void DebugManualExplosion()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(owner!=null)
            {
                if (owner.GetComponentInChildren<Ranged>() != null)
                {
                    owner.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);

                    owner.GetComponentInChildren<Ranged>().StartLandmineGeneration();
                }
            }
            

            Destroy(gameObject);
        }
    }*/

    private void OnTriggerEnter(Collider other)
    {
        Ranged sniper = other.gameObject.GetComponentInChildren<Ranged>();

        if (sniper != null)
        {
            if (sniper.gameObject.GetComponentInParent<Character>() == owner)
            {
                sniper.nearbyLandmine.Add(this);
            }
        }

        if(other.gameObject.GetComponent<Character>() != null && other.gameObject.layer!=gameObject.layer)
        {
            if (other.gameObject.layer != gameObject.layer)
            {
                //other.gameObject.GetComponent<Character>().TakeDamage(new DamageData(landMineDamage,this));
                if (owner != null)
                {
                    

                    if (sniper != null)
                    {
                        sniper.nearbyLandmine.Remove(this);

                        sniper.StartLandmineGeneration();
                    }
                }

                Destroy(gameObject);
            }
        }
        

       
               
    }

    private void OnTriggerExit(Collider other)
    {
        Ranged sniper = other.gameObject.GetComponentInChildren<Ranged>();

        if (sniper != null)
        {
            if (sniper.gameObject.GetComponentInParent<Character>() == owner )
            {
                owner.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);
            }
        }
       
    }

    public float GetDamage()
    {
        return landMineDamage;
    }
}
