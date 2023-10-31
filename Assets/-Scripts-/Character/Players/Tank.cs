using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : CharacterClass
{
    public override void Attack(Character parent)
    {
        base.Attack(parent);
        //se potenziamento 1 ha 2 attacchi
        //se potenziamento 3 attiva hyperArmor

        //se potenziamento boss fight attacco caricato 
        //se potenziamento 5 attacco caricato (Da decidere)
    }
    public override void Defence(Character parent)
    {
        base.Defence(parent);
        //se potenziamento 4 ha parata perfetta
    }
    public override void UseExtraAbility(Character parent)
    {
        base.UseExtraAbility(parent);
        //se potenziamento boss attacco caricato e potenziamento 2 più stun
    }
    public override void UseUniqueAbility(Character parent)
    {
        base.UseUniqueAbility(parent);
        //attacco attiro aggro
    }
    public override void TakeDamage(float damage, Damager dealer)
    {
        //se sta attacando e potenziamento 3 sbloccato non interrompe attacco
    }
}
