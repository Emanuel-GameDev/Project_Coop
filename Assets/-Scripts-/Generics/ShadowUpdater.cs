using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowUpdater : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    MeshRenderer meshRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Update()
    {
        if (spriteRenderer != null)
        {
            meshRenderer.material.mainTexture = spriteRenderer.sprite.texture;
        }
    }
}
