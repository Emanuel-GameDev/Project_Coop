using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
   public static ProjectilePool Instance { get; private set; }

    [SerializeField] Projectile projectilePrefab;
    [SerializeField] int poolSize = 5;

    Stack<Projectile> _projectilePool;

    private void Awake()
    {
        _projectilePool = new Stack<Projectile>();
    }

    private void Start()
    {
        if(Instance== null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

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
    }

    public Projectile GetProjectile()
    {
        if(_projectilePool.Count == 0)
        {
            CreateProjectile();
        }

        Projectile projectile = _projectilePool.Pop();
        projectile.gameObject.SetActive(true);

        return projectile;
    }

    public void ReturnProjectile(Projectile projectile)
    {
        _projectilePool.Push(projectile);
        projectile.gameObject.SetActive(false);
    }

    
}
