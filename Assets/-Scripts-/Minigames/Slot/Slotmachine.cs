using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slotmachine : MonoBehaviour
{
    [Header("Variabili colonna")]   
    
    [SerializeField,Tooltip("Numero delle figure totali nella colonna")]
    int numberOfSlots;
    [SerializeField,Tooltip("Numero delle figure vincenti nella colonna")]
    int numberWinSlots;
    [SerializeField,Tooltip("Distanza delle figure nella colonna")] 
    float slotDistance = 0.25f;
    [SerializeField,Tooltip("Velocità di rotazione della colonna")] 
    float rotationSpeed;
    [SerializeField,Tooltip("Velocità/tempo impiegato alla colonna per fermarsi")] 
    float stabilizationSpeed;

    [Header("Sprite figure")]
    [SerializeField] private Sprite dpsSprite;
    [SerializeField] private Sprite tankSprite;
    [SerializeField] private Sprite rangedSprite;
    [SerializeField] private Sprite healerSprite;
    [SerializeField] private Sprite enemySprite;

    [Header("Colonne")]

    [SerializeField] private List<SlotRow> rows;

    bool win;

    private void Awake()
    {
        foreach(SlotRow row in rows)
        {
            row.SetRow(numberOfSlots,numberWinSlots,slotDistance,dpsSprite,enemySprite,rotationSpeed,stabilizationSpeed);
        }
    }
    private void Update()
    {
        //TODO: inserire input manuali per debug
    }

    private void CheckForWin()
    {
        win = true;

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
            CheckForWin();
        }
    }
}
