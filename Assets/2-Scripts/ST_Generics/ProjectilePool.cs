using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{

    private static ProjectilePool _instance;
    public static ProjectilePool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ProjectilePool>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("ProjectilePool");
                    _instance = singletonObject.AddComponent<ProjectilePool>();
                }
            }

            return _instance;
        }
    }

    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int poolSize = 5;

    Stack<Projectile> _projectilePool;

    private void Awake()
    {
        _projectilePool = new Stack<Projectile>();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        for(int i = 0; i < poolSize; i++)
        {
            CreateProjectile();
        }
    }

    private void CreateProjectile()
    {
        Projectile newProjectile = Instantiate(projectilePrefab);
        newProjectile.gameObject.SetActive(false);
        _projectilePool.Push(newProjectile);
        newProjectile.transform.parent = transform;
    }

    public Projectile GetProjectile()
    {
        if(_projectilePool.Count == 0)
        {
            CreateProjectile();
        }

        Projectile projectile = _projectilePool.Pop();
        projectile.gameObject.SetActive(true);
        projectile.transform.parent = null;

        return projectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        _projectilePool.Push(projectile);
        projectile.gameObject.SetActive(false);
        projectile.transform.parent = transform;
    }

    
}
