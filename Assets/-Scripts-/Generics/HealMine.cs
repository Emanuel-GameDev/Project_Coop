using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealMine : MonoBehaviour
{

    [SerializeField] float heal = 10;
    [SerializeField] public  float radius = 2;
    [SerializeField] public float activationTime = 1;

    float timer = 0;

    GameObject spawner;

    private List<PlayerCharacter> characterInArea;


    private void Awake()
    {
        characterInArea = new List<PlayerCharacter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerCharacter>() != null)
        {
            characterInArea.Add(other.GetComponent<PlayerCharacter>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        characterInArea.Remove(other.GetComponent<PlayerCharacter>());
    }


    private void Start()
    {
        GetComponent<CapsuleCollider>().radius = radius;
    }
    
    private void Update()
    {
        if (timer >= activationTime)
        {
            if (characterInArea.Count >= 1)
            {
                foreach (PlayerCharacter character in characterInArea)
                {
                    character.CharacterClass.currentHp += heal;
                }

                if (spawner.GetComponentInChildren<Healer>() != null)
                    spawner.GetComponentInChildren<Healer>().SetMineIcon(false, null);

                Destroy(gameObject);
            }
        }
        else
            timer += Time.deltaTime;
        
    }

    public void Initialize(GameObject spawner, float heal,float radius,float activationTime)
    {
        this.spawner = spawner;
        this.heal= heal;
        this.radius= radius;
        this.activationTime= activationTime;
    }
}
