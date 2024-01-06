using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotRow : MonoBehaviour
{
    [SerializeField] int numberOfSlots;
    [SerializeField] int numberWinSlots;
    [SerializeField] float slotDistance = 0.25f;

    [SerializeField] private Sprite playerSprite;
    [SerializeField] private Sprite mouseSprite;

    [SerializeField] private List<GameObject> slots;

    [SerializeField] bool stopped;

    [SerializeField] float rotationSpeed;

    private Vector3 finalRowRotation;
    private Vector3 startRowRotation;

    private void Start()
    {
        stopped = true;
        slots = new List<GameObject>();
        InitializeRow();
    }

    private void Update()
    {
        RotateRow();
    }

    private void InitializeRow()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = new GameObject($"slot #{i}");
            slot.transform.SetParent(gameObject.transform, true);

            slot.AddComponent<Slot>();
            slot.GetComponent<Slot>().Sprite = mouseSprite;




            if (slots.Count == 0)
            {
                slot.transform.position = gameObject.transform.position;
                slots.Add(slot);
                //inserire varianti
            }
            else
            {
                GameObject previousSlot = slots[slots.Count - 1];
                slot.transform.position = new Vector2(previousSlot.transform.position.x, previousSlot.transform.position.y - slotDistance);
                slots.Add(slot);
                //inserire varianti
            }
        }
        Vector3 firstSlot = slots[0].transform.localPosition;
        startRowRotation = new Vector3(firstSlot.x, firstSlot.y-(slotDistance/2) + firstSlot.z);

        Debug.Log(startRowRotation.y);

        Vector3 lastSlot = slots[slots.Count - 1].transform.localPosition;
        finalRowRotation=new Vector3(lastSlot.x, lastSlot.y-(slotDistance/2), lastSlot.z);

        Debug.Log(finalRowRotation.y);
    }

    public void RotateRow()
    {
        stopped = false;

        if (!stopped)
        {
            transform.localPosition += Vector3.up * Time.deltaTime * rotationSpeed;

            if (transform.localPosition.y >= -finalRowRotation.y)
            {
                transform.localPosition = new Vector3(transform.position.x,startRowRotation.y);
                
            }
        }
        
    }
}
