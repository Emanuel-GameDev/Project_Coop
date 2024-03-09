using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameMenu : MonoBehaviour
{
    public virtual void Start()
    {
        MinigameMenuManager.Instance.AddMinigameMenu(this);
    }

    public void SetActive(bool active)
    {
        if(active)
            MinigameMenuManager.Instance.SetActiveMenu(this);
        else
            MinigameMenuManager.Instance.DisactivateMenu(this);
    }
}
