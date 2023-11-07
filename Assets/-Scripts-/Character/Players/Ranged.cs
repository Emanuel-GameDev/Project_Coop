using UnityEngine;

public class Ranged : CharacterClass
{
    //ci deve essere il riferimento alla look qua, non al proiettile
    //aggiungere statistiche personaggio + schivata+invincibilità
    //aggiungere prefab mina (?)
    //aggiungere vari timer(arma, schivata,cd vari)

    private Vector3 lookDirection = Vector3.forward;

    //dati dell'attacco
    [SerializeField] float fireCd;
    float fireTimer;

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

        fireTimer = fireCd;
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

        //settare futuri sprite

        //settare la direzione
        newProjectile.SetTravelDirection(direction);
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
