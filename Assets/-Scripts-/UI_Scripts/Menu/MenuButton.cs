using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButton : Button
{
    public GameObject highlight;

    protected override void Start()
    {
        base.Start();
        highlight = GetComponentInChildren<Highlight>().gameObject;
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        if(state == SelectionState.Highlighted)
        {
            DoStateTransition(SelectionState.Selected, true);
        }
        if(state == SelectionState.Normal)
        {
            highlight.SetActive(false);
        }
    }


}