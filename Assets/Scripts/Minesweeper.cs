using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
public class Minesweeper : MonoBehaviour
{
    public Material cellMaterial;

    public GameObject canvas;
    public GameObject reset;
    public GameObject Exit;
    public GameObject Win;
    public GameObject Lose;

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

    GameObject[,] v;

    public static int row;
    public static int col;

    public bool GameStart = false;

    static int count;
    static int bombCount;
    int nonBombCount;

    public bool firstClick = true;
    public int notHereCount = 0;
    public int cellNum = 0;

    private void OnEnable()
    {
        UIManager.OnChangeGridSize += UIManager_OnChangeGridSize;
    }

    private void UIManager_OnChangeGridSize(int m, int n, int bc)
    {
        //Debug.Log($"Bombcount sent from UIManager: {bc}");
        if (bc == -1)
        {
            DestroyCells();
            notHereCount = 0;
            bombCount = 0;
            row = 0;
            col = 0;
            GameStart = false;
            firstClick = true;
            return;
        }

        row = m;
        col = n;
        count = row * col;
        bombCount = bc;
        nonBombCount = count - bombCount;

        v = new GameObject[m, n];
        CreateBoard(m, n);
        //Debug.Log($"Bombcount after {bombCount}");
        PlaceBomb(bc);
        Debug.Log($"GAME STARTED NotHereCount: {notHereCount}, NonBombCount: {nonBombCount}");

        //Debug.Log($"THIS IS AFTER START --> NotHereCount: {notHereCount}, NonBombCount: {nonBombCount}");

        GameStart = true;
    }

    private void OnDisable()
    {
        UIManager.OnChangeGridSize -= UIManager_OnChangeGridSize;
    }

    RaycastHit tmphitHighlight;




    // Start is called before the first frame update
    void Start()
    {
        reset.SetActive(false);
        Win.SetActive(false);
        Lose.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        if (!GameStart) return;

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
        if (firstClick && Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                //v[x, y].transform.GetComponent<CellData>().first = true;

                int g = ReplaceBombs(x, y, bombCount);
                Debug.Log(g);
                PlaceBomb(g);
                PlaceCellValues();

                // Recursively reveal an opening area upon first click then change the value,
                // best way of reducing chances to lose for first few clicks
                // min: reveal a corner area 2x2 without any being bombs
                // max reveal a center area 3x3 without any being bombs
                // this is assuming none of the cells within the areas mentioned have cellVal == 0
                RevealRec(x, y);
                firstClick = false;

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        //Debug.Log($"Cell: {v[i, j]}, Cell value: {v[i, j].transform.GetComponent<CellData>().cellVal}");
                    }
                }
            }
        }// END OF FIRST CLICK

        if (!firstClick && Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                var cd = v[x, y].transform.GetComponent<CellData>();


                //Debug.Log($"Cell value: {cd.cellVal} IsBomb: {cd.isBomb} Revealed: {cd.revealed} Flagged: {cd.flagged} Exploded: {cd.exploded}");

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
                        GameStart = false;
                        Lose.SetActive(true);
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

        if (!firstClick && Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(ray, out tmphitHighlight, 100))
            {
                int x = tmphitHighlight.transform.GetComponent<CellData>().pos.x;
                int y = tmphitHighlight.transform.GetComponent<CellData>().pos.y;

                //Debug.Log($"pos: {x},{y}");

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
            GameStart = false;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    if (v[i, j].GetComponent<CellData>().isBomb && v[i, j] != null)
                    {
                        v[i, j].GetComponent<Renderer>().material = Flag;
                    }
                }
            }
            Win.SetActive(true);
            // Update Smiley face to have sunglasses 8)
        }

    }

    void DestroyCells()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (v[i, j] != null)
                {
                    Destroy(v[i, j]);
                }
            }
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

        Reveal(x, y);

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
                cv.GetComponent<Renderer>().material = cellMaterial;
                break;
        }

        // Change color of the revealed cell
        var go = v[x, y];
        go.transform.GetComponent<Renderer>().material.color = Color.gray;
        cv.revealed = true;
        nonBombCount--;
        Debug.Log($"Cell: {v[x, y]}, revealed this cell; NBC: {nonBombCount}");
    }

    void CreateBoard(int r, int c)
    {
        Debug.Log($"{r},{c}");
        for (int i = 0; i < r; i++)
        {
            for (int j = 0; j < c; j++)
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
    }
    void PlaceBomb(int bc)
    {
        bombCount = bc;

        //Debug.Log($"Bombcount from PlaceBomb method: {bombCount}");

        while (bombCount > 0)
        {
            int x = UnityEngine.Random.Range(0, row);
            int y = UnityEngine.Random.Range(0, col);
            var b = v[x, y];
            var bd = v[x, y].GetComponent<CellData>();
            if (!bd.isBomb && !bd.notHere)
            {
                bd.isBomb = true;
                bd.cellVal = -1;
                bombCount--;
                Debug.Log($"We set a bomb {b.transform.name}, Cell value: {bd.cellVal}");
            }
        }
        //Debug.Log($"Bombcount from PlaceBomb method after setting all bombs: {bombCount}");

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

    int ReplaceBombs(int x, int y, int bc)
    {
        //Debug.Log($"row: {x}, col: {y}, bombcount: {bc}");
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i >= 0 && i < row && j >= 0 && j < col)
                {
                    if (v[i, j].GetComponent<CellData>().isBomb)
                    {
                        bc++;
                        v[i, j].GetComponent<CellData>().isBomb = false;
                        v[i, j].GetComponent<CellData>().cellVal = 0;
                        Debug.Log($"I took out a bomb! {v[i, j]}");
                    }

                    v[i, j].GetComponent<CellData>().notHere = true;
                    notHereCount++;
                }
            }
        }

        //if (nonBombCount < notHereCount)
        //{
        //    for (int i = 0; i < row; i++)
        //    {
        //        for (int j = 0; j < col; j++)
        //        {
        //            if (!(i == x && j == y))
        //            {
        //                v[i, j].GetComponent<CellData>().notHere = false;
        //                notHereCount--;
        //            }
        //        }
        //    }
        //}
        Debug.Log($"BEFORE WHILE NotHereCount: {notHereCount}, NonBombCount: {nonBombCount}");

        while (nonBombCount < notHereCount)
        {
            int a = Random.Range(x - 1, x + 2);
            int b = Random.Range(y - 1, y + 2);

            if (a >= 0 && a < row && b >= 0 && b < col && v[a, b].GetComponent<CellData>().notHere && !(a == x && b == y))
            {
                v[a, b].GetComponent<CellData>().notHere = false;
                notHereCount--;
            }
        }

        Debug.Log($"AFTER WHILE Notherecount: {notHereCount}, Nonbombcount: {nonBombCount}");
        //PlaceBomb(bc);
        return bc;
    }
}