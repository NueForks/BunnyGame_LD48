using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    float timer = 3f;
    public bool hit;
    public Player p;
    public Animator anim;
    public GridCell cell;
    public int damages;
    bool paused;
    public AudioSource aud;

    public void HitBomb()
    {
        if (hit)
        {
            Explode();
        }
        else
        {
            hit = true;
            anim.SetTrigger("hit");
            StartCoroutine(BombTimer());
        }
    }

    void Explode()
    {
        aud.Play();
        StopAllCoroutines();
        anim.SetTrigger("explode");
        List<Vector2Int> coords = new List<Vector2Int>();
        coords.Add(cell.coordinates + new Vector2Int(1, 0));
        coords.Add(cell.coordinates + new Vector2Int(1, 1));
        coords.Add(cell.coordinates + new Vector2Int(1, -1));
        coords.Add(cell.coordinates + new Vector2Int(-1, 0));
        coords.Add(cell.coordinates + new Vector2Int(-1, 1));
        coords.Add(cell.coordinates + new Vector2Int(-1, -1));
        coords.Add(cell.coordinates + new Vector2Int(0, 1));
        coords.Add(cell.coordinates + new Vector2Int(0, -1));

        for (int i = 0; i < coords.Count; i++)
        {
            if (coords[i].x < 0 || coords[i].y < 0 || coords[i].x >= cell.grid.cells.GetLength(0) || coords[i].y >= cell.grid.cells.GetLength(1))
            {
                continue;
            }
            else
            {
                GridCell c = cell.grid.cells[coords[i].x, coords[i].y];
                if (c.ground != GridCell.GroundType.Boss && c.ground != GridCell.GroundType.Unbreakable)
                {
                    if (c.ground != GridCell.GroundType.Empty)
                    {
                        c.DestroyCell();
                        if (c.ground == GridCell.GroundType.Bomb)
                        {
                            c.GetComponent<Bomb>().anim.gameObject.SetActive(false);
                        }
                    }
                    if (p.coordinates == coords[i] && !p.mute)
                    {
                        p.ChangeHealth(-p.bombDamages);
                        p.CheckIfGround();
                    }
                }
            }
        }
        cell.DestroyCell();
    }

    IEnumerator BombTimer()
    {
        float counter = 0;
        while(counter < timer && !paused)
        {
            counter += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        Explode();
    }

}
