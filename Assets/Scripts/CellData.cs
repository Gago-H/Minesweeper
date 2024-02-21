using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellData : MonoBehaviour
{
    public bool isBomb;
    public bool first;
    public int cellVal = 0;

    public bool reveal;
    public static int selectedAmount;
    public bool flag;

    public Transform bombred;
    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }
}