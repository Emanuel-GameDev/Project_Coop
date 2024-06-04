using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{
    
    [SerializeField] private LocalizeStringEvent challegeDescription;
    [SerializeField] private LocalizeStringEvent challengeName;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image challengeCompletedImage;
    public  List<UiChallengeRank> uiChallengeRanks;
    public Challenge challengeSelected;
    private bool selected;

    public void SetUpUI()
    {
        //se sfida non completata oppure completata senza tutte con meno di 3 stelle
        if (!challengeSelected.challengeCompleted || (challengeSelected.challengeCompleted && challengeSelected.rank <=2))
        {
            challengeName.StringReference = challengeSelected.challengeName;
            challegeDescription.StringReference = challengeSelected.challengeDescription;
            selectButton.onClick.AddListener(MenuManager.Instance.CloseMenu);
            selectButton.onClick.AddListener(challengeSelected.ActivateGameobject);
            selectButton.onClick.AddListener(challengeSelected.Initiate);
        }
        //se sfida completata con tutte e 3 le stelle
        else if (challengeSelected.challengeCompleted && challengeSelected.rank == 3)
        {
            challengeName.StringReference = challengeSelected.challengeName;
            challegeDescription.StringReference = challengeSelected.challengeDescription;
            challengeCompletedImage.gameObject.SetActive(true);
          
            selectButton.enabled = false;
        }
    }

    public void ShowRanks(bool value)
    {
       foreach (var rank in uiChallengeRanks)
        {
            rank.gameObject.SetActive(value);
        }
    }

}
