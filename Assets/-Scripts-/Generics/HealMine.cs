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
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerCharacter>() != null)
        {
            characterInArea.Add(collision.GetComponent<PlayerCharacter>());

            if (spawner.GetComponent<Healer>() == collision.GetComponentInChildren<Healer>())
                spawner.GetComponent<Healer>().SetMineIcon(true, null);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        characterInArea.Remove(collision.GetComponent<PlayerCharacter>());

        if (spawner.GetComponent<Healer>() == collision.GetComponentInChildren<Healer>())
            spawner.GetComponent<Healer>().SetMineIcon(false, null);
    }


    private void Start()
    {
        GetComponent<CapsuleCollider2D>().size = new Vector2(radius,radius/2);
    }
    
    private void Update()
    {
        if (timer >= activationTime)
        {
            if (characterInArea.Count >= 1)
            {
                foreach (PlayerCharacter character in characterInArea)
                {
                    //DA RIVEDERE #MODIFICATO
                    //character.CharacterClass.currentHp += heal;
                }

                if (spawner.GetComponent<Healer>() != null)
                    spawner.GetComponent<Healer>().SetMineIcon(false, null);

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
