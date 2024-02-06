using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NotBuildScenes", menuName = "Custom/NotBuildScenes", order = 2)]
public class NotBuildScenes : ScriptableObject
{
    public List<string> scenesNotInBuild = new List<string>();
}
