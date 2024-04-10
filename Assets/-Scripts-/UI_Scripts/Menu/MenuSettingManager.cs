using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettingManager : MonoBehaviour
{
    List<Button> settingButtons = new List<Button>();

   public void DisableAllButtons()
    {
        foreach (Button button in settingButtons)
        {
            button.interactable = false;
        }
    }   

}
