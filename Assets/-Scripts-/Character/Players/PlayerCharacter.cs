using UnityEngine;

public enum ClassCharacter
{
    tank,
    dps,
    ranged,
    healer
}
public class PlayerCharacter : Character
{
    [SerializeField] private ClassCharacter _class;

    public override void Attack()
    {
        Attack attackInfo = skillTree.GetAttackData(this) as Attack;


        foreach (PowerUp p in powerPool)
        {

            //cerca i potenziamenti d'attacco
        }

        //Play animazione attacco
    }
    public override void Defend()
    {
        skillTree.GetDefendData(this);
    }
    public override void Move()
    {
        skillTree.GetMoveData(this);
    }

    public void UniqueAbility()
    {
        skillTree.UseUniqueData(this);
    }







}
