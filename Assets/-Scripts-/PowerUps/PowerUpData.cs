using System;
using System.Collections.Generic;

[Serializable]
public class PowerUpData
{
   public List<PowerUp> _powerUpData { get; private set; } = new List<PowerUp>();

    // Damage
    public int damageIncrease { get; private set; } = 0;

    // Health
    public int healthIncrease { get; private set; } = 0;

    // Speed
    public float speedIncrease { get; private set; } = 0;

    // Cooldown
    public int cooldownDecrease { get; private set; } = 0;

    // Stamina
    public int staminaIncrease { get; private set; } = 0;



    //Aggiunge a lista powerUp e calcolo statistiche
    public void Add(PowerUp powerUp)
    {
        _powerUpData.Add(powerUp);
        switch (powerUp.powerUpType)
        {
            case PowerUpType.Damage:
                damageIncrease += powerUp.damageIncrease;
                break;

            case PowerUpType.Health:
                healthIncrease += powerUp.healthIncrease;
                break;

            case PowerUpType.Speed:
                speedIncrease += powerUp.speedIncrease;
                break;

            case PowerUpType.Cooldown:
                cooldownDecrease += powerUp.cooldownDecrease;
                break;

            case PowerUpType.Stamina:
                staminaIncrease += powerUp.staminaIncrease;
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
                    damageIncrease -= powerUp.damageIncrease;
                    break;

                case PowerUpType.Health:
                    healthIncrease -= powerUp.healthIncrease;
                    break;

                case PowerUpType.Speed:
                    speedIncrease -= powerUp.speedIncrease;
                    break;

                case PowerUpType.Cooldown:
                    cooldownDecrease -= powerUp.cooldownDecrease;
                    break;

                case PowerUpType.Stamina:
                    staminaIncrease -= powerUp.staminaIncrease;
                    break;
            }
        }
    }
}
