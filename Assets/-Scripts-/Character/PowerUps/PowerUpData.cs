using System;
using System.Collections.Generic;

[Serializable]
public class PowerUpData
{
   public List<PowerUp> _powerUpData { get; private set; } = new List<PowerUp>();

    // Damage
    public float damageIncrease { get; private set; } = 0;

    // Health
    public float maxHpIncrease { get; private set; } = 0;

    // Speed
    public float moveSpeedIncrease { get; private set; } = 0;

    // Cooldown
    public float uniqueAbilityCooldownDecrease { get; private set; } = 0;

    // Stamina
    public float attackSpeedIncrease { get; private set; } = 0;



    //Aggiunge a lista powerUp e calcolo statistiche
    public void Add(PowerUp powerUp)
    {
        _powerUpData.Add(powerUp);
        switch (powerUp.powerUpType)
        {
            case PowerUpType.Damage:
                damageIncrease += powerUp.value;
                break;

            case PowerUpType.Health:
                maxHpIncrease += powerUp.value;
                break;

            case PowerUpType.MoveSpeed:
                moveSpeedIncrease += powerUp.value;
                break;

            case PowerUpType.UniqueAbilityCooldown:
                uniqueAbilityCooldownDecrease += powerUp.value;
                break;

            case PowerUpType.AttackSpeed:
                attackSpeedIncrease += powerUp.value;
                break;
        }
    }

    //Rimuovi a lista powerUp e calcolo statistiche
    public void Remove(PowerUp powerUp)
    {
        if (_powerUpData.Contains(powerUp))
        {
            _powerUpData.Remove(powerUp);

            switch (powerUp.powerUpType)
            {
                case PowerUpType.Damage:
                    damageIncrease -= powerUp.value;
                    break;

                case PowerUpType.Health:
                    maxHpIncrease -= powerUp.value;
                    break;

                case PowerUpType.MoveSpeed:
                    moveSpeedIncrease -= powerUp.value;
                    break;

                case PowerUpType.UniqueAbilityCooldown:
                    uniqueAbilityCooldownDecrease -= powerUp.value;
                    break;

                case PowerUpType.AttackSpeed:
                    attackSpeedIncrease -= powerUp.value;
                    break;
            }
        }
    }
}
