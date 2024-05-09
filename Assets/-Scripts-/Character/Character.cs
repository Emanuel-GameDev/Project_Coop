using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public abstract class Character : MonoBehaviour, IDamageable, IDamager, IInteracter
{

    //shader
    [Header("ProvaShaderGraph Hit e Parry")]
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] protected Color _OnHitColor = Color.red;
    [SerializeField] protected Color _OnParryColor = Color.yellow;

    [Header("General VFX")]
    [SerializeField] protected ParticleSystem _walkDustParticles;
    
    

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
    protected List<Condition> conditions;

    private bool canInteract;
    private IInteractable activeInteractable;

    [HideInInspector]
    public float damageReceivedMultiplier = 1;

    public Transform dealerTransform => transform;

    [SerializeField] protected UnityEvent onHit = new();
    [SerializeField] protected UnityEvent onDeath = new();
    [SerializeField] protected UnityEvent onDash = new();
    [SerializeField] protected UnityEvent onParried = new();


    public UnityEvent OnDeath { get => onDeath; set => onDeath = value; } 
    public UnityEvent OnHit { get => onHit; set => onHit = value; } 

    public UnityEvent OnDash { get => onDash; set => onDash = value; } 


    [SerializeField] private AnimationCurve pushAnimationCurve;
    [SerializeField] protected SoundsDatabase soundsDatabase;

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


    public IEnumerator PushCharacter(Vector3 pusherPosizion, float pushStrenght, float pushDuration)
    {
        float timer = 0;
        float interpolationRatio;
        Vector3 startPosition = rb.transform.position;

        Vector3 pushDirection = (startPosition - pusherPosizion).normalized;

        while (timer < pushDuration)
        {
            interpolationRatio = timer / 1;
            timer += Time.deltaTime;
            rb.MovePosition(Vector3.Lerp(startPosition, startPosition + (pushDirection * pushStrenght), pushAnimationCurve.Evaluate(interpolationRatio)));
            yield return new WaitForFixedUpdate();
        }
    }


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

    public void DisableInteraction(IInteractable interactable)
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
            _materialTintColor.a = Mathf.Clamp01(_materialTintColor.a - fadeSpeed * Time.deltaTime);
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




