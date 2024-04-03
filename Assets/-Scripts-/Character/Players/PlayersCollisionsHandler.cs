using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersCollisionsHandler : MonoBehaviour
{
    private void Start()
    {
        Physics2D.IgnoreCollision(gameObject.GetComponentInChildren<PlayersCollisionsHandler>().GetComponent<CircleCollider2D>(), gameObject.GetComponentInChildren<PlayersCollisionsHandler>().GetComponentInChildren<Rigidbody2D>().GetComponent<CircleCollider2D>());
    }

}
