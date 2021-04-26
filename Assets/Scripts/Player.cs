using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;
using TMPro;
using Com.LuisPedroFonseca.ProCamera2D;

public class Player : MonoBehaviour
{
    public PlayerStats stats;

    public SquareGrid grid;
    public Vector2Int coordinates;
    bool busy;
    [Header("Camera")]
    public ProCamera2DShake camShaker;
    public ProCamera2D cam;

    [Header("Animation")]
    public Animator anim;

    [Header("Movement")]
    [HideInInspector]
    public bool mute;
    public float moveTime;
    public float fallDelay;
    public float fallTime;
    public Vector3 offset = new Vector3(0, -0.4f,0);

    [Header("Mining")]
    public int mineDamages;

    [Header("BackPack")]
    public BackPack backPack;

    [Header("Health")]
    public int maxHealth;
    public int health;
    public SpriteRenderer healthBar;
    float maxHealthBarSize;
    public TextMeshPro tmp_health;
    public Animator healthAnim;
    public int carrotHeal;
    public int bombDamages;
    public GameEvent gameOverEvent;

    [Header("Depth")]
    public Transform depthCursor;
    public int maxDepth;
    public float cursorStart, cursorEnd;
    public TextMeshPro tmp_depth;
    public SpriteRenderer rope;
    public float maxRopeSize;
    float startRopeSize;

    [Header("Combat")]
    public Combat combat;
    [HideInInspector]
    public GridCell currentCombatCell;

    [Header("Torch")]
    public float torchConsumeSpeed;
    float torchLife;
    public float maxTorchLife;
    public float torchGain;
    public SpriteRenderer torchBar;
    float maxTorchBarSize;
    public Transform flames;
    float maxFlamesSize;
    public Transform torchMask;
    public float minMaskSize, maxMaskSize;

    [Header("audio")]
    public AudioSource aud;
    public AudioClip sound_move;
    public AudioClip sound_mine;
    public AudioClip sound_hit;
    public AudioClip sound_carrot;
    public AudioClip sound_torch;
    public AudioClip sound_gems;

    void PlaySound(AudioClip c)
    {
        aud.Stop();
        aud.clip = c;
        aud.Play();
    }
    
