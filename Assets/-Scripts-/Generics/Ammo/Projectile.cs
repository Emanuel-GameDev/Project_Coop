using UnityEngine;

public class Projectile : MonoBehaviour, IDamager
{
    [SerializeField] private Vector3 travelDirection;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float rangeRemaining = 1;
    [SerializeField] private float maxRange = 1;
    [SerializeField] private Vector3 projectileSize;
    [SerializeField] private float projectileDamage;

    [SerializeField] bool incrementalDamage;



    private Damager damager;

    Rigidbody rb;

    public Transform dealerTransform => transform;

    private void Awake()
    {
        projectileSize = transform.lossyScale;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.LookAt(travelDirection);
    }

    public void Inizialize(Vector3 direction, float range, float speed, float sizeMultiplier,float damage,LayerMask layer)
    {

        travelDirection = direction * 1000;
        maxRange = range;
        rangeRemaining = maxRange;
        projectileSpeed = speed;
        transform.localScale = projectileSize * sizeMultiplier;
        projectileDamage = damage;
        gameObject.layer = layer;
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
        DismissProjectile();

        return new DamageData(projectileDamage,this);
    }

}
