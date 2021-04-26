using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManager : MonoBehaviour
{
    public int mineDepth;
    public SquareGrid grid;
    public Player player;
    
    [Header("Mine Segments")]
    public MineSegment startSegment;
    public MineSegment endSegment;
    public List<MineSegmentCollection> ms_collections;

    [Header("Elevators")]
    public GameObject elevatorPrefab;
    public Vector3 elevatorOffset;
    public List<ElevatorLevel> elevatorLevels = new List<ElevatorLevel>();
    public List<MineSegmentCollection> ms_elevator_collections;

    private void Start()
    {
        GenerateNewMine();
        InitializePlayer();
    }

    void GenerateNewMine()
    {
        grid.gridSize.y = mineDepth+1;
        grid.cells = new GridCell[grid.gridSize.x, grid.gridSize.y];

        int d = 0;
        for (int y = 0; y < startSegment.structure.GetLength(1); y++) 
        {
            for (int x = 0; x < startSegment.structure.GetLength(0); x++)
            {
                GameObject c_go = Instantiate(startSegment.structure[x, y], grid.transform) as GameObject;
                GridCell c = c_go.GetComponent<GridCell>();
                c.coordinates = new Vector2Int(x, y + d);
                grid.cells[x, y+d] = c;
            }
        }
        d = startSegment.structure.GetLength(1);

        int safety = mineDepth + 5;
        while (d<mineDepth-1 && safety>0)
        {
            safety--;
            if (safety<=0)
            {
                Debug.Log("problem");
                break;
            }
            int depthIndex = 0;
            for(int i = ms_collections.Count-1; i>=0;i--)
            {
                if (d>=ms_collections[i].startDepth)
                {
                    depthIndex = i;
                    break;
                }
            }
            MineSegment ms = ms_collections[depthIndex].GetRandomSegment();
            int addedDepth = 0 ;
            float m = Random.Range(0f, 1f);
            bool mirror = (ms.mirror && m >= 0.5f) ? true : false;
            for (int y = 0; y < ms.structure.GetLength(1); y++)
            {
                if (d + y >= mineDepth-1)
                {
                    break;
                }
                addedDepth++;

                for (int x = 0; x < ms.structure.GetLength(0); x++)
                {
                    int actualX = mirror ? ms.structure.GetLength(0) - 1 - x:x;
                    GameObject c_go = Instantiate(ms.structure[actualX, y], grid.transform) as GameObject;
                    GridCell c = c_go.GetComponent<GridCell>();
                    c.coordinates = new Vector2Int(actualX, y + d);
                    c.grid = grid;
                    grid.cells[actualX, y+d] = c;
                }
            }
            //d += ms.structure.GetLength(1);
            d += addedDepth;

            for (int i = 0;i<elevatorLevels.Count;i++)
            {
                if (elevatorLevels[i].depth<=d && !elevatorLevels[i].created)
                {
                    elevatorLevels[i].created = true;
                   
                    int depthIndex_elevator = 0;
                    for (int j = ms_elevator_collections.Count-1; j >=0; j--)
                    {
                        if (d >= ms_elevator_collections[j].startDepth)
                        {
                            depthIndex_elevator = j;
                            break;
                        }
                    }
                    ms = ms_elevator_collections[depthIndex_elevator].GetRandomSegment();
                    for (int y = 0; y < ms.structure.GetLength(1); y++)
                    {
                        if (d + y >= mineDepth)
                        {
                            break;
                        }
                        for (int x = 0; x < ms.structure.GetLength(0); x++)
                        {
                            GameObject c_go = Instantiate(ms.structure[x, y], grid.transform) as GameObject;
                            GridCell c = c_go.GetComponent<GridCell>();
                            c.coordinates = new Vector2Int(x, y + d);
                            c.grid = grid;
                            grid.cells[x, y + d] = c;
                        }
                    }

                    grid.AllignCells();

                    if (ms.elevatorLeft)
                    {
                        GameObject elevGO = Instantiate(elevatorPrefab, grid.cells[0, ms.structure.GetLength(1)+ d-1].transform.position + elevatorOffset, Quaternion.identity) as GameObject;
                        Elevator e = elevGO.GetComponent<Elevator>();
                        grid.cells[0, ms.structure.GetLength(1) + d-1].elevatorLeft = e;
                    }
                    if (ms.elevatorRight)
                    {
                        Vector3 offset = new Vector3(-elevatorOffset.x, elevatorOffset.y, elevatorOffset.z);
                        GameObject elevGO = Instantiate(elevatorPrefab, grid.cells[grid.cells.GetLength(0)-1, ms.structure.GetLength(1)+ d-1].transform.position + offset, Quaternion.identity) as GameObject;
                        elevGO.transform.localScale = new Vector3(-1*elevGO.transform.localScale.x, 1 * elevGO.transform.localScale.y, 1 * elevGO.transform.localScale.z);
                        Elevator e = elevGO.GetComponent<Elevator>();
                        grid.cells[grid.cells.GetLength(0)-1, ms.structure.GetLength(1) + d-1].elevatorRight = e;
                    }

                    d += ms.structure.GetLength(1);

                    break;
                }
            }            
        }


        for (int y = 0; y < endSegment.structure.GetLength(1); y++)
        {
            for (int x = 0; x < endSegment.structure.GetLength(0); x++)
            {
                GameObject c_go = Instantiate(endSegment.structure[x, y], grid.transform) as GameObject;
                GridCell c = c_go.GetComponent<GridCell>();
                c.coordinates = new Vector2Int(x, y + d);
                grid.cells[x, y + d] = c;
            }
        }

        grid.AllignCells();

    }

    void InitializePlayer()
    {
        player.transform.position = grid.cells[3, 0].transform.position+Vector3.up*6;
        player.Fall(grid.cells[3, 0], 4);
        
    }

}

[System.Serializable]
public class MineSegmentCollection
{
    public int startDepth;
    public List<MineSegment> ms;
    public MineSegment GetRandomSegment()
    {
        float totalWeight = 0;
        foreach (MineSegment m in ms)
        {
            totalWeight += m.weight;
        }
        float p = Random.Range(0f, totalWeight);
        float checkWeight = 0;
        for (int i = 0; i < ms.Count; i++)
        {
            checkWeight += ms[i].weight;
            if (p <= checkWeight)
            {
                return ms[i];
            }
        }
        return ms[0];
    }
}

[System.Serializable]
public class ElevatorLevel
{
    public int depth;
    [HideInInspector]
    public bool created;
}
