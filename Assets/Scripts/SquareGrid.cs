using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SquareGrid : SerializedMonoBehaviour
{
    public GameObject cellPrefab;
    public bool allign = true;

    public Vector2Int gridSize;
    [HideInInspector]
    public GridCell[,] cells = new GridCell[3,3];

    public Vector2 xAxis, yAxis;

    public void Awake()
    {
        //InitializeGrid();
    }

    [Button(ButtonHeight = 30, Name = "Create Grid"), GUIColor(0.4f, 0.8f, 1)]
    public void CreateGrid()
    {
        GridCell[] existingCells = GetComponentsInChildren<GridCell>();
        int cellNumber = gridSize.x * gridSize.y;
        int diff = cellNumber - existingCells.Length;
        
        if (diff>0)
        {
            for(int i =0;i<diff;i++)
            {
                GameObject cell_GO = Instantiate(cellPrefab, transform.position, Quaternion.identity, transform) as GameObject;                
            }
        }
        else if (diff<0)
        {
            for(int i = 0;i<Mathf.Abs(diff);i++)
            {
                DestroyImmediate(existingCells[existingCells.Length-1]);
            }
        }

        GetCells();
        InitializeGrid();
    }

    [Button]
    void GetCells()
    {
        cells = new GridCell[gridSize.x, gridSize.y];
        GridCell[] c = GetComponentsInChildren<GridCell>();
        int i = 0;
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (i>=c.Length)
                {
                    return;
                }
                cells[x, y] = c[i];
                c[i].coordinates = new Vector2Int(x, y);
                i++;
            }
        }
    }

    void InitializeGrid()
    {
        if (allign)
        {
            AllignCells();

        }
        foreach (GridCell cell in cells)
        {
            cell.grid = this;
        }

    }

    [Button]
    public void AllignCells()
    {
        for (int x=0;x<cells.GetLength(0);x++)
        {
            for (int y = 0; y < cells.GetLength(1);y++)
            {
                if (cells[x,y]!= null)
                {
                    cells[x, y].transform.localPosition = new Vector2(x * xAxis.x + y* yAxis.x, x * xAxis.y + y * yAxis.y);
                    cells[x, y].coordinates = new Vector2Int(x, y);
                }
            }
        }
    }
 
}
