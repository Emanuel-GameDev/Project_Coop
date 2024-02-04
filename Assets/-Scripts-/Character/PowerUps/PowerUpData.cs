using System;
using System.Collections.Generic;

[Serializable]
public class PowerUpData
{
   public List<PowerUp> powerUps { get; private set; } = new List<PowerUp>();

    // Damage
    public float damageIncrease { get; private set; } = 0;

    // Health
    public float maxHpIncrease { get; private set; } = 0;

    // Speed
    public float moveSpeedIncrease { get; private set; } = 0;

    // Cooldown
    public float UniqueAbilityCooldownDecrease => 1 - uniqueAbilityCooldownDecrease > 0 ? 1 - uniqueAbilityCooldownDecrease : 0 ;
    private float uniqueAbilityCooldownDecrease = 0;



    // Stamina
    public float attackSpeedIncrease { get; private set; } = 0;



    //Aggiunge a lista powerUp e calcolo statistiche
    public void Add(PowerUp powerUp)
    {
        powerUps.Add(powerUp);
        switch (powerUp.powerUpType)
        {
            case eStatsType.Damage:
                damageIncrease += powerUp.value;
                break;

            case eStatsType.Health:
                maxHpIncrease += powerUp.value;
                break;

            case eStatsType.MoveSpeed:
                moveSpeedIncrease += powerUp.value;
                break;

            case eStatsType.UniqueAbilityCooldown:
                uniqueAbilityCooldownDecrease += powerUp.value;
                break;

            case eStatsType.AttackSpeed:
                attackSpeedIncrease += powerUp.value;
                break;
        }
    }

    //Rimuovi a lista powerUp e calcolo statistiche
    public void Remove(PowerUp powerUp)
    {
        if (powerUps.Contains(powerUp))
        {
            powerUps.Remove(powerUp);

            switch (powerUp.powerUpType)
            {
                case eStatsType.Damage:
                    damageIncrease -= powerUp.value;
                    break;

                case eStatsType.Health:
                    maxHpIncrease -= powerUp.value;
                    break;

                case eStatsType.MoveSpeed:
                    moveSpeedIncrease -= powerUp.value;
                    break;

                case eStatsType.UniqueAbilityCooldown:
                    uniqueAbilityCooldownDecrease -= powerUp.value;
                    break;

                case eStatsType.AttackSpeed:
                    attackSpeedIncrease -= powerUp.value;
                    break;
            }
        }
    }
}
