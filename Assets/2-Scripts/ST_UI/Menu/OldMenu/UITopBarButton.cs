using UnityEngine;
using UnityEngine.UI;

public class UITopBarButton : MonoBehaviour
{
    [SerializeField]
    RectTransform targetRectTransform;
    [SerializeField]
    Vector2 offsetPosition =new(0,-25);

    private void Awake()
    {
        if (targetRectTransform == null)
            targetRectTransform = GetComponentInChildren<RectTransform>();
    }

    public void Select()
    {
        targetRectTransform.anchoredPosition = Vector2.zero;
    }

    public void Deselect()
    {
        targetRectTransform.anchoredPosition = offsetPosition;
    }

}
