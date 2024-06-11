using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TrashPressUi : MonoBehaviour
{
    [SerializeField] private TrashPressPlayerUI[] playersBox = new TrashPressPlayerUI[4];

    [SerializeField] private TMP_Text PhaseTimer;

    private Dictionary<ePlayerCharacter, TrashPressPlayerUI> playerBox = new Dictionary<ePlayerCharacter, TrashPressPlayerUI>();

    public void SetAllPlayer(List<PlayerInputHandler> players)
    {
        if (players != null)
        {
            for (int i = 0; i < players.Count && i < playersBox.Length; i++)
            {
                if (players[i].currentCharacter != ePlayerCharacter.EmptyCharacter)
                {
                    if (!playerBox.ContainsKey(players[i].currentCharacter))
                    {
                        playersBox[i].gameObject.SetActive(true);
                        playerBox.Add(players[i].currentCharacter, playersBox[i]);
                        playersBox[i].SetIconAndBackground(GameManager.Instance.GetCharacterData(players[i].currentCharacter).PixelFaceSprite,
                            GameManager.Instance.GetCharacterData(players[i].currentCharacter).PixelBackgroundSprite);

                    }

                    playersBox[i].SetHp(3);
                }
            }
        }
    }

    public void ResetHp()
    {       
        foreach (TrashPressPlayerUI box in playerBox.Values)
        {
            box.SetHp(3);
        }
    }

  
    public void UpdateCurrentHp(ePlayerCharacter player, int hp)
    {
        if (playerBox.ContainsKey(player))
            playerBox[player].SetHp(hp);
    }
    public void UpdatePhaseTimer(float value)
    {
        PhaseTimer.text = value.ToString();
    }

    void Start()
    {
        TrashPressManager.Instance.SetTrashPressUI(this);
        foreach (TrashPressPlayerUI box in playersBox)
        {
            box.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

}
