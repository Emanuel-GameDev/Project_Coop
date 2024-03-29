using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MinigameMenu
{
    [SerializeField]
    private List<Button> buttons = new();

    private Button selectedButton;
    private int selectedIndex = -1;

    public override void CancelButton(ePlayerID player)
    {
        base.CancelButton(player);
    }

    public override void MenuButton(ePlayerID player)
    {
        base.MenuButton(player);
    }

    public override void NavigateButton(Vector2 direction, ePlayerID player)
    {
        base.NavigateButton(direction, player);
        if (CheckPlayer(player))
            SelectButton(direction);
    }

    public override void SubmitButton(ePlayerID player)
    {
        base.SubmitButton(player);
        if(selectedButton != null && CheckPlayer(player))
            selectedButton.onClick?.Invoke();
    }

    public override void GoPreviousMenu()
    {
        base.GoPreviousMenu();
        this.previousMenu = null;
    }

    private void SelectButton(Vector2 direction)
    {
        if (direction.y > 0)
        {
            selectedIndex--;
        }
        else if (direction.y < 0)
        {
            selectedIndex++;
        }

        if(selectedIndex < 0)
            selectedIndex = buttons.Count - 1;
        else if (selectedIndex >= buttons.Count)
            selectedIndex = 0;

        selectedButton = buttons[selectedIndex];
        selectedButton.Select();
    }

}
