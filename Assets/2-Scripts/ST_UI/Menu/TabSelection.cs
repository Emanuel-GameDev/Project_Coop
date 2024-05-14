using UnityEngine;
using UnityEngine.UI;

public class TabSelection : MonoBehaviour
{
    //[SerializeField]
    //Animator animator;

    //static string NORMAL = "Normal";
    //static string SELECTED = "Selected";


    //[SerializeField]
    //Image targetGraphics;

    //[SerializeField]
    //Sprite selectedSprite;

    //Sprite defaultSprite;

    [SerializeField]
    RectTransform targetRectTransform;
    [SerializeField]
    Vector2 offsetPosition =new(0,-25);

    private void Awake()
    {
        //defaultSprite = targetGraphics.sprite;

        //if (animator == null)
        //    animator = GetComponent<Animator>();

        //animator.SetTrigger(NORMAL);
        if (targetRectTransform == null)
            targetRectTransform = GetComponentInChildren<RectTransform>();
    }

    public void Select()
    {
        //if (defaultSprite == null)
        //    defaultSprite = targetGraphics.sprite;

        //targetGraphics.sprite = selectedSprite;
        //animator.SetTrigger(SELECTED);

        targetRectTransform.anchoredPosition = Vector2.zero;
    }

    public void Deselect()
    {
        //targetGraphics.sprite = defaultSprite;
        //animator.SetTrigger(NORMAL);
        targetRectTransform.anchoredPosition = offsetPosition;
    }

}
