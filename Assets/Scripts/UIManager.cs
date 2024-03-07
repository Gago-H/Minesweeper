using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using static Minesweeper;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeGridSize(int m, int n, int bc);
    public static event ChangeGridSize OnChangeGridSize;

    public GameObject UI;
    public GameObject resetButton;
    public new Camera camera;

    private void Hide()
    {
        UI.SetActive(false);
    }

    public TMP_InputField userInput, userInput2, userInput3;

    public void ButtonClicked(int id)
    {
        Debug.Log($"You clicked button with id of {id}");

        switch (id)
        {
            case 0:
                OnChangeGridSize?.Invoke(9, 9, 10);
                Hide();
                AdjustCameraView(9, 9);
                resetButton.SetActive(true);
                break;
            case 1:
                OnChangeGridSize?.Invoke(16, 16, 40);
                Hide();
                AdjustCameraView(16, 16);
                resetButton.SetActive(true);
                break;
            case 2:
                OnChangeGridSize?.Invoke(30, 16, 99);
                Hide();
                AdjustCameraView(16, 30);
                resetButton.SetActive(true);
                break;
            case 3:
                int d = string.IsNullOrEmpty(userInput.text) ? 9 : Convert.ToInt32(userInput.text);
                int d2 = string.IsNullOrEmpty(userInput2.text) ? 9 : Convert.ToInt32(userInput2.text);
                int mc = string.IsNullOrEmpty(userInput3.text) ? 10 : Convert.ToInt32(userInput3.text);

                OnChangeGridSize?.Invoke(d2, d, mc);
                Hide();
                AdjustCameraView(d, d2);
                resetButton.SetActive(true);
                break;
            case 4:
                resetButton.SetActive(false);
                UI.SetActive(true);
                break;
        }

    }

    private void AdjustCameraView(int columns, int rows)
    {
        //int big;
        //if (rows >= columns)
        //{
        //    big = rows;
        //}
        //else big = columns;
        camera.transform.position = new Vector3((float)rows/2 - 0.5f, ((float)rows + (float)columns)/2, (float)columns/2 - 0.5f); 
    }
}
