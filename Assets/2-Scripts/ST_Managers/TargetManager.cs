using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private static TargetManager _instance;
    public static TargetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TargetManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("TargetManager");
                    _instance = singletonObject.AddComponent<TargetManager>();
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private List<EnemyCharacter> enemyInScene = new List<EnemyCharacter>();

    public void AddEnemy(EnemyCharacter enemy)
    {
        enemyInScene.Add(enemy);
    }

    public void RemoveEnemy(EnemyCharacter enemy)
    {
        enemyInScene.Remove(enemy);
    }

    public void ChangeTarget(PlayerCharacter oldTarget, PlayerCharacter newTarget)
    {
        foreach (EnemyCharacter enemy in enemyInScene)
        {
            if (enemy.Target != null)
                if (enemy.Target.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
                {
                    if (player == oldTarget)
                        enemy.SetTarget(newTarget.transform);
                }
        }
    }

    public void ChangeTarget(PlayerCharacter diedCharacter)
    {
        foreach (EnemyCharacter enemy in enemyInScene)
        {
            if (enemy.Target != null && enemy.Target == diedCharacter.transform)
            {
                enemy.TargetSelection();
            }
        }
    }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }




}
