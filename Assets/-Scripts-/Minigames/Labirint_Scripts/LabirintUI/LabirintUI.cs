using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LabirintUI : MonoBehaviour
{
    [SerializeField] private PlayerBoxUI[] playersBox = new PlayerBoxUI[4];

    [SerializeField] private TMP_Text remainingKeyCount;

    private Dictionary<ePlayerCharacter, PlayerBoxUI> playerBox = new Dictionary<ePlayerCharacter, PlayerBoxUI>();

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

                    playersBox[i].SetKeyCount(0);
                }
            }
        }
    }

    public void ResetCount()
    {
        UpdateRemainingKeyCount(0);
        foreach (PlayerBoxUI box in playerBox.Values)
        {
            box.SetKeyCount(0);
        }
    }

    public void UpdateRemainingKeyCount(int count)
    {
        remainingKeyCount.text = count.ToString();
    }

    public void UpdatePickedKey(ePlayerCharacter player, int count)
    {
        if (playerBox.ContainsKey(player))
            playerBox[player].SetKeyCount(count);
    }

    void Start()
    {
        LabirintManager.Instance.SetLabirintUI(this);
        foreach (PlayerBoxUI box in playersBox)
        {
            box.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

}
