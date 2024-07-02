using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Healer : PlayerCharacter
{
    [SerializeField] float attackDelay = 1f;
    [SerializeField] GameObject visual;

    [Header("Small heal ability information")]

    [Tooltip("Icona che appare sopra il personaggio da curare")]
    [SerializeField] GameObject healIcon;
    [Tooltip("")]
    [SerializeField] public Detector smallHealTrigger;
    [Tooltip("Quantità di vita curata dall'abilità di cura singola")]
    [SerializeField] float smallHeal = 5f;
    [Tooltip("Tempo di ricarica dell'abilità di cura singola")]
    [SerializeField] float singleHealCooldown = 5f;
    [Tooltip("Distanza massima a cui può essere un alleato per essere curato")]
    [SerializeField] float smallHealAreaRadius = 7f;
    [Tooltip("Particellare della cura")]
    [SerializeField] GameObject smallHealParticlePrefab;


    [Header("Heal mine ability information")]

    [Tooltip("Prefab della mina")]
    [SerializeField] GameObject healMine;
    [Tooltip("Quantità di vita curata dalla mina")]
    [SerializeField] float mineHealQuantity = 1f;
    [Tooltip("Raggio del trigget della mina")]
    [SerializeField] float healMineRadius = 1f;
    [Tooltip("Tempo che la mina impiega prima di attivarsi")]
    [SerializeField] float healMineActivationTime = 1f;
    [Tooltip("Tempo di ricarica per poter piazzare la mina")]
    [SerializeField] float mineAbilityCooldown = 0;
    [Tooltip("Sprite che appare sopra al personaggio se può raccogliere la mina")]
    [SerializeField] Sprite healMineIcon;
    [Tooltip("Mine icon gameObject")]
    [SerializeField] GameObject gameObjectMineIcon;

    GameObject instantiatedHealMine;


    [Header("Heal area base information")]
    [Tooltip("Prefab dell'area di cura")]
    [SerializeField] GameObject healArea;
    [Tooltip("Durata dell' area di cura")]
    [SerializeField] float areaDuration = 1;
    [Tooltip("Raggio dell'area di cura")]
    [SerializeField] float healAreaRadius = 1f;
    [Tooltip("Numero di tik al secondo")]
    [SerializeField] float tikPerSecond = 1;
    [Tooltip("Quantità di vita curata ogni tik")]
    [SerializeField] float healPerTik = 1f;


    [Header("Heal area upgrade stats")]
    [Tooltip("Quantità di vita tolta per tik (Abilità 1)")]
    [SerializeField] float DOTPerTik = 1;
    [Tooltip("Aumento del raggio dell'area di cura (Abilità 2)")]
    [SerializeField] float healAreaIncrementedRadious = 5;
    [Tooltip("Rallentamento (Abilità 4)")]
    [SerializeField] PowerUp slowDown;
    [Tooltip("Riduzione difesa (Abilità 5)")]
    [SerializeField] float damageIncrementPercentage = 1;


    [Header("Boss powerUp")]
    [Tooltip("Colpi consecutivi richiesti al boss, senza subire danni, per sbloccare l'abilità del boss")]
    [SerializeField] int bossPowerUpHitToUnlock = 10;
    [Tooltip("Rallentamento durante l'abilità del boss")]
    [SerializeField] float bossAbilitySlowdown;

    [Header("Audio")]
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip bombSound;

    [Header("VFX")]
    [SerializeField] GameObject BombVFX;

    //CapsuleCollider2D smallHealAreaCollider;

    List<PlayerCharacter> playerInArea;
    Dictionary<PlayerCharacter,GameObject> healIcons;

    //private float lastAttackTime;
    //private float lastUniqueAbilityUseTime;
    private int bossPowerUpHitCount;

    float bossAbilityChargeTimer = 0;
    float bossAbilityCharge = 3;
    //float uniqueAbilityTimer;
    float mineAbilityTimer;
    float smallHealTimer;

    bool defenceButtonPressed = false;
    bool bossAbilityReady = false;
    bool bossAbilityPerformed = false;

    bool mineInReach = false;

    ParticleSystem.EmissionModule emissionModule;


    public override void Inizialize()
    {
        base.Inizialize();
        playerInArea = new List<PlayerCharacter>();
        //smallHealAreaCollider = gameObject.AddComponent<CapsuleCollider2D>();
        //smallHealAreaCollider.isTrigger = true;
        //smallHealAreaCollider. = 1.5f;
        //smallHealAreaCollider.radius = smallHealAreaRadius;

        healIcons = new Dictionary<PlayerCharacter, GameObject>();

        animator.SetFloat("Y", -1);
        animator.SetBool("IsDead", false);
        baseMoveSpeed = MoveSpeed;
        blockInput = false;

        //prova

        //SaveManager.Instance.LoadAllData();

        //if (upgradeStatus[AbilityUpgrade.Ability2])
        //{
        //    Debug.Log("2");
        //}
        //else Debug.Log("no");

        emissionModule = _walkDustParticles.emission;
    }


    //Attack: colpo singolo, incremento colpi consecutivi senza subire danni contro boss
    public override void AttackInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (blockInput)
            return;

        if (context.performed)
        {
            animator.SetTrigger("IsAttacking");
        }

        if (context.canceled)
        {
            animator.ResetTrigger("IsAttacking");
        }


    }
    public void PlayAttackSound()
    {
        AudioManager.Instance.PlayAudioClip(attackSound, transform, 0.5f);
    }

    public void PlayBombSound()
    {
        AudioManager.Instance.PlayAudioClip(bombSound, transform, 0.5f);
    }

    public void DeactivateInput()
    {
        blockInput = true;
    }

    public void ActivateInput()
    {
        blockInput = false;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() && !playerInArea.Contains(other.gameObject.GetComponent<PlayerCharacter>()))
        {
            playerInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());
            GameObject instantiatedIcon = Instantiate(healIcon);

            healIcons.Add(other.gameObject.GetComponent<PlayerCharacter>(), instantiatedIcon);
            instantiatedIcon.transform.SetParent(other.transform);
            instantiatedIcon.transform.localPosition = new Vector3(0, 1.5f, 0);
            
        }

        if (other.gameObject.GetComponent<HealMine>())
        {
            SetMineIcon(true, healMineIcon);
            mineInReach = true;
        }
    }



    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
        {
            playerInArea.Remove(other.gameObject.GetComponent<PlayerCharacter>());

            //Destroy(healIcons[other.gameObject.GetComponent<PlayerCharacter>()]);
            healIcons.Remove(other.gameObject.GetComponent<PlayerCharacter>());
        }


        if (other.gameObject.GetComponent<HealMine>())
        {
            SetMineIcon(false, null);
            mineInReach = false;
        }
    }

    private void Start()
    {
        //uniqueAbilityTimer = UniqueAbilityCooldown;
        mineAbilityTimer = mineAbilityCooldown;
        smallHealTimer = singleHealCooldown;
    }
    float baseMoveSpeed = 0;
    bool blockInput = false;
    protected override void Update()
    {
        base.Update();
        
        //Move(moveDir);

        //if (uniqueAbilityTimer < UniqueAbilityCooldown)
        //{
        //    uniqueAbilityTimer += Time.deltaTime;
        //}

        if (mineAbilityTimer < mineAbilityCooldown)
        {
            mineAbilityTimer += Time.deltaTime;
        }

        if (smallHealTimer < singleHealCooldown)
        {
            smallHealTimer += Time.deltaTime;
        }

        if (defenceButtonPressed)
        {
            if (bossAbilityChargeTimer < bossAbilityCharge)
            {
                bossAbilityChargeTimer += Time.deltaTime;

                moveSpeed = bossAbilitySlowdown;         
            }
        }
        else
        {

            moveSpeed = baseMoveSpeed;
            
            bossAbilityChargeTimer = 0;
        }

        if (bossAbilityChargeTimer >= bossAbilityCharge)
            bossAbilityReady = true;

    }


    public override void Move(Vector2 direction)
    {
        if (blockInput)
        {
            base.Move(Vector2.zero);
        }
        else
            base.Move(direction);



        if (direction != Vector2.zero)
        {
            animator.SetBool("IsMoving", true);
            emissionModule.enabled = true;
        }
        else
        {
            animator.SetBool("IsMoving", false);
            emissionModule.enabled= false;
        }
            
    }


    [HideInInspector] public bool canHealEnemies = false; 
    //Defense: cura ridotta singola
    public override void DefenseInput(InputAction.CallbackContext context)
    {
        if (blockInput)
            return;

        if (context.performed)
        {
            defenceButtonPressed = true;
        }

        if (context.canceled)
        {

            if (bossAbilityReady && !bossAbilityPerformed)
            {
                BossAbility();
                bossAbilityReady = false;
            }
            else
            {
                if (smallHealTimer >= singleHealCooldown)
                {

                    animator.SetTrigger("CastSmallHeal");
                    StartCoroutine(InputReactivationDelay(animator.GetCurrentAnimatorClipInfo(0).Length));
                   

                    smallHealTimer = 0;
                }
            }

            defenceButtonPressed = false;
            bossAbilityChargeTimer = 0;
        }
    }

    public void SpawnBombVFX()
    {
        Instantiate(BombVFX,transform.position, Quaternion.identity);
    }

    public void HealCharactersInRange()
    {
        TakeHeal(new DamageData(smallHeal, null));

        foreach (PlayerCharacter pc in smallHealTrigger.GetPlayersDetected())
        {
            pc.TakeHeal(new DamageData(smallHeal, null));
            PubSub.Instance.Notify(EMessageType.characterHealed, pc);
        }

        if (canHealEnemies)
        {
            foreach (EnemyCharacter enemyCharacter in smallHealTrigger.GetEnemiesDetected())
            {
                if (enemyCharacter.gameObject.GetComponent<TutorialEnemy>() != null)
                {
                    PubSub.Instance.Notify(EMessageType.characterHealed, enemyCharacter.gameObject.GetComponent<TutorialEnemy>());
                }
            }

        }
    }


    public void PlaySmallHealParticles()
    {
        Instantiate(smallHealParticlePrefab,transform.position , Quaternion.identity);
    }

    //UniqueAbility: lancia area di cura
    public override void UniqueAbilityInput(InputAction.CallbackContext context)
    {
        if (blockInput)
            return;

        if (/*uniqueAbilityTimer < UniqueAbilityCooldown */  !UniqueAbilityAvaiable || !context.performed)
            return;

        

        animator.SetTrigger("CastHealArea");
        StartCoroutine(InputReactivationDelay(animator.GetCurrentAnimatorClipInfo(0).Length));
        base.UniqueAbilityInput(context);
        UniqueAbilityUsed();
        //uniqueAbilityTimer = 0;
    }

    public void SpawnHealArea()
    {
        PubSub.Instance.Notify(EMessageType.uniqueAbilityActivated, this);

        float radius;

        //calcolo raggio area
        if (upgradeStatus[AbilityUpgrade.Ability2])
        {
            radius = healAreaRadius + healAreaIncrementedRadious;
        }
        else
            radius = healAreaRadius;

        HealArea areaSpawned = Instantiate(healArea, new Vector2(transform.position.x, transform.position.y), Quaternion.identity).GetComponent<HealArea>();


        areaSpawned.Initialize(
            gameObject,
            areaDuration,
            tikPerSecond,
            radius,
            upgradeStatus[AbilityUpgrade.Ability1],
            upgradeStatus[AbilityUpgrade.Ability4],
            upgradeStatus[AbilityUpgrade.Ability5]);


        areaSpawned.healPerTik = healPerTik;
        areaSpawned.DOTPerTik = DOTPerTik;
        areaSpawned.slowDown = slowDown;
        areaSpawned.damageIncrementPercentage = damageIncrementPercentage;
    }


    //ExtraAbility: piazza mina di cura
    public override void ExtraAbilityInput(InputAction.CallbackContext context)
    {
        if (blockInput)
            return;
       

        if (instantiatedHealMine == null)
        {
            if (upgradeStatus[AbilityUpgrade.Ability3] && context.started)
            {
                if (mineAbilityTimer < mineAbilityCooldown)
                    return;

                animator.SetTrigger("PlaceMine");

                instantiatedHealMine = Instantiate(healMine, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
                instantiatedHealMine.GetComponent<HealMine>().Initialize(gameObject, mineHealQuantity, healMineRadius, healMineActivationTime);

                mineAbilityTimer = 0;
                mineInReach = false;
            }
                
        }
        else
        {
            if (mineInReach && context.started)
            {
                Destroy(instantiatedHealMine);
                instantiatedHealMine = null;
                mineInReach = false;
                SetMineIcon(false, null);
                mineAbilityTimer = mineAbilityCooldown;

            }
                Debug.Log("Reached2");
        }
    }

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        bossPowerUpHitCount = 0;
    }

    public override void Die()
    {
        base.Die();
        blockInput = true;
        animator.SetBool("IsDead", true);
    }

    public override void Ress()
    {
        base.Ress();
        blockInput = false;
        animator.SetBool("IsDead", false);
    }


    //Cura tutti i player
    public void BossAbility()
    {
        Debug.Log("Cura tutti");
        //foreach(PlayerCharacter player in GameManager.Instance.coopManager.activePlayers)
        //{
        //    player.CharacterClass.currentHp = player.CharacterClass.MaxHp;
        //}

        bossAbilityPerformed = true;
    }

    public void SetMineIcon(bool state, Sprite spriteIcon)
    {
        //gameObjectMineIcon.SetActive(state);

        //if (spriteIcon != null)
        //    gameObjectMineIcon.GetComponent<SpriteRenderer>().sprite = spriteIcon;
    }

    IEnumerator InputReactivationDelay(float delay)
    {
        blockInput = true;
        yield return new WaitForSecondsRealtime(delay);
        blockInput = false;
    }


    //Potenziamento boss fight: cura istantanea in tutta la mappa


    //Ottenimento potenziamento Boss fight: Colpire il boss tot volte senza subire danni


    //Ability Upgrade:
    //1: L’area di cura intorno al personaggio danneggia i nemici al suo interno
    //2: L’area di cura diventa più ampia
    //3: Il personaggio lascia a terra una “mina” che cura i giocatori che ci passano sopra
    //4: L’area di cura intorno al personaggio rallenta i nemici al suo interno (cumulabile con altre abilità simili)
    //5: L’area di cura intorno al personaggio riduce la difesa dei nemici al suo interno (cumulabile con altre abilità simili)
}
