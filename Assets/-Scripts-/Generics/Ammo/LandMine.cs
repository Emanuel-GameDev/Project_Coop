using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandMine : MonoBehaviour,IDamager
{
    [SerializeField] GameObject owner;

    private void Update()
    {
        DebugManualExplosion();
    }

    public void Initialize(GameObject owner)
    {
        this.owner = owner;
    }

    public void PickUpLandmine()
    {

        owner.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);

        owner.GetComponentInChildren<Ranged>().RecoverLandMine();
       
        Destroy(gameObject);
    }

    private void DebugManualExplosion()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(owner!=null)
            {
                if (owner.GetComponentInChildren<Ranged>() != null)
                {
                    owner.GetComponentInChildren<Ranged>().RegenerateLandMine();
                }
            }
            

            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInChildren<Ranged>() != null)
        {
            if (other.gameObject.GetComponentInChildren<Ranged>().gameObject == owner && other.gameObject.GetComponentInChildren<Ranged>() != null)
            {
                owner.GetComponentInChildren<Ranged>().nearbyLandmine.Add(this);
            }
        }
        

        //if (qualcosa){ boom boom }
               
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetComponentInChildren<Ranged>() != null)
        {
            if (other.gameObject.GetComponentInChildren<Ranged>().gameObject == owner )
            {
                owner.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);
            }
        }
       
    }

    public float GetDamage()
    {
        throw new System.NotImplementedException();
    }
}
