using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MinigameMenu
{
    [SerializeField]
    private List<Button> buttons = new();

    private Button selectedButton;

    public override void CancelButton(InputReceiver player)
    {
        base.CancelButton(player);
    }

    public override void MenuButton(InputReceiver player)
    {
        base.MenuButton(player);
    }

    public override void NavigateButton(Vector2 direction, InputReceiver player)
    {
        base.NavigateButton(direction, player);
    }

    public override void SubmitButton(InputReceiver player)
    {
        base.SubmitButton(player);
    }
}
