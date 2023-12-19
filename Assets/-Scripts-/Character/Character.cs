using MBT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour, IDamageable, IDamager, IInteracter
{


    protected List<Condition> conditions;
    public bool stunned = false;
    protected Rigidbody rb;
    private bool canInteract;
    private IInteractable activeInteractable;



    //Lo uso per chimare tutte le funzioni iniziali
    protected void Awake()
    {
        InitialSetup();
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponent<Rigidbody>();

        conditions = new();
        canInteract = false;
    }

    
    protected void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(canInteract)
                InteractWith(activeInteractable);
        }
    }

   
    public Rigidbody GetRigidBody() => rb;

    public virtual void TakeDamage(DamageData data) { }
    

    #region InteractionSystem
    public void InteractWith(IInteractable interactable)
    {
        activeInteractable.Interact(this);
    }

    public void EnableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = true;
    }

    public void DisableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = false;
    }

    //modifiche
    public virtual void AddPowerUp(PowerUp powerUp) { }
    public virtual void RemovePowerUp(PowerUp powerUp) { }
    public virtual List<PowerUp> GetPowerUpList() { return null; }
    //public float GetDamage() => characterClass.GetDamage();

    public virtual DamageData GetDamageData() { return null; }

    //fine modifiche

    public void AddToConditions(Condition condition)
    {
        conditions.Add(condition);
        condition.AddCondition(this);
    }

    public void RemoveFromConditions(Condition condition)
    {
        conditions.Remove(condition);

    }
    #endregion
}
