using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Challenge> challengesList;
    [SerializeField] private MenuInfo menuInfo;
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject challengeUIPrefab;
    private Challenge selectedChallenge;


    private void Start()
    {
        Shuffle(challengesList);
        for (int i = 0; i < 3; ++i)
        {
            GameObject tempObj = Instantiate(challengeUIPrefab, panel.gameObject.transform);
            ChallengeUI tempUI = tempObj.GetComponent<ChallengeUI>();
            tempUI.challengeSelected = challengesList[i];
            tempUI.SetUpUI();
        }
        

    }
    public static void Shuffle(List<Challenge> list)
    {
        int count = list.Count;
        int last = count - 1;
        for (int i = 0; i < last; ++i)
        {
            int r = UnityEngine.Random.Range(i, count);
            Challenge tmp = list[i];
            list[i] = list[r];
            list[r] = tmp;
        }
    }
    
    public void Interact(IInteracter interacter)
    {
        MenuManager.Instance.OpenMenu(menuInfo,CoopManager.Instance.GetPlayer(ePlayerID.Player1));
       
       
    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.EnableInteraction(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteracter>(out var interacter))
        {
            interacter.DisableInteraction(this);
        }
    }

    public void CancelInteraction(IInteracter interacter)
    {
        
    }

    public IInteracter GetFirstInteracter()
    {
        return null;
    }

    public void AbortInteraction(IInteracter interacter)
    {
        
    }
}
