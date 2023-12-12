using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Healer : CharacterClass
{
    [SerializeField] float attackDelay = 1f;
    [SerializeField] GameObject visual;

    [Header("Small heal ability information")]

    //[Tooltip("Icona che appare sopra il personaggio da curare")]
    //[SerializeField] GameObject healIcon;
    [Tooltip("Quantità di vita curata dall'abilità di cura singola")]
    [SerializeField] float smallHeal = 5f;
    [Tooltip("Tempo di ricarica dell'abilità di cura singola")]
    [SerializeField] float singleHealCooldown = 5f;
    [Tooltip("Distanza massima a cui può essere un alleato per essere curato")]
    [SerializeField] float smallHealAreaRadius = 1f;


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
    [SerializeField] float damageIncrement = 1;


    [Header("Boss powerUp")]
    //[Tooltip("Quantità di vita curata dall'abilità del boss")]
    //[SerializeField] float bossPowerUpHeal = 50f;
    [Tooltip("Colpi consecutivi richiesti al boss, senza subire danni, per sbloccare l'abilità del boss")]
    [SerializeField] int bossPowerUpHitToUnlock = 10;
    [Tooltip("Rallentamento durante l'abilità del boss")]
    [SerializeField] PowerUp bossAbilitySlowdown;

    CapsuleCollider smallHealAreaCollider;

    List<PlayerCharacter> playerInArea;


    private float lastAttackTime;
    private float lastUniqueAbilityUseTime;
    private int bossPowerUpHitCount;


    float uniqueAbilityTimer;
    float mineAbilityTimer;
    float smallHealTimer;

    bool defenceButtonPressed = false;
    bool bossAbilityReady = false;
    bool bossAbilityPerformed = false;

    bool mineInReach = false;

    bool inputState = true;

    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        transform.position = character.transform.position;
        playerInArea = new List<PlayerCharacter>();
        smallHealAreaCollider = gameObject.AddComponent<CapsuleCollider>();
        smallHealAreaCollider.isTrigger = true;
        smallHealAreaCollider.height = 1.5f;
        smallHealAreaCollider.radius = smallHealAreaRadius;


        ////provvisorio
        //instantiatedHealIcon = Instantiate(healIcon);
        //MoveIcon(transform);


        animator.SetFloat("Y", -1);
    }


    //private void MoveIcon(Transform newParent)
    //{
    //    instantiatedHealIcon.transform.SetParent(newParent);
    //    instantiatedHealIcon.transform.localPosition = new Vector3(0, 1, 0);
    //}


    //Attack: colpo singolo, incremento colpi consecutivi senza subire danni contro boss
    public override void Attack(Character parent, UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            animator.SetTrigger("IsAttacking");
        }

        if (context.canceled)
        {
            animator.ResetTrigger("IsAttacking");
        }

    }

    public void DeactivateInput()
    {
        inputState = false;
    }

    public void ActivateInput()
    {
        inputState = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>() && !playerInArea.Contains(other.gameObject.GetComponent<PlayerCharacter>()))
        {
            playerInArea.Add(other.gameObject.GetComponent<PlayerCharacter>());
        }

        if (other.gameObject.GetComponent<HealMine>())
        {
            SetMineIcon(true, healMineIcon);
            mineInReach = true;
        }
    }

    private void Start()
    {
        uniqueAbilityTimer = UniqueAbilityCooldown;
        mineAbilityTimer = mineAbilityCooldown;
        smallHealTimer = singleHealCooldown;
    }

    private void Update()
    {
        if (uniqueAbilityTimer < UniqueAbilityCooldown)
        {
            uniqueAbilityTimer += Time.deltaTime;
        }

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
                if(!character.GetPowerUpList().Contains(bossAbilitySlowdown))
                    character.AddPowerUp(bossAbilitySlowdown);
            }
        }
        else
        {
            if (character.GetPowerUpList().Contains(bossAbilitySlowdown))
                character.RemovePowerUp(bossAbilitySlowdown);
            bossAbilityChargeTimer = 0;
        }

        if (bossAbilityChargeTimer >= bossAbilityCharge)
            bossAbilityReady = true;

    }

    float bossAbilityChargeTimer = 0;
    float bossAbilityCharge = 3;


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerCharacter>())
            playerInArea.Remove(other.gameObject.GetComponent<PlayerCharacter>());


        if (other.gameObject.GetComponent<HealMine>())
        {
            SetMineIcon(false, null);
            mineInReach = false;
        }
    }




    public override void Move(Vector2 direction, Rigidbody rb)
    {
        if (!inputState)
        {
            base.Move(Vector2.zero, rb);
            return;
        }

        base.Move(direction, rb);

        if (direction != Vector2.zero)
            animator.SetBool("IsMoving", true);
        else
            animator.SetBool("IsMoving", false);
    }



    //Defense: cura ridotta singola
    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (!inputState)
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
                    TakeDamage(new DamageData(-smallHeal, null));

                    foreach (PlayerCharacter pc in playerInArea)
                    {
                        pc.TakeDamage(new DamageData(-smallHeal, null));
                    }
                    
                    smallHealTimer = 0;
                }
            }

            defenceButtonPressed = false;
            bossAbilityChargeTimer = 0;
        }
    }

    //UniqueAbility: lancia area di cura
    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {
        if (!inputState)
            return;

        if (uniqueAbilityTimer < UniqueAbilityCooldown || !context.performed)
            return;

        animator.SetTrigger("CastHeal");

        uniqueAbilityTimer = 0;
    }

    public void SpawnHealArea()
    {
        float radius;

        //calcolo raggio area
        if (upgradeStatus[AbilityUpgrade.Ability2])
            radius = healAreaRadius + healAreaIncrementedRadious;
        else
            radius = healAreaRadius;


        HealArea areaSpawned = Instantiate(healArea, new Vector3(character.transform.position.x, 0, character.transform.position.z), Quaternion.identity).GetComponent<HealArea>();


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
        areaSpawned.damageIncrement = damageIncrement;
    }


    //ExtraAbility: piazza mina di cura
    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {

        if (!inputState)
            return;

        if (instantiatedHealMine == null)
        {
            if (upgradeStatus[AbilityUpgrade.Ability3]  && context.performed)
            {
                if (mineAbilityTimer < mineAbilityCooldown)
                    return;

                animator.SetTrigger("PlaceMine");

                instantiatedHealMine = Instantiate(healMine, new Vector3(parent.transform.position.x, 0.1f, parent.transform.position.z), Quaternion.identity);
                instantiatedHealMine.GetComponent<HealMine>().Initialize(gameObject, mineHealQuantity, healMineRadius, healMineActivationTime);

                mineAbilityTimer = 0;
                mineInReach = false;
            }
        }
        else
        {
            if (mineInReach && context.performed)
            {
                Destroy(instantiatedHealMine);
                instantiatedHealMine = null;
                mineInReach = false;
                SetMineIcon(false, null);
                mineAbilityTimer = mineAbilityCooldown;
            }
        }
    }

    public override void TakeDamage(DamageData data)
    {
        base.TakeDamage(data);
        currentHp -= data.damage;
        bossPowerUpHitCount = 0;
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
        gameObjectMineIcon.SetActive(state);

        if (spriteIcon != null)
            gameObjectMineIcon.GetComponent<SpriteRenderer>().sprite = spriteIcon;
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
