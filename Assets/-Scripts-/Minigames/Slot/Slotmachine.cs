using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slotmachine : MonoBehaviour
{
    [Header("Variabili colonna")]   
    
    [SerializeField,Tooltip("Numero delle figure totali nella colonna")]
    [Min(5)]
    int numberOfSlots=0;
    [SerializeField,Tooltip("Numero delle figure vincenti nella colonna")]
    int numberWinSlots=0;
    [SerializeField,Tooltip("Distanza delle figure nella colonna")] 
    float slotDistance = 0.25f;
    [SerializeField,Tooltip("Velocità di rotazione della colonna")] 
    float rotationSpeed=0;
    [SerializeField,Tooltip("Velocità/tempo impiegato alla colonna per fermarsi")] 
    float stabilizationSpeed = 0;
    [SerializeField, Tooltip("Delay restart della colonna")]
    float restartDelay=0;

    [Header("Sprite figure")]
    [SerializeField] private Sprite dpsSprite;
    [SerializeField] private Sprite tankSprite;
    [SerializeField] private Sprite rangedSprite;
    [SerializeField] private Sprite healerSprite;
    [SerializeField] private Sprite enemySprite;

    [Header("Colonne")]

    [SerializeField] private List<SlotRow> rows;

    bool canInteract;

    public GameObject GO;

    private void Awake()
    {
        foreach(SlotRow row in rows)
        {
            row.SetRow(numberOfSlots,numberWinSlots,slotDistance,dpsSprite,enemySprite,rotationSpeed,stabilizationSpeed);
        }

       // GameManager.Instance.coopManager.playerInputPrefab = GO;
    }

    private void Start()
    {
        canInteract = true;

        
    }

   

    private void Update()
    {
        //TODO: inserire input manuali per debug

        if (canInteract)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                rows[0].StartSlowDown();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                rows[1].StartSlowDown();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                rows[2].StartSlowDown();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                rows[3].StartSlowDown();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(RestartSlotMachine());
            }
        }
       

    }

    //rorrrorr
    private void CheckForWin()
    {
        bool win = true;

        foreach (SlotRow row in rows)
        {
            if(row.GetSelectedSlot().Type != slotType.Player)
            {
                win = false;
                break;
            }
        }

        if (win)
        {
            Debug.Log("avete vinto");
        }
        else
        {
            Debug.Log("avete perso");
        }
    }

    public GameObject GetLastRow()
    {
        return rows[rows.Count - 1].gameObject;
    }

    public void CheckAllRowStopped()
    {
        bool allStopped=true;

        foreach (SlotRow row in rows)
        {
            if(!row.stopped)
            {
                allStopped = false;
                break;
            }
        }

        if (allStopped)
        {
            canInteract = false;
            CheckForWin();
        }
    }

    public IEnumerator RestartSlotMachine()
    {
        foreach (SlotRow row in rows)
        {
            row.ResetRow();

            yield return new WaitForSeconds(restartDelay);
        }

        canInteract = true;
    }


}
