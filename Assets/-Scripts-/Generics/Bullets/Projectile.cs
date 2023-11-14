using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 travelDirection;
    [SerializeField] private float projectileSpeed;
    //[SerializeField] private float lifetime=5f;
    private float rangeRemaining=1;
    private Vector3 projectileSize=Vector3.one;

    Rigidbody rb;



    private void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        transform.LookAt(travelDirection);
        
    }

    public void Inizialize(Vector3 direction,float range,float speed,float sizeMultiplier)
    {
        travelDirection = direction*1000;
        rangeRemaining = range;
        projectileSpeed = speed;
        transform.localScale = projectileSize*sizeMultiplier;
    }

    //private void OnEnable()
    //{
    //    rangeRemaining = lifetime;
    //}

    private void OnDisable()
    {
        //effetti vari
    }

    private void FixedUpdate()
    {
        ProjectileFlyDirection();
        ProjectileLiveTimer();
    }

    //il tempo di vita del proiettile in base alla distanza
    private void ProjectileLiveTimer()
    {
        rangeRemaining -= Time.deltaTime*projectileSpeed;

        //Debug.Log(rangeRemaining);

        if (rangeRemaining <= 0)
        {
            ProjectilePool.Instance.ReturnProjectile(this);
        }
    }

    private void ProjectileFlyDirection()
    {
        transform.position = Vector3.MoveTowards(transform.position, travelDirection, projectileSpeed * Time.deltaTime);
    }

   
}
