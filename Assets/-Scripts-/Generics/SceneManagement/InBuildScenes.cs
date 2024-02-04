using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InBuildScenes", menuName = "Custom/InBuildScenes", order = 1)]
public class InBuildScenes : ScriptableObject
{
    public List<string> scenesInBuild = new List<string>();
}