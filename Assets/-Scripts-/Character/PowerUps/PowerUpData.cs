using System;
using System.Collections.Generic;

[Serializable]
public class PowerUpData
{
    private List<PowerUp> powerUps = new();
    public List<PowerUp> PowerUps => powerUps;

    // Damage
    private float damageIncrease = 1;
    public float DamageIncrease => damageIncrease;

    // Health
    private float maxHpIncrease = 1;
    public float MaxHpIncrease => maxHpIncrease;

    // Speed
    private float moveSpeedIncrease = 1;
    public float MoveSpeedIncrease => moveSpeedIncrease;

    // Cooldown
    public float UniqueAbilityCooldownDecrease => 1 - uniqueAbilityCooldownDecrease > 0 ? 1 - uniqueAbilityCooldownDecrease : 0;
    private float uniqueAbilityCooldownDecrease = 1;

    // AttackSpeed
    private float attackSpeedIncrease = 1;
    public float AttackSpeedIncrease => attackSpeedIncrease;


    //Aggiunge a lista powerUp e calcolo statistiche
    public void Add(PowerUp powerUp)
    {
        powerUps.Add(powerUp);
        switch (powerUp.powerUpType)
        {
            case StatsType.Damage:
                damageIncrease += powerUp.value;
                break;

            case StatsType.Health:
                maxHpIncrease += powerUp.value;
                break;

            case StatsType.MoveSpeed:
                moveSpeedIncrease += powerUp.value;
                break;

            case StatsType.UniqueAbilityCooldown:
                uniqueAbilityCooldownDecrease += powerUp.value;
                break;

            case StatsType.AttackSpeed:
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
                case StatsType.Damage:
                    damageIncrease -= powerUp.value;
                    break;

                case StatsType.Health:
                    maxHpIncrease -= powerUp.value;
                    break;

                case StatsType.MoveSpeed:
                    moveSpeedIncrease -= powerUp.value;
                    break;

                case StatsType.UniqueAbilityCooldown:
                    uniqueAbilityCooldownDecrease -= powerUp.value;
                    break;

                case StatsType.AttackSpeed:
                    attackSpeedIncrease -= powerUp.value;
                    break;
            }
        }
    }
}
