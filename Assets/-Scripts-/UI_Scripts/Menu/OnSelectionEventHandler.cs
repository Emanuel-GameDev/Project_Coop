using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class OnSelectionEventHandler : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    UnityEvent onSelection;
    public UnityEvent OnSelection
    {
        get;
        set;
    }

    public void OnSelect(BaseEventData eventData)
    {
        onSelection?.Invoke();
    }
}
