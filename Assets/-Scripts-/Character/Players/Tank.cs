using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


//Unique = tasto nord,Q = Urlo
//BossUpgrade = tasto est,E = attacco bossFight
public class Tank : CharacterClass
{
    [Header("Attack")]
    [SerializeField, Tooltip("Durata tempo pressione tasto attacco prima per decidere se attacco caricato o no")]
    float timeCheckAttackType = 0.2f;
    [SerializeField, Tooltip("Tempo minimo attacco caricato per essere eseguito")]
    float chargedAttackTimer = 2.5f;
    [SerializeField, Tooltip("danno ad area intorno al player")]
    float chargedAttackDamage = 50f;
    [SerializeField, Tooltip("Range danno ad area intorno al player")]
    float chargedAttackRadius = 5f;
    [SerializeField, Tooltip("Sprite del segnale visivo se attacco caricato pronto")]
    GameObject chargedAttackSprite;

    [Header("Block")]

    [SerializeField, Tooltip("Quantità di danno parabile prima di rottura parata")]
    float maxStamina;
    [SerializeField, Tooltip("Danno parata perfetta")]
    float perfectBlockDamage;
    [SerializeField, Tooltip("Angolo parata frontale al tank")]
    float blockAngle = 90;
    [SerializeField, Tooltip("distanza parata frontale al tank")]
    float blockRange = 3f;

    [Header("Unique Ability")]

    [SerializeField, Tooltip("Range aggro")]
    float aggroRange;
    [SerializeField, Tooltip("Durata aggro")]
    float aggroDuration;
    [SerializeField, Tooltip("Moltiplicatore buff difesa")]
    float defenceMultiplier;
    [SerializeField, Tooltip("Moltiplicatore buff stamina")]
    float staminamultiplier;

    [Header("Bossfight Upgrade")]
    [SerializeField, Tooltip("Numero attacchi da parare perfettamente per ottenimento potenziamento bossfight")]
    int attacksToBlockForUpgrade = 10;
    [SerializeField, Tooltip("Cooldown attacco potenziamento bossfight")]
    float cooldownExtraAbility;
    [SerializeField, Tooltip("Durata stun attacco potenziamento boss fight")]
    float chargedAttackStunDuration = 5;
    [SerializeField, Tooltip("Moltiplicatore durata stun")]
    float stunDurationMultiplier;


    private bool doubleAttack => upgradeStatus[AbilityUpgrade.Ability1];
    private bool maximizedStun => upgradeStatus[AbilityUpgrade.Ability2];
    private bool implacableAttack => upgradeStatus[AbilityUpgrade.Ability3];
    private bool damageOnParry => upgradeStatus[AbilityUpgrade.Ability4];
    private bool chargedAttack => upgradeStatus[AbilityUpgrade.Ability5];


    private bool hyperArmorOn;
    private bool isAttacking = false;
    private bool bossfightUpgradeUnlocked;
    private bool canPressInput;
    private bool pressed;
    private bool chargedAttackReady;
    private bool canMove = true;
    private bool isBlocking;
    private bool canCancelAttack;

    private int comboIndex = 0;
    private int comboMax = 2;
    private int perfectBlockCount;

    private float currentStamina;
    private float blockAngleThreshold => (blockAngle - 180) / 180;
    private GenericBarScript staminaBar;
    private GameObject chargedAttackAreaObject = null;


