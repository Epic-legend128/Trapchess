using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{    

    [SerializeField] private Color baseColor, offsetColor;
    [SerializeField] private SpriteRenderer Renderer;
    [SerializeField] private GameObject Highlight;

    public void init(bool isOffset)
    {
        Renderer.color = isOffset ? baseColor : offsetColor;
    }

    void OnMouseEnter() {
        Highlight.SetActive(true);
    }
    void OnMouseExit() {
        Highlight.SetActive(false);
    }
    
}
