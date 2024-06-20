using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SlotMachineUI : MonoBehaviour
{
    [Header("Side UI")]
    [SerializeField] TextMeshProUGUI difficultyTest;

    public GameObject lightEasyModeGameobject;
    public GameObject LightMediumModeGameobject;
    public GameObject LighthardModeGameobject;



    public List<UnityEngine.UI.Image> winCombinationUIGameObjects;

    public List<Sprite> charactersButtonsUISprites;

    public List<UnityEngine.UI.Image> buttonUIGameObjects;

    public List<UnityEngine.UI.Image> playerUISprite;

    public void SetTextDifficulty(string text)
    {
        difficultyTest.text = text;
    }
    


}
