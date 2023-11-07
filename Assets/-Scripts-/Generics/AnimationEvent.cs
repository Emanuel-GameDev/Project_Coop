using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationEvent : MonoBehaviour
{
    //[SerializeField] UnityEvent event1;
    //[SerializeField] UnityEvent event2;
    //[SerializeField] UnityEvent event3;
    //public void CallEvent1()
    //{
    //    event1?.Invoke();
    //}

    //public void CallEvent2()
    //{
    //    event2?.Invoke();
    //}

    //public void CallEvent3()
    //{
    //    event3?.Invoke();
    //}

    [SerializeField] List<UnityEvent> events;

    public void CallEvent(int index)
    {
        if (index >= 0 && index < events.Count)
        {
            events[index]?.Invoke();
        }
        else
        {
            Debug.LogError("Index out of range.");
        }
    }
}
