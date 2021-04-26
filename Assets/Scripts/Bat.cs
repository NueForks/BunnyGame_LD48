using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    public Enemy e;
    public int damages;
    public float attackTiming;
    float counter;
    bool busy;
    GridCell attackCell;
    
    private void Start()
    {
        counter = 0;
    }

    private void Update()
    {
        if(e.active && !busy)
        {
            counter += Time.deltaTime;
            if (counter >= attackTiming)
            {
                counter -= attackTiming;
                StartCoroutine(Attacking());
            }
        }
    }

    public void StartNewAttackCycle()
    {
        List<GridCell> potentialCells = new List<GridCell>();
        for(int i =1;i<e.grid.cells.GetLength(0)-1;i++)
        {
            GridCell c = e.grid.cells[i, 1];
            if (c.ground == GridCell.GroundType.Empty)
            {
                potentialCells.Add(c);
            }
        }
        if(potentialCells.Count==0)
        {
            return;
        }
        else
        {
            int index = Random.Range(0, potentialCells.Count);
            attackCell = potentialCells[index];
            e.Move(attackCell);
        }
    }

    public IEnumerator Attacking()
    {
        busy = true;
        e.anim.SetTrigger("attack");
        GridCell c = e.grid.cells[attackCell.coordinates.x, attackCell.coordinates.y - 1];        
        yield return new WaitForSeconds(0.6f);
        if (e.combat.pcombat.coordinates == c.coordinates && e.active)
        {
            e.combat.pcombat.ChangeHealth(-damages);
        }
        yield return new WaitForSeconds(1);
        busy = false;
        StartNewAttackCycle();

    }
}
