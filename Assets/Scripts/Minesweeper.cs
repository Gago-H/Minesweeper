using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Minesweeper : MonoBehaviour
{
    public Material cellMaterial;

    public Material tile1;
    public Material tile2;
    public Material tile3;
    public Material tile4;
    public Material tile5;
    public Material tile6;
    public Material tile7;
    public Material tile8;

    public Material Mine;
    public Material MineExploded;
    public Material Flag;


    public static int row = 9;
    public static int col = 9;

    public bool lost = false;
    public bool won = false;

    public int selAm = CellData.selectedAmount;

    static int count = row * col;
    static int bombCount = count * 15 / 100;
    int nonBombCount = count - bombCount;

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
                go.transform.Rotate(Vector3.up, 180);
                go.transform.name = $"[{i},{j}]";

                go.transform.GetComponent<Renderer>().material = cellMaterial;
                go.transform.AddComponent<CellData>().cellVal = 0;
                go.transform.GetComponent<CellData>().pos = new Vector2Int(i, j);
                go.transform.GetComponent<CellData>().revealed = false;
                go.transform.GetComponent<CellData>().flagged = false;
                go.transform.GetComponent<CellData>().exploded = false;
                go.transform.GetComponent<CellData>().notHere = false;
                //go.transform.GetComponent<CellData>().isBomb = false;

                v[i, j] = go;
            }
        }

        PlaceBomb();// PSEUDO RANDOMLY PLACES BOMBS

        //PlaceCellValues();// CHECKS ADJACENT CELLS FOR NUMBERING CELLS
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
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                //v[x, y].transform.GetComponent<CellData>().first = true;
                
                ReplaceBombs(x, y);
                PlaceCellValues();

                // Recursively reveal an opening area upon first click then change the value,
                // best way of reducing chances to lose for first few clicks
                // min: reveal a corner area 2x2 without any being bombs
                // max reveal a center area 3x3 without any being bombs
                // this is assuming none of the cells within the areas mentioned have cellVal == 0
                RevealRec(x, y);
                selAm = 1;

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Debug.Log($"Cell: {v[i, j]}, Cell value: {v[i, j].transform.GetComponent<CellData>().cellVal}");
                    }
                }
            }
        }// END OF FIRST CLICK

        if (selAm != 0 && Input.GetMouseButtonUp(0) && !lost && !won)
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                var cd = v[x, y].transform.GetComponent<CellData>();


                Debug.Log($"Cell value: {cd.cellVal} IsBomb: {cd.isBomb} Revealed: {cd.revealed} Flagged: {cd.flagged} Exploded: {cd.exploded}");

                if (!cd.flagged)
                {
                    //Debug.Log(cd);
                    //cd.tmpCellValue.text = $"{cd.cellVal}";

                    RevealRec(x, y);

                    if (cd.isBomb)
                    {
                        v[x, y].transform.GetComponent<Renderer>().material = MineExploded;
                        v[x, y].GetComponent<CellData>().exploded = true;

                        // BOOL VALUE TO DEACTIVE ANY FURTHER CLICKS
                        lost = true;

                        for (int i = 0; i < row; i++)
                        {
                            for (int j = 0; j < col; j++)
                            {
                                if (v[i, j].transform.GetComponent<CellData>().isBomb && v[i, j] != v[x, y])
                                {
                                    v[i, j].transform.GetComponent<Renderer>().material = Mine;
                                    v[i, j].GetComponent<CellData>().exploded = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        if (selAm != 0 && Input.GetMouseButtonUp(1) && !lost && !won)
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                Debug.Log($"pos: {x},{y}");

                var cd = v[x, y].transform.GetComponent<CellData>();
                
                if (!cd.revealed)
                {
                    if (!cd.flagged)
                    {
                        cd.flagged = true;
                        //set flag sprite active
                        v[x, y].GetComponent<Renderer>().material = Flag;
                    }
                    else
                    {
                        cd.flagged = false;
                        //deactivate flag sprite
                        v[x, y].transform.GetComponent<Renderer>().material = cellMaterial;
                    }
                }
            }
        }

        if (nonBombCount == 0)
        {
            //Debug.Log($"You win!");
            won = true;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (v[i, j].GetComponent<CellData>().isBomb)
                    {
                        v[i, j].GetComponent<Renderer>().material = Flag;
                    }
                }
            }
            // Update Smiley face to have sunglasses 8)
        }

    }

    void RevealRec(int x, int y)
    {
        if (x < 0 || x >= row || y < 0 || y >= col || v[x, y].GetComponent<CellData>().revealed)
        {
            return; // Base case: cell out of bounds or already revealed
        }

        if (v[x, y].GetComponent<CellData>().isBomb || v[x, y].GetComponent<CellData>().flagged)
        {
            return; // Base case: cell is bomb or flagged
        }

        if (!v[x, y].GetComponent<CellData>().revealed)
        {
            Reveal(x, y);
        }

        if (v[x, y].GetComponent<CellData>().cellVal == 0)
        {
            // If the current cell has a value of 0, recursively reveal its neighbors
            RevealRec(x - 1, y - 1); // BL
            RevealRec(x - 1, y);     // ML
            RevealRec(x - 1, y + 1); // TL
            RevealRec(x, y - 1);     // BM
            RevealRec(x, y + 1);     // TM
            RevealRec(x + 1, y - 1); // BR
            RevealRec(x + 1, y);     // MR
            RevealRec(x + 1, y + 1); // TR
        }
    }

    void Reveal(int x, int y)
    {
        var cv = v[x, y].transform.GetComponent<CellData>();

        switch (cv.cellVal)
        {
            case 1:
                cv.GetComponent<Renderer>().material = tile1;
                break;

            case 2:
                cv.GetComponent<Renderer>().material = tile2;
                break;

            case 3:
                cv.GetComponent<Renderer>().material = tile3;
                break;

            case 4:
                cv.GetComponent<Renderer>().material = tile4;
                break;

            case 5:
                cv.GetComponent<Renderer>().material = tile5;
                break;

            case 6:
                cv.GetComponent<Renderer>().material = tile6;
                break;

            case 7:
                cv.GetComponent<Renderer>().material = tile7;
                break;

            case 8:
                cv.GetComponent<Renderer>().material = tile8;
                break;
            default:
                cv.GetComponent <Renderer>().material = cellMaterial;
                break;
        }

        // Change color of the revealed cell
        var go = v[x, y];
        go.transform.GetComponent<Renderer>().material.color = Color.gray;
        cv.revealed = true;
        nonBombCount--;
    }

    void PlaceBomb()
    {
        int chance = Random.Range(50, 70);
        bool sideSwap = true;
        int start = 0, end = row - 1, side = 0;


        for (int i = start; bombCount != 0; i = side)
        {
            for (int j = 0; j < col; j++)
            {
                var go = v[i, j];
                var cd = go.transform.GetComponent<CellData>();
                if (Random.Range(1, 100) > chance && bombCount != 0 && !cd.isBomb && !cd.notHere)//!cd.revealed && !cd.isBomb && cd.first)
                {
                    cd.isBomb = true;
                    cd.cellVal = -1;
                    chance += 20;
                    bombCount--;
                    //go.transform.GetComponent<Renderer>().material.color = Color.cyan;
                    Debug.Log($"We set a bomb {go.transform.name}, Cell value: {cd.cellVal}");
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
    }

    void PlaceCellValues()
    {
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
    }

    void ReplaceBombs(int x, int y)
    {
        v[x, y].GetComponent<CellData>().cellVal = 0;
        v[x, y].GetComponent<CellData>().notHere = true;
        v[x, y].GetComponent<CellData>().isBomb = false;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < row && j >= 0 && j < col && i <= x + 1 && j <= y + 1)
                {
                    if (v[i, j].GetComponent<CellData>().isBomb)
                    {
                        bombCount++;
                        v[i, j].GetComponent<CellData>().isBomb = false;
                        v[i, j].GetComponent<CellData>().cellVal = 0;
                        Debug.Log($"I took out a bomb!");
                    }

                    v[i, j].GetComponent<CellData>().notHere = true;
                    
                    //Reveal(i, j);
                    //if (v[i, j].GetComponent<CellData>().cellVal == 0)
                    //{
                    //    RevealRec(i, j);
                    //}
                    //RevealRec(i, j);
                }
            }
        }

        PlaceBomb();
    }
}