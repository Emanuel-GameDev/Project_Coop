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
                if (challengeSelected.firstStarDescription != null)
                {
                    challengeRankDescription.StringReference = challengeSelected.firstStarDescription;
                    if(challengeSelected.rank >= 1)
                    {
                        challengeRankCompletedImage.gameObject.SetActive(true);
                    }
                }
                break;

            case 1:
                if (challengeSelected.secondStarDescription != null)
                {
                    challengeRankDescription.StringReference = challengeSelected.secondStarDescription;
                    if (challengeSelected.rank >= 2)
                    {
                        challengeRankCompletedImage.gameObject.SetActive(true);
                    }
                }

                break;

            case 2:
                if (challengeSelected.thirdStarDescription != null)
                {
                    challengeRankDescription.StringReference = challengeSelected.thirdStarDescription;
                    if (challengeSelected.rank >= 3)
                    {
                        challengeRankCompletedImage.gameObject.SetActive(true);
                    }
                }

                break;

        }

        if (challengeSelected.challengeCompleted)
        {
            

        }
    }


}
