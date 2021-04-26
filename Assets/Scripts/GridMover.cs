using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;

public class GridMover : MonoBehaviour
{
    public Vector2 cellOffset;
    public bool canMove = true;
    public int moveRange;
    public float moveTime;
    //IntReference movementModifier;


    GridCell currentCell;

    bool moving;
    List<IEnumerator> movements = new List<IEnumerator>();

    public void SetOnCell(GridCell cell)
    {
        if (moving)
        {
            StopCoroutine(movements[0]);
            movements.Clear();
        }
        transform.position = (Vector2)cell.transform.position + cellOffset;
        currentCell = cell;
    }


    public void MoveToCell(GridCell targetCell)
    {
        if (canMove)
        {
            movements.Add(Move(targetCell));
        }

        if (!moving)
        {
            StartCoroutine(movements[0]);            
        }
    }



    IEnumerator Move(GridCell targetCell)
    {
        moving = true;
        float t = 0;
        float currentLerpTime = 0;
        while (t<1)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime/moveTime;
            t = Ease.SmoothStep(t);
            Vector2 newPos = Vector2.Lerp((Vector2)currentCell.transform.position + cellOffset, (Vector2)targetCell.transform.position + cellOffset, t);
            transform.position = newPos;
            yield return new WaitForEndOfFrame();
        }
        moving = false;
        currentCell = targetCell;
        movements.RemoveAt(0);

        if (movements.Count!=0)
        {
            StartCoroutine(movements[0]);
        }
    }




}
