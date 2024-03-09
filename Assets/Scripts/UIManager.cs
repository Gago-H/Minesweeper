using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public delegate void ChangeGridSize(int m, int n, int bc);
    public static event ChangeGridSize OnChangeGridSize;

    public GameObject UI;
    public GameObject winText;
    public GameObject loseText;
    public GameObject resetButton;
    public GameObject exitButton;

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
                int rowInput = string.IsNullOrEmpty(userInput.text) ? 9 : Convert.ToInt32(userInput.text);
                int colInput = string.IsNullOrEmpty(userInput2.text) ? 9 : Convert.ToInt32(userInput2.text);
                int mineCount = string.IsNullOrEmpty(userInput3.text) ? 10 : Convert.ToInt32(userInput3.text);
                int bigger, smaller;

                if (colInput >= rowInput)
                {
                    bigger = colInput;
                    smaller = rowInput;
                    if (bigger < 8 && smaller < 8)
                    {
                        bigger = 8;
                    }
                    if (bigger > 50)
                    {
                        bigger = 50;
                    }
                    if (smaller > 50)
                    {
                        smaller = 50;
                    }
                    if (mineCount > (bigger * smaller))
                    {
                        mineCount = (bigger * smaller) - 1;
                    }
                }
                else
                {
                    smaller = colInput;
                    bigger = rowInput;
                    if (bigger < 8 && smaller < 8)
                    {
                        bigger = 8;
                    }
                    if (bigger > 50)
                    {
                        bigger = 50;
                    }
                    if (smaller > 50)
                    {
                        smaller = 50;
                    }
                    if (mineCount > (bigger * smaller))
                    {
                        mineCount = (bigger * smaller) - 1;
                    }
                }

                OnChangeGridSize?.Invoke(bigger, smaller, mineCount);
                Hide();
                AdjustCameraView(smaller, bigger);
                resetButton.SetActive(true);
                break;
            case 4:
                resetButton.SetActive(false);
                winText.SetActive(false);
                loseText.SetActive(false);
                UI.SetActive(true);
                OnChangeGridSize?.Invoke(0, 0, -1);
                break;
            case 5:
                ExitGame();
                break;
        }

    }
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

    private void AdjustCameraView(int columns, int rows)
    {
        camera.transform.position = new Vector3((float)rows/2 - 0.5f,
                                               ((float)rows + (float)columns)/2 + ((float)rows * .1f),
                                                (float)columns/2 - 0.5f); 
    }
}
