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

    private void Start()
    {
        slots = new List<GameObject>();
        for (int i = 0; i < numberOfSlots; i++)
        {
            GameObject slot = new GameObject($"slot #{i}");
            slot.transform.SetParent(gameObject.transform, true);

            slot.AddComponent<Slot>().Sprite=mouseSprite;
  
            
            
            if (slots.Count==0)
            {
                slot.transform.position= gameObject.transform.position;
                slots.Add(slot);
                //inserire varianti
            }
            else
            {
                GameObject previousSlot = slots[slots.Count - 1];
                slot.transform.position=new Vector2(previousSlot.transform.position.x,previousSlot.transform.position.y-slotDistance);
                slots.Add(slot);
                //inserire varianti
            }
        }
    }
}