    public override void Inizialize(CharacterData characterData, Character character)
    {
        base.Inizialize(characterData, character);
        currentStamina = maxStamina;
        currentHp = maxHp;

        staminaBar = GetComponentInChildren<GenericBarScript>();

        staminaBar.Setvalue(maxStamina);
        staminaBar.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            bossfightPowerUpUnlocked = true;
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(new DamageData(10, null));
        }



    }


    #region Attack

    public override void Attack(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isBlocking == false)
        {
            ActivateHyperArmor();
            pressed = true;
            canCancelAttack = false;
            Invoke(nameof(CheckAttackToDo), timeCheckAttackType);

        }

        if (context.canceled)
        {
            pressed = false;


            if (chargedAttackReady && !canCancelAttack)
            {
                animator.SetTrigger("ChargedAttackEnd");
                Debug.Log("Charged Attack executed");
                chargedAttackSprite.gameObject.SetActive(false);

            }

            else if (!chargedAttackReady && canCancelAttack)
            {
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();

            }


            isAttacking = false;
            chargedAttackReady = false;
        }

    }
    public void IncreaseComboIndex()
    {
        comboIndex++;
        if (comboIndex > 2)
        {
            comboIndex = 0;
        }
    }
    public void CheckAttackToDo()
    {
        SetCanMove(false, character.GetRigidBody());

        if (pressed && chargedAttack)
        {
            canCancelAttack = true;
            isAttacking = true;
            animator.SetTrigger("ChargedAttack");
            Debug.Log($"Started Charged Attack");
            StartCoroutine(StartChargedAttackTimer());

        }

        else if (!pressed)
        {
            canCancelAttack = false;

            if (comboIndex == 0 && !isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("Attack1");
                IncreaseComboIndex();
            }

            else if (doubleAttack && comboIndex == 1)
            {
                if (comboIndex != 2)
                {
                    animator.SetTrigger("Attack2");
                    IncreaseComboIndex();
                }

            }

        }
    }
    IEnumerator StartChargedAttackTimer()
    {
        chargedAttackReady = false;

        yield return new WaitForSeconds(chargedAttackTimer);

        if (pressed)
        {

            chargedAttackReady = true;
            canCancelAttack = false;

            Debug.Log("Charged Attack Ready");
            chargedAttackSprite.SetActive(true);
        }

    }
    public void ActivateHyperArmor()
    {
        if (implacableAttack)
        {
            hyperArmorOn = true;

            Debug.Log("hyper armor on");
        }

    }
    private void DeactivateHyperArmor()
    {
        hyperArmorOn = false;
    }
    public void ResetAttackCombo()
    {
        if (comboIndex == 0 || comboIndex == 1)
        {
            comboIndex = 0;
            isAttacking = false;
            Debug.Log("Reset Variables");
            SetCanMove(true, character.GetRigidBody());

        }
        else if (comboIndex == 2)
        {
            IncreaseComboIndex();
        }

    }
    public void aaaaaaaaaaaaaaaaaaaaaa()
    {
        Debug.Log($"combo index:[{comboIndex}] can Double Attack[{doubleAttack}]");
    }

    public void ChargedAttackAreaDamage()
    {
        RaycastHit[] hitted = Physics.SphereCastAll(transform.position, chargedAttackRadius, Vector3.up, chargedAttackRadius);
        if (hitted != null)
        {
            foreach (RaycastHit r in hitted)
            {
                if (Utility.IsInLayerMask(r.transform.gameObject, LayerMask.GetMask("Enemy")))
                {
                    IDamageable hittedDama = r.transform.gameObject.GetComponent<IDamageable>();
                    hittedDama.TakeDamage(new DamageData(chargedAttackDamage, character, null));
                    Debug.Log(r.transform.gameObject.name + " colpito con " + chargedAttackDamage + " damage");

                }
            }
        }
    }



    #endregion

    #region Block
    private bool AttackInBlockAngle(DamageData data)
    {
        MonoBehaviour dealerMB = (MonoBehaviour)data.dealer;
        Vector3 dealerDirection = (dealerMB.transform.position - transform.position);

        Vector3 lastDirection = GetLastNonZeroDirection();

        return Vector3.Dot(lastDirection, dealerDirection) < blockAngleThreshold ? true : false;

    }

    Vector3 RotateVectorAroundPivot(Vector3 vector, Vector3 pivot, float angle)
    {
        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        vector -= pivot;
        vector = rotation * vector;
        vector += pivot;
        return vector;
    }



    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false && canCancelAttack == false)
        {
            SetCanMove(false, character.GetRigidBody());
            isBlocking = true;
            ShowStaminaBar(true);
            Debug.Log($"is blocking [{isBlocking}]");

            mostraGizmoRangeParata = true;

        }

        else if (context.canceled && isBlocking == true)
        {
            SetCanMove(true, character.GetRigidBody());
            isBlocking = false;
            ShowStaminaBar(false);
            ResetStamina();
            Debug.Log($"is blocking [{isBlocking}]");
            mostraGizmoRangeParata = false;
        }


        //se potenziamento 4 parata perfetta fa danno
    }

    private void ResetStamina()
    {
        currentStamina = maxStamina;
        staminaBar.ResetValue();
    }

    public void ShowStaminaBar(bool toShow)
    {
        staminaBar.gameObject.SetActive(toShow);
    }
    #endregion

    #region onHit

    public override void TakeDamage(DamageData data)
    {
        if (hyperArmorOn == false)
        {
            //Hit Reaction
            //annulla attacco
        }

        if (isBlocking)
        {

            staminaBar.DecreaseValue(data.damage);
            currentStamina -= data.damage;
            Debug.Log($"{currentStamina}");
            if (currentStamina <= 0)
            {
                SetCanMove(true, character.GetRigidBody());
                isBlocking = false;
                ShowStaminaBar(false);
                Debug.Log("Parata Rotta");
                //Stun per 2 secondi dopo reset stamina
            }
        }
        else
        {
            base.TakeDamage(data);
            Debug.Log($" Tank current hp : {currentHp}");
        }
    }


    #endregion

    #region UniqueAbility(Shout)

    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Da eliminare
            mostraGizmoAbilitaUnica = true;
            Invoke(nameof(SetGizmoAbilitaUnica), 1.2f);


            animator.SetTrigger("UniqueAbility");
            Debug.Log("UniqueAbility Used");
        }

    }

    public void PerformUniqueAbility()
    {
        RaycastHit[] hitted = Physics.SphereCastAll(transform.position, aggroRange, Vector3.up, aggroRange);
        if (hitted != null)
        {
            foreach (RaycastHit r in hitted)
            {
                if (Utility.IsInLayerMask(r.transform.gameObject, LayerMask.GetMask("Enemy")))
                {
                    IDamageable hittedDama = r.transform.gameObject.GetComponent<IDamageable>();

                    AggroCondition aggroCondition = Utility.InstantiateCondition<AggroCondition>();
                    aggroCondition.SetVariable(this, aggroDuration);

                    hittedDama.TakeDamage(new DamageData(0, character, aggroCondition));


                }
            }
        }
        //Incremento difesa e buff stamina
    }

    #endregion

    #region ExtraAbility(BossAttack)

    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context) //Tasto est
    {
        if (context.performed && !isAttacking && !isBlocking)
        {
            SetCanMove(false, character.GetRigidBody());

            if (bossfightPowerUpUnlocked && isAttacking == false)
            {
                damager.SetCondition(Utility.InstantiateCondition<AggroCondition>(), true);
                animator.SetTrigger("UniqueAbility");

                float stunDamageDuration = maximizedStun ? (chargedAttackStunDuration * stunDurationMultiplier) : chargedAttackStunDuration;
                Debug.Log($"BossFight Upgrade Attack Executed, stun duration:[{stunDamageDuration}]");
            }
        }

    }

    #endregion

    #region Move

    public override void Move(Vector2 direction, Rigidbody rb)
    {
        if (canMove)
        {
            base.Move(direction, rb);

        }
        animator.SetBool("IsMoving", isMoving);

    }


    private void SetCanMove(bool move, Rigidbody rigidbody)
    {
        canMove = move;
        if (move == false)
        {
            rigidbody.velocity = Vector3.zero;
        }

    }
    #endregion

    //Eliminare

    private bool mostraGizmoAbilitaUnica;
    public bool mostraGizmoRangeParata;
    int segments = 50;
    float startAngle => blockAngle / 2;



    public void OnDrawGizmos()
    {


        if (mostraGizmoAbilitaUnica)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, aggroRange);
        }
        if (mostraGizmoRangeParata)
        {
            Vector3 temp = new Vector3((GetLastNonZeroDirection().x + transform.position.x), transform.position.y, GetLastNonZeroDirection().y + transform.position.z);


            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, temp);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (-1 * (blockAngle / 2))));
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, RotateVectorAroundPivot(temp, transform.position, (1 * (blockAngle / 2))));


        }

    }

    private void SetGizmoAbilitaUnica()
    {
        mostraGizmoAbilitaUnica = false;
    }


    void DrawArc(Vector3 center, float radius, float angle, int segments)
    {
        Gizmos.color = Color.blue;

    }


    Vector3 GetPointOnCircle(float radius, float angle)
    {
        float radianAngle = Mathf.Deg2Rad * angle;
        float x = transform.position.x + radius * Mathf.Cos(radianAngle);
        float y = transform.position.y + radius * Mathf.Sin(radianAngle);

        return new Vector3(x, y, transform.position.z);
    }

}
