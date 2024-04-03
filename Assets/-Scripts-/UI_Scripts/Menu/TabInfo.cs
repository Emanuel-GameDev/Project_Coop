using UnityEngine;

using UnityEngine.UI;

public class TabInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject tabRoot;
    public GameObject TabRoot => tabRoot;

    [SerializeField]
    private Button connectedButton;
    public Button ConnectedButton => connectedButton;

    [SerializeField]
    private GameObject defaultFirstObjectSelected;
    private GameObject firstObjectSelected;
    public GameObject FirstObjectSelected
    {
        get
        {
            if (firstObjectSelected == null)
                return defaultFirstObjectSelected;
            else
                return firstObjectSelected;
        }
        set
        {
            firstObjectSelected = value;
        }
    }

}
