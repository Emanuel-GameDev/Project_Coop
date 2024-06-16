using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ChallengeUI : MonoBehaviour
{

    [SerializeField] private LocalizeStringEvent challengeDescription;
    [SerializeField] private LocalizeStringEvent challengeName;
    [SerializeField] private Button selectButton;
    [SerializeField] private Image challengeCompletedImage;
    public List<UiChallengeRank> uiChallengeRanks;
    public Challenge challengeSelected;
    private bool selected;

    public void SetUpUI()
    {
        if (challengeSelected.hasRanks)
        {
            //se sfida non completata oppure completata senza tutte con meno di 3 stelle
            if (challengeSelected.rank <= 2)
            {
                CanStartChallenge();
            }

            //se sfida completata con tutte e 3 le stelle
            else 
            {
                
                NotStartChallenge();
            }


            for (int i = 0; i < uiChallengeRanks.Count; i++)
            {
                uiChallengeRanks[i].SetUpUI(i);
            }
        }


        else
        {
            if (!challengeSelected.challengeCompleted)
            {
                CanStartChallenge();
            }

            else
            {
                NotStartChallenge();
            }
        }
    }

    private void NotStartChallenge()
    {
        challengeName.StringReference = challengeSelected.challengeName;
        challengeDescription.StringReference = challengeSelected.challengeDescription;
        challengeCompletedImage.gameObject.SetActive(true);
        selectButton.interactable = false;
    }

    private void CanStartChallenge()
    {
       
        challengeName.StringReference = challengeSelected.challengeName;
        challengeDescription.StringReference = challengeSelected.challengeDescription;
        selectButton.onClick.AddListener(MenuManager.Instance.CloseMenu);
        selectButton.onClick.AddListener(challengeSelected.ActivateGameobject);
        selectButton.onClick.AddListener(challengeSelected.Initiate);
    }

    public void ShowRanks(bool value)
    {
        foreach (var rank in uiChallengeRanks)
        {
            rank.gameObject.SetActive(value);
        }
    }

}
