using UnityEngine;

public class SubTabInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject subTabRoot;
    public GameObject SubTabRoot => subTabRoot;

    [SerializeField]
    private TabSelection connectedButton;
    public TabSelection ConnectedButton => connectedButton;

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

    public void Inizialize()
    {

    }

    public void SelectTabButton()
    {
        if (connectedButton != null)
            connectedButton.Select();
    }
    public void DeselectTabButton()
    {
        if (connectedButton != null)
            connectedButton.Deselect();
    }

}
