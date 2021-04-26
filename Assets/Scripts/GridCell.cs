using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Easings;

public class GridCell : MonoBehaviour
{
    public enum GroundType {Dirt, Rock, Obsidian, Unbreakable, Monster, Bomb, Boss, Empty, Combat_Block, Combat_Enemy}
    public enum RewardType {Null, Carrot, Gold, Emerald, Ruby, Diamond, Torch}

    public GroundType ground;
    public RewardType reward;
    public int health;
    public int rewardAmount;
    public GameObject visual;
    public Vector2Int coordinates;
    public SquareGrid grid;
    public GameEvent destroyedEvent;
    [HideInInspector]
    public Elevator elevatorRight, elevatorLeft;
    [HideInInspector]
    public Enemy enemy;

    [Header("health")]
    public Tweener healthTweener;
    public SpriteRenderer healthBar;
    float healthBarStartSize;
    int maxHealth;
    bool feedback;
    public Tweener mainTweener;
    public ParticleSystem ps;

    private void Start()
    {
        maxHealth = health;
        healthBarStartSize = healthBar.size.y;
    }

    void UpdateCellUI()
    {
        if (!feedback)
        {
            healthTweener.TweenScaleTo(Vector3.one, 0.1f, Easings.Ease.EaseOut);
        }

        float t = (float)health / (float)maxHealth;
        healthBar.size = new Vector2(healthBar.size.x, Mathf.Lerp(0, healthBarStartSize, t));
    }

    public void Mine(int amount, out bool destroyed)
    {
        destroyed = false;
        if (ground == GroundType.Empty)
        {
            //Debug.Log("tried to mine a destroyed cell");
            return;
        }
        ps.Play();
        health -= amount;
        if (health <= 0)
        {
            health = 0;
            destroyed = true;
            DestroyCell();
        }
        else
        {
            StartCoroutine(MiningFeedback());
            UpdateCellUI();
        }
    }

    public void DestroyCell()
    {
        ps.Play();
        mainTweener.StopAllCoroutines();
        healthTweener.gameObject.SetActive(false);
        visual.SetActive(false);
        ground = GroundType.Empty;
        destroyedEvent.Raise();
    }    

    IEnumerator MiningFeedback()
    {
        mainTweener.TweenScaleTo(Vector3.one * 0.9f, 0.05f, Ease.EaseOut);
        yield return new WaitForSeconds(0.05f);
        mainTweener.TweenScaleTo(Vector3.one, 0.05f, Ease.EaseIn);

    }
}

[System.Serializable]
public class CellStateTransition
{
    //public GridCell.CellState state;
    public UnityEvent transitionResponse;
}
