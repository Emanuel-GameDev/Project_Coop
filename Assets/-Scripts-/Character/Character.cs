using MBT;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IDamageable, IDamager, IInteracter
{
    public bool stunned = false;

    protected Rigidbody rb;
    protected List<Condition> conditions;

    private bool canInteract;
    private IInteractable activeInteractable;

    [HideInInspector]
    public float damageReceivedMultiplier = 1;

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
        damageReceivedMultiplier = 1;
    }

    public Rigidbody GetRigidBody() => rb;

    public abstract void TakeDamage(DamageData data);

    public abstract DamageData GetDamageData();

    #region PowerUp & Conditions
    public abstract void AddPowerUp(PowerUp powerUp);
    public abstract void RemovePowerUp(PowerUp powerUp);
    public abstract List<PowerUp> GetPowerUpList();

    public virtual void AddToConditions(Condition condition)
    {
        conditions.Add(condition);
        condition.AddCondition(this);
    }
    public virtual void RemoveFromConditions(Condition condition)
    {
        conditions.Remove(condition);
    }
    #endregion

    #region InteractionSystem
    protected void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canInteract)
                InteractWith(activeInteractable);
        }
    }

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
    #endregion
}
