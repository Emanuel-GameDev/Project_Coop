using UnityEngine;

public class LandMine : MonoBehaviour, IDamager
{
    [SerializeField] Character owner;

    [SerializeField] float landMineDamage = 0;

    [SerializeField] Pickable pickable;

    public Transform dealerTransform => transform;

    private void Awake()
    {
        pickable=GetComponentInChildren<Pickable>();
    }

    private void Update()
    {

    }

    public void Initialize(Character owner, float radius, float damage, LayerMask layer)
    {
        this.owner = owner;
        GetComponent<SphereCollider>().radius = radius;
        this.landMineDamage = damage;
        gameObject.layer = layer;

        pickable.SetCharacter(owner);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ranged sniper = other.gameObject.GetComponentInChildren<Ranged>();       

        if (other.gameObject.GetComponent<Character>() != null && other.gameObject.layer != gameObject.layer)
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



    private void OnTriggerExit2D(Collider2D other)
    {
        
    }

    public void RemoveInRange()
    {
        owner.GetComponentInChildren<Ranged>().nearbyLandmine.Remove(this);
    }

    public void AddInRange()
    {
        owner.GetComponentInChildren<Ranged>().nearbyLandmine.Add(this);
    }

    //public float GetDamage()
    //{
    //    return landMineDamage;
    //}

    //modifica
    public DamageData GetDamageData()
    {
        return new DamageData(landMineDamage, this);
    }

    public void OnParryNotify(Character whoParried)
    {
        throw new System.NotImplementedException();
    }
}
