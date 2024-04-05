using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class ChallengeManager : MonoBehaviour
{
    [SerializeField] private List<Challenge> ChallengesObjectsParents;
    private Challenge selectedChallenge;
    public bool debug;


    private void Start()
    {
        if (!debug)
        {
            selectedChallenge = SelectChallenge();
            selectedChallenge.Initiate();
        }
        
    }
    //Debug

    private void Update()
    {
        //DEBUG
        if (Input.GetKeyUp(KeyCode.I) && debug)
        {
            selectedChallenge = SelectChallenge();
            selectedChallenge.Initiate();
        }
    }
    private Challenge SelectChallenge()
    {
        foreach (Challenge challenge in ChallengesObjectsParents)
        {
            challenge.gameObject.SetActive(false);
            
        }
        Challenge selectedChallenge = ChallengesObjectsParents[Random.Range(0, ChallengesObjectsParents.Count)];
        selectedChallenge.gameObject.SetActive(true);
        return selectedChallenge;
    }
}
