using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class UiChallengeRank : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent challengeRankDescription;
    [SerializeField] private Image challengeRankCompletedImage;
    public Challenge challengeSelected;

    public void SetUpUI(int id)
    {
        Debug.LogWarning(challengeSelected.ToString());
        Debug.LogWarning(challengeSelected.firstStarDescription);
        switch (id)
        {
            case 0:
                if(challengeSelected.firstStarDescription != null)
                {
                    challengeRankDescription.StringReference = challengeSelected.firstStarDescription;
                }             
                break;

            case 1:
                if (challengeSelected.secondStarDescription != null)
                    challengeRankDescription.StringReference = challengeSelected.secondStarDescription;
                break;

            case 2:
                if (challengeSelected.thirdStarDescription != null)
                    challengeRankDescription.StringReference = challengeSelected.thirdStarDescription;
                break;

        }
        
        if (challengeSelected.challengeCompleted)
        {
            challengeRankCompletedImage.gameObject.SetActive(true);

        }
    }


}
