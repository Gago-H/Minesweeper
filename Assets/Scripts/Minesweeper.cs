using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Minesweeper : MonoBehaviour
{
    public Material cellMaterial;

    public static int row = 9;
    public static int col = 9;

    public int selAm = CellData.selectedAmount;

    static int count = row * col;
    int bombCount = count * 15 / 100;

    public int cellNum = 0;

    RaycastHit tmphitHighlight;

    //private void OnEnable()
    //{
    //   UIManager.OnChangeSize += UIManager_OnchangeSize;
    //}

    //private void UIManager_OnChangeSize OnDisable()
    //{
    GameObject[,] v = new GameObject[row, col];


    // Start is called before the first frame update
    void Start()
    {

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = new Vector3(i, 0, j);
                go.transform.localScale = new Vector3(1, 0.1f, 1);
                go.transform.name = $"[{i},{j}]";

                go.transform.GetComponent<Renderer>().material = cellMaterial;
                go.transform.AddComponent<CellData>().cellVal = 0;
                go.transform.AddComponent<CellData>().pos = new Vector3Int(i, 0, j);
                //var cd = go.transform.AddComponent<CellData>();

                v[i, j] = go;


                // place bomb based on logic ...


            }
        }

        // compute the cell values 

    }


    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                Debug.Log($"We got a hit {tmphitHighlight.transform.name}," +
                    $" Bomb status: {tmphitHighlight.transform.GetComponent<CellData>().isBomb}");
            }
        }

        // PSEUDO RANDOM BOMB PLACEMENT ON FIRST CLICK + FIRST CLICK CANNOT BE BOMB
        if (selAm == 0 && Input.GetMouseButtonUp(0))
        {
            selAm = 1;
            int chance = Random.Range(50, 70);

            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                bool sideSwap = true;
                int start = 0, end = row - 1, side = 0;

                tmphitHighlight.transform.GetComponent<CellData>().first = true;

                for (int i = start; bombCount != 0; i = side)
                {
                    for (int j = 0; j < col; j++)
                    {
                        var go = v[i, j];
                        var cd = go.transform.GetComponent<CellData>();
                        if (Random.Range(1, 100) > chance && bombCount != 0 && !cd.first)
                        {
                            cd.isBomb = true;
                            cd.cellVal = -1;
                            chance += 20;
                            bombCount--;
                            go.transform.GetComponent<Renderer>().material.color = Color.red;
                            Debug.Log($"We set a bomb {go.transform.name}, Cell value: {cd.cellVal}");
                        }
                        else
                        {
                            cd.isBomb = false;
                            //chance += 5;
                        }
                    }
                    chance = Random.Range(50, 70);

                    switch (sideSwap)
                    {
                        case true:
                            sideSwap = false;
                            start++;
                            side = end;
                            break;
                        case false:
                            sideSwap = true;
                            end--;
                            side = start;
                            break;
                    }

                }

                // CHECKS ADJACENT CELLS FOR NUMBERING CELLS
                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        var go = v[i, j];
                        var cd = go.transform.GetComponent<CellData>();

                        int[,] adj = { { i - 1, j - 1 }, { i, j - 1 }, { i + 1, j - 1 }, { i - 1, j }, { i + 1, j }, { i - 1, j + 1 }, { i, j + 1 }, { i + 1, j + 1 } };

                        //Debug.Log(adj.GetLength(0));
                        if (cd.isBomb)
                        {

                            for (int k = 0; k < adj.GetLength(0); k++)
                            {
                                int m = adj[k, 0];
                                int n = adj[k, 1];
                                //Debug.Log($"{m},{n}");

                                if (m >= 0 && n >= 0 && m < row && n < col && !(m == i && n == j) && !v[m, n].transform.GetComponent<CellData>().isBomb)
                                {
                                    var M = v[m, n];
                                    M.transform.GetComponent<CellData>().cellVal++;
                                    v[m, n] = M;
                                    //Debug.Log($"Cell: {v[m, n]}, Cell value: {v[m, n].transform.GetComponent<CellData>().cellVal}");
                                }

                            }
                        }


                    }
                }

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Debug.Log($"Cell: {v[i, j]}, Cell value: {v[i, j].transform.GetComponent<CellData>().cellVal}");
                    }
                }
            }
        }// END OF FIRST CLICK

        if (selAm != 0 && Input.GetMouseButtonUp(0))
        {

        }

    }

    //GameObject GetCell()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    if (Physics.Raycast(ray, out tmphitHighlight, 100))
    //    {
    //        GameObject clicked = tmphitHighlight.collider.gameObject;
    //        int r = clicked.
    //        return tmphitHighlight.transform.GetComponent<Minesweeper>().v[row, col];
    //    }

    //    return null;
    //}

    void Reveal(Vector3 p)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out tmphitHighlight, 100))
        {
            //i = tmphitHighlight.transform.GetComponent<Minesweeper>().v[p];
            //var go = v[i, j];
        }
    }
}