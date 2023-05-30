using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Winner : ChessBoard
{
    public TMP_Text _WinnerText_;

    public void Awake()
    {

    }

    private void Update()
    {
        Debug.Log("The turn is " + _Turn_);
        _WinnerText_.text = "The winner is " + (_Turn_ ? "Black" : "White");
        _WinnerText_.color = (_Turn_ ? Color.black : Color.white);
    }
}
