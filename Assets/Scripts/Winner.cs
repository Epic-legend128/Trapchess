using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Winner : ChessBoard
{
    private TMP_Text _text_;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _text_.text = (_Turn_ ? "White Won !" : "Black Won !");
        _text_.color = Color.white;
    }
}