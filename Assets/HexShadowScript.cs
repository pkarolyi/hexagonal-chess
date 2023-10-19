using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HexShadowScript : MonoBehaviour
{
    void Update()
    {
        Color hexColor = transform.parent.Find("HexSprite").GetComponent<SpriteRenderer>().color;
        Color borderColor = new Color(hexColor.r * 0.9f, hexColor.g * 0.9f, hexColor.b * 0.9f);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = borderColor;
    }
}
