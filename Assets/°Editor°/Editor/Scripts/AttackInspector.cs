using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Attack))]
public class AttackInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Attack attack = (Attack)target;

        // Mostra sempre i camp "damage,cooldown,ranged" nel Inspector
        attack.damage = EditorGUILayout.FloatField("Damage",attack.cooldown);
        attack.cooldown = EditorGUILayout.FloatField("Cooldown",attack.cooldown);
        attack.ranged = EditorGUILayout.Toggle("Ranged", attack.ranged);

        // In base al tipo selezionato, mostra le variabili appropriate
        if (attack.ranged)
        {
            attack.range = EditorGUILayout.FloatField("Range", attack.range);
            attack.prefabBullet = (GameObject)EditorGUILayout.ObjectField("Bullet Prefab", attack.prefabBullet, typeof(GameObject), false);
        }

        // Assicurati di aggiornare i dati serializzati nell'oggetto PowerUp
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
