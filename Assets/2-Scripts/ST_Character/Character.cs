using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IDamageable, IDamager, IInteracter
{
  
    [Header("General VFX")]
    [SerializeField] protected ParticleSystem _walkDustParticles;
    [SerializeField] protected GameObject _hitParticlesObject;
    [SerializeField] private float fadeSpeedOnHit = 2f;
    [SerializeField] protected Color _OnHitColor = Color.red;

    private Color _materialTintColor;
    private Material spriteMaterial;
    private SpriteRenderer spriteRendererVisual;

    [Header("Character Generics")]
    [HideInInspector] public bool stunned = false;
    [HideInInspector] public bool underAggro = false;
    [HideInInspector] public bool inLove = false;
    [HideInInspector] public bool bleeding = false;

    public bool isDead = false;
    protected Rigidbody2D rb;
    [SerializeField] protected List<Collider2D> colliders ;
    protected List<Condition> conditions;

    protected bool canInteract;
    protected IInteractable activeInteractable;

    [HideInInspector]
    public float damageReceivedMultiplier = 1;

    public Transform dealerTransform => transform;

    [SerializeField] protected UnityEvent onHit = new();
    [SerializeField] protected UnityEvent onDeath = new();
    [SerializeField] protected UnityEvent onDefenceAbility = new();
    [SerializeField] protected UnityEvent onParried = new();
   


    public UnityEvent OnDeath { get => onDeath; set => onDeath = value; } 
    public UnityEvent OnHit { get => onHit; set => onHit = value; } 

    public UnityEvent OnDefenceAbility { get => onDefenceAbility; set => onDefenceAbility = value; } 


    [SerializeField] private AnimationCurve pushAnimationCurve;
    [SerializeField] protected SoundsDatabase soundsDatabase;

    #region RumbleVars

    [Header("Rumble Data")]
    [SerializeField, ReorderableList]
    List<RumbleData> rumbleData;

    #endregion

    //Lo uso per chimare tutte le funzioni iniziali
    protected virtual void Awake()
    {
        InitialSetup();
    }

    //Tutto ciò che va fatto nello ad inizio
    protected virtual void InitialSetup()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        conditions = new();
        canInteract = false;
        damageReceivedMultiplier = 1;

        //Shader
        spriteRendererVisual = GetComponentInChildren<SpriteRotation>().GetComponent<SpriteRenderer>();
        spriteMaterial = spriteRendererVisual.material;
    }

    public Rigidbody2D GetRigidBody() => rb;


    public virtual IEnumerator PushCharacter(Vector3 pusherPosition, float pushStrenght, float pushDuration)
    {
        float timer = 0;
        float interpolationRatio;
        Vector3 startPosition = rb.transform.position;

        Vector3 pushDirection = (startPosition - pusherPosition).normalized;

        while (timer < pushDuration)
        {
            interpolationRatio = timer / 1;
            timer += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPosition, startPosition + (pushDirection * pushStrenght), pushAnimationCurve.Evaluate(interpolationRatio)));
            yield return new WaitForFixedUpdate();
        }
    }

    #region Rumble

    public void RumbleAllPads(string rumbleName)
    {
        RumbleData dataFound = GetRumbleData(rumbleName);

        if (dataFound == null) return;

        foreach (PlayerInputHandler handler in CoopManager.Instance.GetActiveHandlers())
        {
            handler.RumblePulse(dataFound);
        }
    }

    protected RumbleData GetRumbleData(string rumbleName)
    {
        RumbleData result = null;

        foreach (RumbleData data in rumbleData)
        {
            if (data.rumbleName == rumbleName)
            {
                result = data;
                return result;
            }
        }

        return result;
    }

    public void StopAllRumblePads(string rumbleName)
    {
        RumbleData dataFound = GetRumbleData(rumbleName);

        if (dataFound == null) return;

        foreach (PlayerInputHandler handler in CoopManager.Instance.GetActiveHandlers())
        {
            handler.RumbleStop(dataFound);
        }
    }

    #endregion

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
    protected virtual void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canInteract)
                InteractWith(activeInteractable);
        }
        if (context.canceled)
        {
            if (canInteract)
                AbortInteraction(activeInteractable);
        }
    }

    public void CancelInteraction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canInteract)
                CancelInteract(activeInteractable);
        }
    }

    public void InteractWith(IInteractable interactable)
    {
        activeInteractable.Interact(this);
    }

    public void AbortInteraction(IInteractable interactable)
    {
        activeInteractable.AbortInteraction(this);
    }

    public void CancelInteract(IInteractable interactable)
    {
        activeInteractable.CancelInteraction(this);
    }

    public void EnableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = true;
    }

    public virtual void DisableInteraction(IInteractable interactable)
    {
        activeInteractable = interactable;
        canInteract = false;
    }

    public virtual void DisableOtherActions()
    {

    }

    public virtual void EnableAllActions()
    {

    }


    public GameObject GetInteracterObject()
    {
        return gameObject;
    }

    public virtual void OnParryNotify(Character whoParried)
    {

    }


    #endregion

    #region shader
    //Shader
    public void SetHitMaterialColor(Color newColor)
    {
        _materialTintColor = newColor;
        spriteMaterial.SetColor("_Tint", _materialTintColor);
        StartCoroutine(StartShaderFade());
    }

    public IEnumerator StartShaderFade()
    {
        while (_materialTintColor.a > 0)
        {
            _materialTintColor.a = Mathf.Clamp01(_materialTintColor.a - fadeSpeedOnHit * Time.deltaTime);
            spriteMaterial.SetColor("_Tint", _materialTintColor);

            yield return new WaitForEndOfFrame();
        }
    }

  



    #endregion
    #region Damage


    public abstract void TakeDamage(DamageData data);

    public abstract DamageData GetDamageData();



    #endregion

    #region Audio

    public virtual void OnAttackSound(int databaseClipNumber)
    {
        AudioManager.Instance.PlayAudioClip(soundsDatabase.attackSounds[databaseClipNumber], transform);
    }    
    public virtual void OnBlockSound()
    {
        AudioManager.Instance.PlayAudioClip(soundsDatabase.blockSounds[0], transform);
    }
    public virtual void OnDodgeSound()
    {
        AudioManager.Instance.PlayAudioClip(soundsDatabase.dodgeSounds[0], transform);
    }
    public virtual void OnWalkSound()
    {
        AudioManager.Instance.PlayRandomAudioClip(soundsDatabase.walkSounds, transform);
    }
    #endregion

   
}




