using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PowerUp))]
public class PowerUpInspector : Editor
{
    public override void OnInspectorGUI()
    {
        PowerUp powerUp = (PowerUp)target;

        // Mostra il campo "powerUpType" nel Inspector
        powerUp.powerUpType = (PowerUpType)EditorGUILayout.EnumPopup("Power Up Type", powerUp.powerUpType);

        // In base al tipo selezionato, mostra le variabili appropriate
        switch (powerUp.powerUpType)
        {
            case PowerUpType.Damage:
                powerUp.damageIncrease = EditorGUILayout.IntField("Damage Up", powerUp.damageIncrease);
                break;

            case PowerUpType.Health:
                powerUp.healthIncrease = EditorGUILayout.IntField("Health Up", powerUp.healthIncrease);
                break;

            case PowerUpType.Speed:
                powerUp.speedIncrease = EditorGUILayout.FloatField("Speed Up", powerUp.speedIncrease);
                break;

            case PowerUpType.Cooldown:
                powerUp.cooldownDecrease = EditorGUILayout.IntField("Cooldown down", powerUp.cooldownDecrease);
                break;

            case PowerUpType.Stamina:
                powerUp.staminaIncrease = EditorGUILayout.IntField("Stamina Up", powerUp.staminaIncrease);
                break;

            default:
                break;
        }

        // Assicurati di aggiornare i dati serializzati nell'oggetto PowerUp
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
