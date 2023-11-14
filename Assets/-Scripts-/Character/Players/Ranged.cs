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
    float fireTimer;

    [Header("Variabili attacco")]
    [SerializeField, Tooltip("velocit� proiettile base")]
    float projectileSpeed=30f;
    [SerializeField, Tooltip("gittata proiettile base")]
    float projectileRange=30f;
    [SerializeField, Tooltip("frequenza di sparo multiplo")]
    float consecutiveFireTimer=0.3f;

    [Header("Abilit� unica")]

    [SerializeField, Tooltip("tempo necessario per colpo potenziato")]
    float empowerFireCoolDown=1.5f;
    float empowerFireTimer=0; //timer da caricare
    [SerializeField, Tooltip("Aumento gittata per colpo potenziato")]
    float empowerAdditionalRange=15f;
    [SerializeField, Tooltip("moltiplicatore danno per colpo potenziato")]
    [Min(1)]
    float empowerMultiplier=1.3f;

    [Header("Schivata")]

    [SerializeField, Tooltip("coolDown Schivata")]
    float dodgeCoolDown=3f;
    float dodgeTimer=0;
    [SerializeField, Tooltip("distanza massima schivata")]
    float dodgeDistance=15f;
    [SerializeField, Tooltip("Durata schivata")]
    float dodgeDuration = 0.3f;
    [SerializeField, Tooltip("Danno schivata perfetta")]
    float dodgeDamageMultiplier = 0.75f;

    [Header("Abilit� extra")]
    [SerializeField, Tooltip("Prefab della mina")]
    GameObject prefabLandMine;
    [SerializeField, Tooltip("danno della mina")]
    float landMineDamageMultiplier=2f;
    [SerializeField, Tooltip("raggio della mina")]
    float landMineRange=5f;

    [Header("Potenziamneto Boss fight")]
    [SerializeField, Tooltip("distanza massima per schivata perfetta ")]
    float perfectDodgeBossDistance = 30f;
    [SerializeField, Tooltip("Schivate perfette per sbloccare l'abilit�")]
    int dodgeCounterToUnlock=10;
    int dodgeCounter=0; //contatore schivate perfette durante la bossfight
    [SerializeField, Tooltip("moltiplicatore danno per distanza del colpo")]
    [Min(1)]
    float maxDamageMultiplier=2.5f;





    private void Update()
    {
        CoolDownManager();
    }

    

    public override void Attack(Character parent,InputAction.CallbackContext context)
    {

        if(fireTimer > 0)
        {
            Debug.Log("In ricarica...");

            return;
            //inserire suono (?)
        }

        Vector3 _look = parent.GetComponent<PlayerCharacter>().ReadLook();

        //controllo che la look non sia zero, possibilit� solo se si una il controller
        if(_look != Vector3.zero)
        {
            lookDirection = _look;
        }   

        //in futuro inserire il colpo avanzato
        BasicFireProjectile(lookDirection);

        fireTimer = AttackSpeed;
    }

    public override void Defence(Character parent, InputAction.CallbackContext context)
    {
        base.Defence(parent, context);
    }

    public override void UseExtraAbility(Character parent, InputAction.CallbackContext context)
    {
        base.UseExtraAbility(parent, context);
    }

    public override void UseUniqueAbility(Character parent, InputAction.CallbackContext context)
    {

        EmpowerFireProjectile(lookDirection);

    }

    //Sparo normale
    private void BasicFireProjectile(Vector3 direction)
    {
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = transform.position;

        newProjectile.Inizialize(direction, projectileRange, projectileSpeed);
    
    }

    //sparo caricato (abilit� unica)
    private void EmpowerFireProjectile(Vector3 direction)
    {
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = transform.position;

        newProjectile.Inizialize(direction, projectileRange+empowerAdditionalRange, projectileSpeed);
    }

    //vari coolDown del personaggio
    private void CoolDownManager()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
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
//5: Il personaggio pu� lasciare a terra una mina che esplode al contatto con un nemico
