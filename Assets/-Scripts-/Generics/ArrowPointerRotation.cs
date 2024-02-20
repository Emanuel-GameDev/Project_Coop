using System;
using UnityEngine;

public class ArrowPointerRotation : MonoBehaviour
{
    [SerializeField] 
    private PlayerCharacter character;
    [SerializeField]
    private float rotationSpeed = 5f;


    void Start()
    {
        if(character == null)
            character = GetComponentInParent<PlayerCharacter>();
    }

    void Update()
    {
        Rotate(character.LastDirection);
    }

    private void Rotate(Vector2 direction)
    {
        if (direction.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
