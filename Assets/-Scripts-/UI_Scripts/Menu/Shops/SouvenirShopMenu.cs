using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SouvenirShopMenu : Menu
{
    public override void OpenMenu()
    {


        base.OpenMenu();
        Locale();
    }
    public Locale engLocale;

    public void Locale()
    {
        LocalizationSettings.SelectedLocale = engLocale;

    }
}
