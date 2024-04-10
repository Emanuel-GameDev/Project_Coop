using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AutoSelectedButtonSwapper : MonoBehaviour
{
    [SerializeField]
    GameObject defaultSelectedObject;

    GameObject lastSelected;

    private void OnEnable()
    {
        if (EventSystem.current != null)
        {
            if (lastSelected != null)
            {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
            else if (defaultSelectedObject != null)
            {

                EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
            }
        }
    }

    private void OnDisable()
    {
        if(EventSystem.current != null)
            lastSelected = EventSystem.current.currentSelectedGameObject;
    }
}
