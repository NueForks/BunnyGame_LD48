using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;

public class PlayerCombat : MonoBehaviour
{
    public Player p;
    public PlayerStats stats;
    public Animator anim;
    public float moveTime;
    public float attackDelay;

    public int attackDamages;
    [HideInInspector]
    public bool mute;
    bool busy;
    public SquareGrid grid;
    public Vector2Int coordinates;
    public GameEvent gameOverEvent;
    public Vector3 offset = new Vector3(0, -0.4f, 0);

    [Header("audio")]
    public AudioSource aud;
    public AudioClip sound_move;
    public AudioClip sound_mine;
    public AudioClip sound_hit;

    void PlaySound(AudioClip c)
    {
        aud.Stop();
        aud.clip = c;
        aud.Play();
    }

    void ReadPlayerStats()
    {
        attackDamages = attackDamages + stats.battleDamageBonus;
    }

    private void InitializeCombatPlayer()
    {
        ReadPlayerStats();
        moveTime = p.moveTime;
        mute = true;
    }
    private void Start()
    {
        InitializeCombatPlayer();
    }
    void Update()
    {
        if (mute)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            InputDir(new Vector2Int(1, 0));
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A))
        {
            InputDir(new Vector2Int(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
        {
            InputDir(new Vector2Int(0, 1));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            InputDir(new Vector2Int(0, -1));
        }
    }

    void InputDir(Vector2Int dir)
    {        
        Vector2Int checkedCoord = coordinates + dir;

        if (checkedCoord.x < 0 || checkedCoord.y < 0 || checkedCoord.x >= grid.cells.GetLength(0) || checkedCoord.y >= grid.cells.GetLength(1))
        {
            return;
        }
        if (!busy)
        {
            if (dir.x != 0 && Mathf.Sign(dir.x) == Mathf.Sign(transform.localScale.x))
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        GridCell c = grid.cells[checkedCoord.x, checkedCoord.y];
        if (c.ground == GridCell.GroundType.Empty)
        {
            if (dir.y == 0)
            {
                Move(c);
            }
        }
        else
        {
            if (!busy)
            {
                if (dir.y == 1)
                {
                    anim.SetTrigger("mineup");
                }
                else if (dir.y == -1)
                {
                    anim.SetTrigger("minedown");
                }
                else if (dir.x != 0)
                {
                    anim.SetTrigger("mineside");
                }
            }
            Attack(c);
        }
    }

    void Attack(GridCell targetCell)
    {
        if (busy)
        {
            return;
        }
        
        if(targetCell.ground == GridCell.GroundType.Combat_Enemy)
        {
            Enemy e = targetCell.enemy;
            if (e == null)
            {
                return;
            }
            e.ChangeHealth(-attackDamages);
            StartCoroutine(AttackDelay());
            PlaySound(sound_mine);
        }
        
    }

    public void Move(GridCell targetCell)
    {
        if (!busy)
        {
            anim.SetTrigger("move");
            StartCoroutine(Moving(targetCell));
            PlaySound(sound_move);
        }
    }

    IEnumerator Moving(GridCell targetCell)
    {
        busy = true;
        Vector2 startPos = transform.position;
        Vector2 targetPos = targetCell.transform.position + offset;
        float counter = 0;
        while (counter < moveTime)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0f, moveTime);
            float t = Ease.EaseOut(counter / moveTime);
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return new WaitForEndOfFrame();
        }
        coordinates = targetCell.coordinates;
        busy = false;
    }

    IEnumerator AttackDelay()
    {
        busy = true;
        float counter = 0;
        while(counter<attackDelay)
        {
            counter += Time.deltaTime;
            yield return null;
        }
        busy = false;

    }

    public void ChangeHealth(int amount)
    {
        p.ChangeHealth(amount);

        if (amount < 0)
        {
            anim.SetTrigger("hit");
            PlaySound(sound_hit);
        }
        else
        {
            //p.healthAnim.SetTrigger("heal");
        }
        if (p.health == 0)
        {
            mute = true;
            anim.SetTrigger("die");
            gameOverEvent.Raise();
        }

    }

}
