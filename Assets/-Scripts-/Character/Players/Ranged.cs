using UnityEngine;

public class Ranged : CharacterClass
{
    //ci deve essere il riferimento alla look qua, non al proiettile
    //aggiungere statistiche personaggio + schivata+invincibilità
    //aggiungere prefab mina (?)
    //aggiungere vari timer(arma, schivata,cd vari)

    private Vector3 lookDirection = Vector3.forward;

    
    //base Attack
    public override float AttackSpeed => base.AttackSpeed;
    float fireTimer;

    [Header("Variabili attacco")]
    [SerializeField, Tooltip("velocità proiettile base")]
    float projectileSpeed=30f;
    [SerializeField, Tooltip("gittata proiettile base")]
    float projectileRange=30f;
    [SerializeField, Tooltip("frequenza di sparo multiplo")]
    float consecutiveFireTimer=0.3f;

    [Header("Abilità unica")]

    [SerializeField, Tooltip("tempo necessario per colpo potenziato")]
    float empowerFireCoolDown=1.5f;
    float empowerFireTimer=0;
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

    [Header("Potenziamneto Boss fight")]
    [SerializeField, Tooltip("Schivate perfette per sbloccare l'abilità")]
    int dodgeCounterToUnlock=10;
    int dodgeCounter=0;
    [SerializeField, Tooltip("moltiplicatore danno per distanza del colpo")]
    [Min(1)]
    float maxDamageMultiplier=2.5f;





    private void Update()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }


    public override void Attack(Character parent)
    {

        if(fireTimer > 0)
        {
            Debug.Log("In ricarica...");

            return;
            //inserire suono (?)
        }

        Vector3 _look = parent.GetComponent<PlayerCharacter>().ReadLook();

        //controllo che la look non sia zero, possibilità solo se si una il controller
        if(_look != Vector3.zero)
        {
            lookDirection = _look;
        }

        //in futuro inserire il colpo avanzato
        BasicFireProjectile(lookDirection);

        fireTimer = AttackSpeed;
    }

    public override void Defence(Character parent)
    {
        base.Defence(parent);
    }

    public override void UseExtraAbility(Character parent)
    {
        base.UseExtraAbility(parent);
    }

    public override void UseUniqueAbility(Character parent)
    {
        base.UseUniqueAbility(parent);
    }

    private void BasicFireProjectile(Vector3 direction)
    {
        Projectile newProjectile = ProjectilePool.Instance.GetProjectile();

        newProjectile.transform.position = transform.position;

        newProjectile.Inizialize(direction, projectileRange, projectileSpeed);

     
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
//5: Il personaggio può lasciare a terra una mina che esplode al contatto con un nemico
