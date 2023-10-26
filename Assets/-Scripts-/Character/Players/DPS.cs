using UnityEngine;

public class DPS : CharacterClass
{
    //Attack: combo rapida di tre attacchi melee, ravvicinati. 
    public override void Attack(Character parent)
    {
       
    }

    //Defense: fa una schivata, si sposta di tot distanza verso la direzione decisa dal giocatore con uno scatto
    public override void Defence(Character parent)
    {
        
    }

    //UniqueAbility: immortalità per tot secondi
    public override void UseUniqueAbility(Character parent)
    {
       
    }

    //ExtraAbility: è l'ability upgrade 1
    public void AbilityUpgrade1()
    {
        
    }

    //Potenziamento boss fight: gli attacchi consecutivi aumentano il danno del personaggio a ogni colpo andato a segno.
    //Dopo tot tempo (es: 1.5 secondi) senza colpire, il danno torna al valore standard.
    // Implementa il potenziamento boss fight
    public void BossFightPowerUp()
    {
        
    }

    //Ottenimento potenziamento Boss fight: sbloccabile automaticamente dopo tot danni fatti al boss
    // Implementa la logica per l'ottenimento del potenziamento della boss fight
    public void UnlockBossFightPowerUp()
    {
        
    }

    //Ability Upgrade:
    //1: Sblocca uno scatto in avanti + attacco
    //2: Annulla il tempo di ricarica tra le combo di attacchi
    //3: Il personaggio può respingere certi tipi di colpi(es: proiettili) con il suo attacco
    //4: quando il personaggio usa l’abilità unica(i secondi di immortalità) i suoi movimenti diventano più rapidi(attacchi, schivate e spostamenti)
    //5: Effettuare una schivata perfetta aumenta i danni per tot tempo(cumulabile con il bonus ai danni del potenziamento).

    // Implementa le logiche per gli Ability Upgrades da 1 a 5
}
