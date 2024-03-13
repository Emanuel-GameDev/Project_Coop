using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPHandler : MonoBehaviour
{
    private static HPHandler _instance;
    public static HPHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HPHandler>();

                if (_instance == null)
                {
                    GameObject singletonObject = new("HPHandler");
                    _instance = singletonObject.AddComponent<HPHandler>();
                }
            }

            return _instance;
        }
    }


    public TextMeshProUGUI P1HP;
    public TextMeshProUGUI P2HP;
    public TextMeshProUGUI P3HP;
    public TextMeshProUGUI P4HP;

    private PlayerCharacter player1;
    private PlayerCharacter player2;
    private PlayerCharacter player3;
    private PlayerCharacter player4;


    public void SetActivePlayers()
    {
        List<PlayerCharacter> players = GameManager.Instance.coopManager.ActivePlayers;
        foreach (PlayerCharacter player in players)
        {
           if(player1 == null)
             player1 = player;
            else if (player2 == null)
                player2 = player;
            else if (player3 == null)
                player3 = player;
            else
                player4 = player;
        }
    }

    private void Update()
    {
        if (player1 != null)
            P1HP.text = player1.CharacterClass.currentHp.ToString();
        if (player2 != null)
            P2HP.text = player2.CharacterClass.currentHp.ToString();
        if (player3 != null)
            P3HP.text = player3.CharacterClass.currentHp.ToString();
        if (player4 != null)
            P4HP.text = player4.CharacterClass.currentHp.ToString();
    }


}
