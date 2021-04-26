using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    int health;
    public SpriteRenderer healthBar;
    float healthBarMaxWidth;
    public Animator anim;
    public float moveTime;
    GridCell currentCell;
    GridCell.GroundType previousGround;
    public bool active;
    [HideInInspector]
    public SquareGrid grid;

    IEnumerator moveRoutine;
    [HideInInspector]
    public Combat combat;
    [HideInInspector]
    public float startDelay;
    public Vector2 cellOffset;
    public UnityEngine.Events.UnityEvent onActivate;

    public void InitializeEnemy(GridCell startCell, Combat c, float delay)
    {
        grid = c.grid;
        combat = c;
        currentCell = startCell;
        currentCell.enemy = this;
        previousGround = startCell.ground;
        startCell.ground = GridCell.GroundType.Combat_Enemy;
        healthBarMaxWidth = healthBar.size.y;
        health = maxHealth;
        active = false;
        startDelay = delay;
    }

    public void Move(GridCell targetCell)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }
        moveRoutine = Moving(targetCell);
        StartCoroutine(moveRoutine);
    }

    IEnumerator Moving(GridCell targetCell)
    {
        currentCell.enemy = null;
        currentCell.ground = previousGround;
        Vector2 startPos = transform.position;
        Vector2 targetPos = targetCell.transform.position + new Vector3(cellOffset.x, cellOffset.y, 0);
        float counter = 0;
        float mt = moveTime * Mathf.Abs(currentCell.coordinates.x - targetCell.coordinates.x);
        while (counter < mt)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0f, mt);
            float t = Ease.EaseOut(counter / mt);
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return new WaitForEndOfFrame();
        }
        currentCell = targetCell;
        currentCell.enemy = this;
        previousGround = currentCell.ground;
        currentCell.ground = GridCell.GroundType.Combat_Enemy;
        //coordinates = targetCell.coordinates;
    }

    public void ChangeHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        float t = ((float)health) / ((float)maxHealth);
        healthBar.size = new Vector2(healthBar.size.x, Mathf.Lerp(0, healthBarMaxWidth, t));
        if (amount < 0)
        {
            anim.SetTrigger("hit");
        }
        else
        {
            anim.SetTrigger("heal");
        }
        if (health == 0)
        {
            Kill();
        }

    }

    public void Kill()
    {
        active = false;
        anim.SetTrigger("death");
        currentCell.ground = previousGround;
        currentCell.enemy = null;
        combat.KillEnemy(this);
    }

    public IEnumerator ActivationDelay()
    {
        yield return new WaitForSeconds(startDelay);
        active = true;
        onActivate.Invoke();
    }
}
