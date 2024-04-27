using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Ranged : PlayerCharacter
{
    //ci deve essere il riferimento alla look qua, non al proiettile
    //aggiungere statistiche personaggio + schivata+invincibilità
    //aggiungere prefab mina (?)
    //aggiungere vari timer(arma, schivata,cd vari)

    private Vector2 lookDirection = Vector2.up;
    private Vector2 ShootDirection = Vector2.up;

    


    //base Attack
    public override float AttackSpeed => base.AttackSpeed;
    public override float UniqueAbilityCooldown => base.UniqueAbilityCooldown;

    float fireTimer=0;

    [Header("Variabili attacco")]
    [SerializeField, Tooltip("Punto di sparo")]
    GameObject shootingPoint;
    [SerializeField, Tooltip("Mirino")]
    Crosshair rangedCrossair;
    float alphaCrosshair = 1;
    [SerializeField, Tooltip("velocità proiettile base")]
    float projectileSpeed = 30f;
    [SerializeField, Tooltip("gittata proiettile base")]
    float projectileRange = 30f;
    [SerializeField, Tooltip("frequenza di sparo multiplo")]
    float consecutiveFireTimer = 0.3f;
    [SerializeField, Tooltip("numero di spari con sparo multiplo")]
    float numberProjectile = 3;

    [Header("Abilità unica")]

    [SerializeField, Tooltip("tempo necessario per colpo potenziato")]
    float empowerFireChargeTime = 1.5f;
    [SerializeField, Tooltip("tempo ridotto caricamento con potenziamento")]
    float chargeTimeReduction = 0.5f;
    float empowerStartTimer = 0; //timer da caricare
    float empowerCoolDownTimer = 0;
    bool canUseUniqueAbility => empowerCoolDownTimer <= 0;
    [SerializeField, Tooltip("Aumento gittata per colpo potenziato")]
    float empowerAdditionalRange = 15f;
    [SerializeField, Tooltip("moltiplicatore danno per colpo potenziato")]
    [Min(1)]
    float empowerDamageMultiplier = 1.3f;
    [SerializeField, Tooltip("Moltplicatore grandezza proiettile per colpo caricato")] //forse da cambiare con uno sprite
    float empowerSizeMultiplier = 1.3f;

    [Header("Schivata")]

    [SerializeField, Tooltip("coolDown Schivata")]
    [Min(0)]
    float dodgeCoolDown = 3f;
    float dodgeTimer = 0;
    [SerializeField, Tooltip("distanza massima schivata")]
    float dodgeDistance = 15f;
    [SerializeField, Tooltip("Durata schivata")]
    [Min(0)]
    float dodgeDuration = 0.3f;
    [SerializeField, Tooltip("Durata schivata perfetta")]
    [Min(0)]
    float perfectDodgeDuration = 0.15f;
    [SerializeField, Tooltip("Danno schivata perfetta")]
    float dodgeDamageMultiplier = 0.75f;

    [Header("Abilità extra")]
    [SerializeField, Tooltip("coolDown recupero mina esplosa")]
    float landMineCoolDown = 10f;
    [SerializeField, Tooltip("Numero massimo delle mine")]
    int maxNumberLandMine = 1;
    [SerializeField] int landMineInInventory;
    [SerializeField, Tooltip("Prefab della mina")]
    GameObject prefabLandMine;
    [SerializeField, Tooltip("danno della mina")]
    float landMineDamageMultiplier = 2f;
    [SerializeField, Tooltip("raggio della mina")]
    float landMineRange = 5f;
    [SerializeField, Tooltip("Sprite di raccolta mina")]
    GameObject minePickUpVisualizer;
    bool mineNearby => nearbyLandmine.Count > 0;
    

    public List<LandMine> nearbyLandmine;

    [Header("Potenziamneto Boss fight")]
    [SerializeField, Tooltip("distanza massima per schivata perfetta ")]
    float perfectDodgeBossDistance = 30f;
    [SerializeField, Tooltip("Schivate perfette per sbloccare l'abilità")]
    int dodgeCounterToUnlock = 10;
    int dodgeCounter = 0; //contatore schivate perfette durante la bossfight
    [SerializeField, Tooltip("moltiplicatore danno per distanza del colpo")]
    [Min(1)]
    float maxDamageMultiplier = 2.5f;

    [Header("VFX")]
    [SerializeField] TrailRenderer trailDodgeVFX;
    [SerializeField] GameObject ChargedVFX;

    

    private bool reduceEmpowerFireCoolDownUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool multiBaseAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool dodgeTeleportBossUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool dodgeDamageUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool landMineUnlocked => upgradeStatus[AbilityUpgrade.Ability5];

    private PerfectTimingHandler perfectTimingHandler;

    private float empowerCoolDownDecrease => reduceEmpowerFireCoolDownUnlocked ? chargeTimeReduction : 0;

    bool isAttacking=false;
    bool isDodging=false;
    bool isInvunerable=false;

    public override void Inizialize()
    {
        base.Inizialize();
        nearbyLandmine = new List<LandMine>();
        landMineInInventory = maxNumberLandMine;
        perfectTimingHandler=GetComponentInChildren<PerfectTimingHandler>(true);
        perfectTimingHandler.gameObject.SetActive(false);
    }


    private void Start()
    {
        
    }


    protected override void Update()
    {
        base.Update();
        
        CoolDownManager();

        minePickUpVisualizer.SetActive(mineNearby);

        UpdateCrosshair(ReadLookdirCrosshair(shootingPoint.transform.position));
    }

    private void FixedUpdate()
    {
        if (!isRightInputRecently)
        {
            if(alphaCrosshair> 0)
            {
                alphaCrosshair-=Time.deltaTime;
                rangedCrossair.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alphaCrosshair);
            }
            
        }
        else
        {
            alphaCrosshair = 1;
            rangedCrossair.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alphaCrosshair);
        }

        


    }

    public override void Move(Vector2 direction)
    {
        if(!isDodging)
        {
            if(!isAttacking && !isDodging)
            {
                base.Move(direction);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
            

            if (rb.velocity.magnitude > 0.1f)
            {
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
        }      
    }

    

    public override void TakeDamage(DamageData data)
    {
        if (!isDodging)
        {
            StartCoroutine(PerfectDodgeHandler(data));
        }

        if (currentHp <= 0)
        {
            currentHp = 0;

            animator.SetTrigger("Death");
        }
    }

    //sparo
    #region Attack
    public override void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isAttacking)
        {
            if (fireTimer > 0)
            {
                Debug.Log("In ricarica...");

                return;
                //inserire suono (?)
            }

            isAttacking = true;

            Vector2 _look = ReadLook(context);

            //controllo che la look non sia zero, possibilità solo se si una il controller
            if (_look != Vector2.zero)
            {
                lookDirection = _look;
                SetShootDirection();
            }

            //in futuro inserire il colpo avanzato
            if (multiBaseAttackUnlocked)
            {
                StartCoroutine(MultipleFireProjectile(ShootDirection));
            }
            else
            {

                BasicFireProjectile(ShootDirection);

                fireTimer = AttackSpeed;

                Debug.Log("colpo normale");

                isAttacking = false;
            }

            rightInputTimer = recentlyInputTimer;

        }
    }

    //Sparo normale
    private void BasicFireProjectile(Vector2 direction)
    {

        animator.SetTrigger("SimpleShoot");
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = shootingPoint.transform.position;

        newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1,Damage,gameObject.layer);

        //TODO inserire if se in presenza di boss, per ora c'è per provare
        newProjectile.AddIncrementalDamage();

    }

    //Sparo triplo
    private IEnumerator MultipleFireProjectile(Vector2 direction)
    {
        for (int i = 0; i < numberProjectile; i++)
        {
            BasicFireProjectile(direction);

            yield return new WaitForSeconds(consecutiveFireTimer);
        }

        fireTimer = AttackSpeed;

        Debug.Log("colpo triplo");

        dodgeTimer = dodgeCoolDown;

        isAttacking = false;
    }



    #endregion

    //schivata
    #region Defence
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dodgeTimer > 0)
            {
                Debug.Log("schivata effettuata di recente");
                return;
            }

            StartCoroutine(Dodge(lastNonZeroDirection, rb));

            Debug.Log(lastNonZeroDirection);
        }
    }

    protected IEnumerator Dodge(Vector2 direction,Rigidbody2D rb)
    {
        if (!isDodging)
        {
            isDodging = true;
            isAttacking = false;


            //animazione

            animator.SetTrigger("Dodge");

            trailDodgeVFX.gameObject.SetActive(true);

            Vector2 dodgeDirection = direction.normalized;

            rb.velocity = dodgeDirection * (dodgeDistance / dodgeDuration);

            yield return new WaitForSeconds(dodgeDuration);
            PubSub.Instance.Notify(EMessageType.dodgeExecuted, this);

            rb.velocity = Vector2.zero;

            isDodging = false;

            trailDodgeVFX.gameObject.SetActive(false);

            dodgeTimer = dodgeCoolDown;


        }
        
    }

    protected IEnumerator PerfectDodgeHandler(DamageData data)
    {
        Debug.Log("Check");
        perfectTimingHandler.gameObject.SetActive(true);
        yield return new WaitForSeconds(perfectDodgeDuration);
        if(isDodging)
        {
            //se potenziamento sbloccato => damage
            if (dodgeDamageUnlocked)
            {
               
            }

            //se c'è il boss + potenziamento sbloccato => tp
            PubSub.Instance.Notify(EMessageType.perfectDodgeExecuted, this);
        }
        else
        {
            base.TakeDamage(data);
        }

        perfectTimingHandler.gameObject.SetActive(false);
        Debug.Log($"PerfectDodge: {isDodging}");
    }

    #endregion

    //mina
    #region ExtraAbility
    public override void ExtraAbilityInput(InputAction.CallbackContext context) //E
    {

        if (context.performed)
        {
            Debug.Log("Ho premuto e, ganzo");

            if (landMineUnlocked)
            {
                if (nearbyLandmine.Count<=0)
                {
                    //animazione droppaggio mina

                    //animator.settrigger("Droplandmine"); => da aggiungere

                    //momentaneo
                    CreateLandMine();
                    //

                    
                }
                else
                {
                    //prendo mina
                    nearbyLandmine[nearbyLandmine.Count - 1].PickUpLandmine();
                }
                
            }
        }
       
    }

    //probabile funzione da mettere in una animazione
    public void CreateLandMine()
    {
        if (landMineInInventory > 0)
        {
            GameObject newLandMine = Instantiate(prefabLandMine);

            newLandMine.transform.position = new Vector3(transform.position.x, transform.position.y);

            newLandMine.GetComponent<LandMine>().Initialize(gameObject.GetComponentInParent<PlayerCharacter>(),landMineRange,Damage * landMineDamageMultiplier,gameObject.layer);

            landMineInInventory--;

            Debug.Log("lascio mina");
        }
        else
        {
            Debug.Log("mina già piazzata");
        }
    }

    public void RecoverLandMine()
    {
        landMineInInventory++;
    }

    public void StartLandmineGeneration()
    {
        StartCoroutine(RegenerateLandMine());
    }
    private IEnumerator RegenerateLandMine()
    {
        yield return new WaitForSeconds(landMineCoolDown);

        landMineInInventory++;
    }

    #endregion
      
    //sparo caricato
    #region Unique ability
    public override void UniqueAbilityInput(InputAction.CallbackContext context) //Q
    {

        if (context.performed)
        {
            if (!canUseUniqueAbility)
            {
                Debug.Log("In ricarica...(abilità unica)");

                return;
                //inserire suono (?)
            }
            else
            {
                empowerStartTimer = Time.time;
                isAttacking = true;

                animator.SetBool("isCharging",true);
                animator.SetTrigger("StartCharging");
            }

            

        }
        else if (context.canceled)
        {

            if (canUseUniqueAbility && isAttacking)
            {
                float endTimer = Time.time;

                if (endTimer - empowerStartTimer > empowerFireChargeTime - empowerCoolDownDecrease)
                {

                    Vector2 _look = ReadLook(context);

                    //controllo che la look non sia zero, possibilità solo se si una il controller
                    if (_look != Vector2.zero)
                    {
                        lookDirection = _look;
                        SetShootDirection();
                    }

                    //in futuro inserire il colpo avanzato
                    EmpowerFireProjectile(ShootDirection);

                    empowerCoolDownTimer = UniqueAbilityCooldown;

                    Debug.Log("colpo potenziato");


                }
            }

            animator.SetBool("isCharging", false);
            isAttacking = false;
        }

        

    }

    //sparo caricato (abilità unica)
    private void EmpowerFireProjectile(Vector2 direction)
    {
        animator.SetTrigger("EmpowerShoot");
        

        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position =shootingPoint.transform.position;

        newProjectile.Inizialize(direction, projectileRange + empowerAdditionalRange, projectileSpeed, empowerSizeMultiplier,Damage*empowerDamageMultiplier,gameObject.layer);

        
    }

    #endregion

    //vari coolDown del personaggio
    private void CoolDownManager()
    {
        //timer visualizzazione mirino
        if (isRightInputRecently)
        {
            rightInputTimer -= Time.deltaTime;
        }       

        //sparo normale
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }

        //abilità unica (colpo caricato)
        if (empowerCoolDownTimer > 0)
        {
            empowerCoolDownTimer -= Time.deltaTime;
        }

        //schivata
        if(dodgeTimer > 0)
        {
            dodgeTimer -= Time.deltaTime;
        }

        
        

       
    }

    private void UpdateCrosshair(Vector2 position)
    {
        rangedCrossair.transform.position=new Vector2 (position.x,position.y);

       
    }



    private void SetShootDirection()
    {
        //Vector2 dummyTargetAimPosition = (Vector2)transform.position + lookDir;
        
        //ShootDirection = (dummyTargetAimPosition-(Vector2)(shootingPoint.transform.position)).normalized;

        ShootDirection=(rangedCrossair.transform.position- shootingPoint.transform.position).normalized;


    }

    //aggiungi death override

    public override void Ress()
    {
        animator.SetTrigger("Ress");
    }




}


// lista delle cose
//base

//    CECCHINO
//Attacco: attacco dalla distanza con cadenza media
//Schivata: schivata molto ampia che copre metà arena
//Abilità unica:  colpo di cecchino caricato
//Potenziamento boss fight: più spara da lontano più fa danno
//Ottenimento potenziamento Boss fight: deve schivare perfettamente 10 attacchi del boss;
//HP: medi


//avanzati
//1: Il caricamento del colpo di cecchino è più rapido
//2: L’attacco base spara una raffica di 3 proiettili
//3: Schivare perfettamente un colpo teletrasporta il personaggio in una parte dell’arena lontana dal boss
//4: Schivare perfettamente un colpo danneggia il nemico attaccante
//5: Il personaggio può lasciare a terra una mina che esplode al contatto con un nemico -- unica che va ripresa, se esplode applicare cd
