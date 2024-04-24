using UnityEngine;

public class Projectile : MonoBehaviour, IDamager
{
    [Header("Proiettile statistiche base")]
    [SerializeField] private Vector2 travelDirection;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rangeRemaining = 1;
    [SerializeField] private float maxRange = 1;
    [SerializeField] private Vector3 projectileSize;
    [SerializeField] private float baseProjectileDamage;
    [SerializeField] private float boostedProjectileDamage;

    //valore utilizzare unicamente nel reflect per tenere conto della grandezza di base
    private float sizeMultiplier;

    [Header("Proprietà danno incrementale")]
    [SerializeField] bool incrementalDamage;
    [Min(0)]
    [SerializeField] float zone1Distance;
    [Min(1)]
    [SerializeField] float zone1DamageMultiplier;
    [Min(0)]
    [SerializeField] float zone2Distance;
    [Min(1)]
    [SerializeField] float zone2DamageMultiplier;
    [Min(0)]
    [SerializeField] float zone3Distance;
    [Min(1)]
    [SerializeField] float zone3DamageMultiplier;
    //[Min(0)]
    //[SerializeField] float zone4Distance;
    [Min(1)]
    [SerializeField] float maxZoneDamageMultiplier;



    private Damager damager;

    Rigidbody rb;

    public Transform dealerTransform => transform;

    private void Awake()
    {
        projectileSize = transform.lossyScale;
        damager = GetComponent<Damager>();

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //transform.LookAt(travelDirection);
        transform.position = new Vector3(transform.position.x, transform.position.y, -1);
        
    }

    public void Inizialize(Vector2 direction, float range, float speed, float sizeMultiplier,float damage,LayerMask layer)
    {
        

        travelDirection = direction * 1000;
        maxRange = range;
        rangeRemaining = maxRange;
        projectileSpeed = speed;
        transform.localScale = projectileSize * sizeMultiplier;
        this.sizeMultiplier = sizeMultiplier;
        baseProjectileDamage = damage;
        boostedProjectileDamage = damage;
        gameObject.layer = layer;

        transform.right = (Vector3)travelDirection - transform.position;
        

        //momentaneo
        transform.position=new Vector3(transform.position.x,transform.position.y, -1);

        //TODO cambiare sta roba asap
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            int Targetlayer= LayerMask.NameToLayer("Enemy");

            damager.targetLayers = (1 << Targetlayer);
        }
        else
        {
            int Targetlayer = LayerMask.NameToLayer("Player");
            damager.targetLayers = (1 << Targetlayer);
        }
    }

    private void OnEnable()
    {
        transform.localScale = projectileSize;
    }

    private void OnDisable()
    {
        //effetti vari
    }

    private void Update()
    {
        ProjectileFlyDirection();
        ProjectileLiveTimer();
    }

    //il tempo di vita del proiettile in base alla distanza
    private void ProjectileLiveTimer()
    {
        rangeRemaining -= Time.deltaTime * projectileSpeed;

        //Debug.Log(rangeRemaining);

        if (rangeRemaining <= 0)
        {
            DismissProjectile();
        }
    }

    public void DismissProjectile()
    {
        incrementalDamage = false;
        ProjectilePool.Instance.ReturnProjectile(this);

        
    }

    private void ProjectileFlyDirection()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, travelDirection, projectileSpeed * Time.deltaTime);
        
    }

    //public float GetDamage()
    //{
    //    DismissProjectile();

    //    return projectileDamage;
    //}


    //Modifica
    public DamageData GetDamageData()
    {
        
        if (incrementalDamage)
        {
            float projectileTraveled = maxRange - rangeRemaining;
            if(projectileTraveled <= zone1Distance)
            {
                boostedProjectileDamage *= zone1DamageMultiplier;
            }
            else if(projectileTraveled > zone1Distance && projectileTraveled<= zone2Distance)
            {
                boostedProjectileDamage *= zone2DamageMultiplier;
            }
            else if(projectileTraveled > zone2Distance && projectileTraveled <= zone3Distance)
            {
                boostedProjectileDamage *= zone3DamageMultiplier;
            }
            else
            {
                boostedProjectileDamage *= maxZoneDamageMultiplier;
            }
        }

        DismissProjectile();

        return new DamageData(boostedProjectileDamage,this);
    }

    public void ReflectProjectile(GameObject reflectionOwner,float reflectionMultiplier)
    {
        Projectile reflectedProjectile = ProjectilePool.Instance.GetProjectile();

        reflectedProjectile.Inizialize(-travelDirection,maxRange,projectileSpeed,sizeMultiplier,baseProjectileDamage*reflectionMultiplier,reflectionOwner.layer);

        DismissProjectile();
    }

    public void AddIncrementalDamage() 
    {
        incrementalDamage = true;
    }

    
    public void OnParryNotify(Character whoParried)
    {
        throw new System.NotImplementedException();
    }
}
