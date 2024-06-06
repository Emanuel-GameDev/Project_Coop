using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RangedEnemy), true)]
public class BasicRangedEnemyEditor : Editor
{
    private void OnSceneGUI()
    {
        RangedEnemy enemy = (RangedEnemy)target;
        Color oldColor = Handles.color;
        UnityEngine.Rendering.CompareFunction oldZtest = Handles.zTest;

        if (enemy.groundLevel != null)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = new Color(1, 0, 0, 0.2f);
            Handles.DrawSolidDisc(enemy.groundLevel.position, Vector3.forward, enemy.viewRange);

            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.color = new Color(0, 0, 1, 0.2f);
            Handles.DrawSolidDisc(enemy.groundLevel.position, Vector3.forward, enemy.changeTargetRange);

            Handles.color = new Color(0, 1, 0, 0.2f);
            Handles.DrawSolidDisc(enemy.groundLevel.position, Vector3.forward, enemy.attackRange);

            Handles.color = new Color(0, 0, 1, 0.2f);
            Handles.DrawSolidDisc(enemy.groundLevel.position, Vector3.forward, enemy.escapeRange);
        }

        Handles.zTest = oldZtest;
        Handles.color = oldColor;
    }
}
