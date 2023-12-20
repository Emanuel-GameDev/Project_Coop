
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Ranged : CharacterClass
{
    //ci deve essere il riferimento alla look qua, non al proiettile
    //aggiungere statistiche personaggio + schivata+invincibilit�
    //aggiungere prefab mina (?)
    //aggiungere vari timer(arma, schivata,cd vari)

    private Vector3 lookDirection = Vector3.forward;


    //base Attack
    public override float AttackSpeed => base.AttackSpeed;
    public override float UniqueAbilityCooldown => base.UniqueAbilityCooldown;

    float fireTimer;

    [Header("Variabili attacco")]
    [SerializeField, Tooltip("velocit� proiettile base")]
    float projectileSpeed = 30f;
    [SerializeField, Tooltip("gittata proiettile base")]
    float projectileRange = 30f;
    [SerializeField, Tooltip("frequenza di sparo multiplo")]
    float consecutiveFireTimer = 0.3f;
    [SerializeField, Tooltip("numero di spari con sparo multiplo")]
    float numberProjectile = 3;

    [Header("Abilit� unica")]

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

    [Header("Abilit� extra")]
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
    [SerializeField, Tooltip("Schivate perfette per sbloccare l'abilit�")]
    int dodgeCounterToUnlock = 10;
    int dodgeCounter = 0; //contatore schivate perfette durante la bossfight
    [SerializeField, Tooltip("moltiplicatore danno per distanza del colpo")]
    [Min(1)]
    float maxDamageMultiplier = 2.5f;

    

    private bool reduceEmpowerFireCoolDownUnlocked => upgradeStatus[AbilityUpgrade.Ability1];
    private bool multiBaseAttackUnlocked => upgradeStatus[AbilityUpgrade.Ability2];
    private bool dodgeTeleportBossUnlocked => upgradeStatus[AbilityUpgrade.Ability3];
    private bool dodgeDamageUnlocked => upgradeStatus[AbilityUpgrade.Ability4];
    private bool landMineUnlocked => upgradeStatus[AbilityUpgrade.Ability5];

    private PerfectTimingHandler perfectTimingHandler;

    private float empowerCoolDownDecrease => reduceEmpowerFireCoolDownUnlocked ? chargeTimeReduction : 0;

    bool isAttacking;
    bool isDodging;
    bool isInvunerable;

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        nearbyLandmine = new List<LandMine>();
        landMineInInventory = maxNumberLandMine;
        perfectTimingHandler=GetComponentInChildren<PerfectTimingHandler>();
        perfectTimingHandler.gameObject.SetActive(false);
    }


    private void Start()
    {
        
    }


    private void Update()
    {
        CoolDownManager();

        minePickUpVisualizer.SetActive(mineNearby);


    }

    public override void Move(Vector3 direction, Rigidbody rb)
    {
        if(!isDodging)
        {
            base.Move(direction, rb);

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

    //sparo
    #region Attack
    public override void Attack(Character parent, InputAction.CallbackContext context)
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

            Vector3 _look = parent.GetComponent<PlayerCharacter>().ReadLook();

            //controllo che la look non sia zero, possibilit� solo se si una il controller
            if (_look != Vector3.zero)
            {
                lookDirection = _look;
            }

            //in futuro inserire il colpo avanzato
            if (multiBaseAttackUnlocked)
            {
                StartCoroutine(MultipleFireProjectile(lookDirection));
            }
            else
            {

                BasicFireProjectile(lookDirection);

                fireTimer = AttackSpeed;

                Debug.Log("colpo normale");

                isAttacking = false;
            }

        }
    }

    //Sparo normale
    private void BasicFireProjectile(Vector3 direction)
    {

        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = transform.position;

        //newProjectile.Inizialize(direction, projectileRange, projectileSpeed, 1,Damage,gameObject.layer);

    }

    //Sparo triplo
    private IEnumerator MultipleFireProjectile(Vector3 direction)
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
    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dodgeTimer > 0)
            {
                Debug.Log("schivata effettuata di recente");
                return;
            }

            StartCoroutine(Dodge(lastNonZeroDirection, parent.GetRigidBody()));

            Debug.Log(lastNonZeroDirection);
        }
    }

    protected IEnumerator Dodge(Vector2 direction,Rigidbody rb)
    {
        if (!isDodging)
        {
            isDodging = true;

            //animazione

            Vector3 dodgeDirection = new Vector3(direction.x, 0f, direction.y).normalized;

            rb.velocity = dodgeDirection * (dodgeDistance / dodgeDuration);

            yield return new WaitForSeconds(dodgeDuration);

            rb.velocity = Vector3.zero;

            isDodging = false;
        }
        
    }

    protected IEnumerator PerfectDodgeHandler(DamageData data)
    {
        perfectTimingHandler.gameObject.SetActive(true);
        yield return new WaitForSeconds(perfectDodgeDuration);
        if(isDodging)
        {
            //se potenziamento sbloccato => damage
            if (dodgeDamageUnlocked)
            {
               
            }
            
            //se c'� il boss + potenziamento sbloccato => tp
            
        }
        else
        {
            base.TakeDamage(data);
        }

        Debug.Log($"PerfectDodge: {isDodging}");
    }

    #endregion

    //mina
    #region ExtraAbility
    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context) //E
    {
        if (context.performed)
        {
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

            newLandMine.transform.position = new Vector3(transform.position.x, 0 , transform.position.z);

            newLandMine.GetComponent<LandMine>().Initialize(gameObject.GetComponentInParent<PlayerCharacter>(),landMineRange,Damage * landMineDamageMultiplier,gameObject.layer);

            landMineInInventory--;

            Debug.Log("lascio mina");
        }
        else
        {
            Debug.Log("mina gi� piazzata");
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
    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context) //Q
    {

        if (context.performed)
        {
            if (empowerCoolDownTimer > 0)
            {
                Debug.Log("In ricarica...(abilit� unica)");

                return;
                //inserire suono (?)
            }

            empowerStartTimer = Time.time;

        }
        else if (context.canceled && canUseUniqueAbility)
        {
            float endTimer = Time.time;

            if (endTimer - empowerStartTimer > empowerFireChargeTime - empowerCoolDownDecrease)
            {

                Vector3 _look = parent.GetComponent<PlayerCharacter>().ReadLook();

                //controllo che la look non sia zero, possibilit� solo se si una il controller
                if (_look != Vector3.zero)
                {
                    lookDirection = _look;
                }

                //in futuro inserire il colpo avanzato
                EmpowerFireProjectile(lookDirection);

                empowerCoolDownTimer = UniqueAbilityCooldown;

                Debug.Log("colpo potenziato");
            }
        }

    }

    //sparo caricato (abilit� unica)
    private void EmpowerFireProjectile(Vector3 direction)
    {
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = transform.position;

        //newProjectile.Inizialize(direction, projectileRange + empowerAdditionalRange, projectileSpeed, empowerSizeMultiplier,Damage*empowerDamageMultiplier,gameObject.layer);
    }

    #endregion

    //vari coolDown del personaggio
    private void CoolDownManager()
    {
        //sparo normale
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }

        //abilit� unica (colpo caricato)
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




}


// lista delle cose
//base

//    CECCHINO
//Attacco: attacco dalla distanza con cadenza media
//Schivata: schivata molto ampia che copre met� arena
//Abilit� unica:  colpo di cecchino caricato
//Potenziamento boss fight: pi� spara da lontano pi� fa danno
//Ottenimento potenziamento Boss fight: deve schivare perfettamente 10 attacchi del boss;
//HP: medi


//avanzati
//1: Il caricamento del colpo di cecchino � pi� rapido
//2: L�attacco base spara una raffica di 3 proiettili
//3: Schivare perfettamente un colpo teletrasporta il personaggio in una parte dell�arena lontana dal boss
//4: Schivare perfettamente un colpo danneggia il nemico attaccante
//5: Il personaggio pu� lasciare a terra una mina che esplode al contatto con un nemico -- unica che va ripresa, se esplode applicare cd
