using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellData : MonoBehaviour
{
    public bool isBomb;
    public bool first;
    public int cellVal = 0;

    public Vector2Int pos;

    public bool revealed;
    public bool flagged;
    public bool exploded;
    public bool notHere;

}