    void ReadPlayerStats()
    {
        carrotHeal += stats.carotHealBonus;
        bombDamages -= stats.bombDamagesReduction;
        mineDamages += stats.miningDamageBonus;
        maxHealth += stats.healthBonus;
        health = maxHealth;
        torchGain += stats.torchGainBonus;
        maxTorchLife += stats.torchLifeBonus;
        torchLife = maxTorchLife;
        torchConsumeSpeed *= 1 - stats.torchConsumeReductionBonus;

    }
    private void Start()
    {
        Cursor.visible = false;

        ReadPlayerStats();
        maxHealthBarSize = healthBar.size.x;
        maxTorchBarSize = torchBar.size.y;
        maxFlamesSize = flames.transform.localScale.x;
        startRopeSize = rope.size.y;
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
        if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.A))
        {
            InputDir(new Vector2Int(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W))
        {
            InputDir(new Vector2Int(0, -1));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            InputDir(new Vector2Int(0, 1));
        }

        HandleTorch();
    }

    void InputDir(Vector2Int dir)
    {
        if (grid.cells[coordinates.x, coordinates.y].elevatorLeft != null && dir.x == -1)
        {
            EnterElevator(grid.cells[coordinates.x, coordinates.y].elevatorLeft);
            return;
        }
        else if (grid.cells[coordinates.x, coordinates.y].elevatorRight != null && dir.x == 1)
        {
            EnterElevator(grid.cells[coordinates.x, coordinates.y].elevatorRight);
            return;
        }

        Vector2Int checkedCoord = coordinates + dir;        

        if (checkedCoord.x<0||checkedCoord.y<0||checkedCoord.x>=grid.cells.GetLength(0)||checkedCoord.y>=grid.cells.GetLength(1))
        {
            return;
        }
        GridCell c = grid.cells[checkedCoord.x, checkedCoord.y];
        if (!busy)
        {
            if (dir.x != 0 && Mathf.Sign(dir.x) == Mathf.Sign(transform.localScale.x))
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
        if (c.ground == GridCell.GroundType.Empty)
        {
            if (dir.y >= 0)
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
            Mine(c);
        }
    }

    void Mine(GridCell targetCell)
    {
        if (busy)
        {
            return;
        }
        PlaySound(sound_mine);


        bool destroyed = false;

        switch (targetCell.ground)
        {
            case GridCell.GroundType.Monster:
                mute = true;
                combat.FindEncounter(coordinates.y);
                currentCombatCell = targetCell;
                break;
            case GridCell.GroundType.Bomb:
                Bomb b = targetCell.GetComponent<Bomb>();
                b.p = this;
                b.HitBomb();
                break;
            default:
                targetCell.Mine(mineDamages, out destroyed);
                break;
        }

        if (destroyed)
        {
            switch(targetCell.reward)
            {
                case GridCell.RewardType.Carrot:
                    ChangeHealth(carrotHeal);
                    PlaySound(sound_carrot);
                    break;
                case GridCell.RewardType.Torch:
                    GainTorch();
                    PlaySound(sound_torch);
                    break;
                case GridCell.RewardType.Gold:
                    backPack.GainGold(targetCell.rewardAmount);
                    PlaySound(sound_gems);
                    break;
                case GridCell.RewardType.Emerald:
                    backPack.GainEmerald(targetCell.rewardAmount);
                    PlaySound(sound_gems);
                    break;
                case GridCell.RewardType.Ruby:
                    backPack.GainRuby(targetCell.rewardAmount);
                    PlaySound(sound_gems);
                    break;
                case GridCell.RewardType.Diamond:
                    backPack.GainDiamond(targetCell.rewardAmount);
                    PlaySound(sound_gems);
                    break;
            }
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

    public void CheckIfGround()
    {
        Vector2Int checkedCoord = coordinates + new Vector2Int(0, 1);
        if (checkedCoord.x < 0 || checkedCoord.y < 0 || checkedCoord.x > grid.cells.GetLength(0) || checkedCoord.y > grid.cells.GetLength(1))
        {
            return;
        }        
        
        GridCell c = grid.cells[checkedCoord.x, checkedCoord.y];
        if (c.ground == GridCell.GroundType.Empty)
        {
            bool ground = false;
            int height = 1;
            while (!ground)
            {
                if (checkedCoord.y < 0 || checkedCoord.y > grid.cells.GetLength(1))
                {
                    break;
                }
                if (grid.cells[checkedCoord.x, checkedCoord.y].ground == GridCell.GroundType.Empty)
                {
                    height++;
                    c = grid.cells[checkedCoord.x, checkedCoord.y];
                    checkedCoord += new Vector2Int(0, 1);                    
                }
                else
                {
                    ground = true;
                }
            }
            Fall(c, height);
        }
    }

    public void Fall(GridCell targetCell, int height)
    {
        if (!busy)
        {
            StartCoroutine(Falling(targetCell, height));
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
        CheckIfGround();
        UpdateDepth();
    }

    IEnumerator Falling(GridCell targetCell, int height)
    {
        busy = true;
        yield return new WaitForSeconds(fallDelay);
        Vector2 startPos = transform.position;
        Vector2 targetPos = targetCell.transform.position + offset;
        float counter = 0;
        while (counter < fallTime*height)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0f, fallTime * height);
            float t = (counter<fallTime)? Ease.EaseIn(counter / fallTime):counter/ fallTime * height;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return new WaitForEndOfFrame();
        }
        coordinates = targetCell.coordinates;
        busy = false;
        UpdateDepth();
    }

    public void ChangeHealth(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        tmp_health.text = health.ToString();
        float t = (float)health / (float)maxHealth;
        healthBar.size = new Vector2(Mathf.Lerp(0, maxHealthBarSize, t), healthBar.size.y);

        if (amount<0)
        {
            camShaker.Shake(0);
            if (!mute)
            {
                anim.SetTrigger("hit");
                PlaySound(sound_hit);
            }
        }
        else
        {
           // healthAnim.SetTrigger("heal");
        }
        if (health == 0)
        {
            if (!mute)
            {
                gameOverEvent.Raise();
            }
            mute = true;
            anim.SetTrigger("die");
        }

    }

    void EnterElevator(Elevator e)
    {
        cam.FollowVertical = false;
        anim.transform.parent = e.animParent;
        anim.transform.localPosition = Vector2.zero;
        coordinates = new Vector2Int(-1, -1);
        mute = true;
        e.Enter();
    }
    
    void UpdateDepth()
    {
        float t = (float)coordinates.y / (float)maxDepth;

        Vector3 pos = depthCursor.transform.localPosition;
        depthCursor.transform.localPosition = new Vector3(pos.x, Mathf.Lerp(cursorStart, cursorEnd, t), pos.z);
        tmp_depth.text = coordinates.y.ToString() + "m";
        rope.size = new Vector2(rope.size.x, Mathf.Lerp(startRopeSize, maxRopeSize, t));
    }

    void HandleTorch()
    {
        if (mute)
        {
            return;
        }
        torchLife -= Time.deltaTime * torchConsumeSpeed;
        torchLife = Mathf.Clamp(torchLife, 0, maxTorchLife);
        float t = torchLife / maxTorchLife;
        torchBar.size = new Vector2(torchBar.size.x, Mathf.Lerp(0, maxTorchBarSize, t));
        flames.transform.localScale = Vector3.one * Mathf.Lerp( 0.2f, maxFlamesSize, t);
        torchMask.transform.localScale = Vector3.one * Mathf.Lerp(minMaskSize, maxMaskSize, t);
        if (torchLife == 0)
        {
            if (!mute)
            {
                gameOverEvent.Raise();
            }
            mute = true;
            anim.SetTrigger("die");
        }
    }

    public void GainTorch()
    {
        torchLife += torchGain;
        torchLife = Mathf.Clamp(torchLife, 0, maxTorchLife);
    }


}
