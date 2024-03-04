using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeGridSize(int m, int n, int bc);
    public static event ChangeGridSize OnChangeGridSize;

    public TMP_InputField userInput;

    public void ButtonClicked(int id)
    {
        Debug.Log($"You clicked button with id of {id}");

        switch (id)
        {
            case 0:
                OnChangeGridSize?.Invoke(9, 9, 10);
                break;
            case 1:
                OnChangeGridSize?.Invoke(16, 16, 40);
                break;
            case 2:
                OnChangeGridSize?.Invoke(16, 30, 99);
                break;
            case 3:
                var d = Convert.ToInt32(userInput.text);
                var d2 = Convert.ToInt32(userInput.text);
                if (d2 < 8) d2 = 8;
                OnChangeGridSize?.Invoke(d, d2, d);
                break;
        }

    }
}
