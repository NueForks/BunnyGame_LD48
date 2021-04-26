using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Combat : MonoBehaviour
{
    public PlayerCombat pcombat; 
    public List<Enemy> enemies = new List<Enemy>();
    public Tweener combatTweener;
    public Animation victoryAnim;
    public Animation startCombatAnim;
    public Animation gameOverAnim;
    public SquareGrid grid;
    public SpriteAlphaSwitch fade;
    public Tweener lootTweener;
    public TextMeshPro tmp_loot_gold, tmp_loot_emerald, tmp_loot_ruby;
    public BackPack backpack;
    int goldLoot, emeraldLoot, rubyLoot;

    public List<EncounterCollection> encounterCollections = new List<EncounterCollection>();

    public void FindEncounter(int depth)
    {
        int depthIndex = 0;
        for (int i = encounterCollections.Count - 1; i >= 0; i--)
        {
            if (depth >= encounterCollections[i].startDepth)
            {
                depthIndex = i;
                break;
            }
        }

        Encounter e = encounterCollections[depthIndex].GetRandomEncounter();
        for(int i = 0;i<e.enemies.Count;i++)
        {            
            GameObject instanceGO = Instantiate(e.enemies[i].enemyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            instanceGO.transform.parent = grid.cells[e.enemies[i].startCell.x, e.enemies[i].startCell.y].transform;            
            Enemy instance = instanceGO.GetComponent<Enemy>();
            instance.transform.localPosition = Vector3.zero+new Vector3(instance.cellOffset.x, instance.cellOffset.y, 0);
            instance.transform.localScale = Vector3.one;
            instance.InitializeEnemy(grid.cells[e.enemies[i].startCell.x, e.enemies[i].startCell.y], this, e.enemies[i].delay);
            enemies.Add(instance);
        }

        pcombat.transform.parent = grid.cells[3, 0].transform;
        pcombat.transform.localPosition = Vector3.zero + pcombat.offset;
        pcombat.transform.localScale = Vector3.one;
        pcombat.coordinates = new Vector2Int(3, 0);
        pcombat.mute = true;

        goldLoot = e.goldGain;
        emeraldLoot = e.emeraldGain;
        rubyLoot = e.rubyGain;
        
        StartCoroutine(StartingCombat());
    }

    public void KillEnemy(Enemy e)
    {
        enemies.Remove(e);
        if (enemies.Count == 0)
        {
            WinCombat();
        }
    }

    public void WinCombat()
    {
        StartCoroutine(WinningRoutine());
    }

    public void LoseCombat()
    {

    }

    IEnumerator WinningRoutine()
    {
        pcombat.mute = true;
        yield return new WaitForSeconds(1);
        victoryAnim.Play("VictoryDisplay");
        yield return new WaitForSeconds(2);
        victoryAnim.Play("VictoryDisappear");
        yield return new WaitForSeconds(0.25f);
        tmp_loot_gold.text = goldLoot.ToString();
        tmp_loot_emerald.text = emeraldLoot.ToString();
        tmp_loot_ruby.text = rubyLoot.ToString();
        lootTweener.TweenScaleTo(Vector3.one, 0.5f, Easings.Ease.EaseOut);
        yield return new WaitForSeconds(1f);
        backpack.GainGold(goldLoot);
        backpack.GainEmerald(emeraldLoot);
        backpack.GainRuby(rubyLoot);
        yield return new WaitForSeconds(1f);
        lootTweener.TweenScaleTo(Vector3.zero, 0.35f, Easings.Ease.EaseIn);
        combatTweener.TweenScaleTo(Vector3.zero, 0.5f, Easings.Ease.EaseIn);
        yield return new WaitForSeconds(0.25f);
        fade.SwitchAlphaToAbsolute(0f);
        yield return new WaitForSeconds(0.25f);
        pcombat.p.currentCombatCell.DestroyCell();
        pcombat.p.mute = false;
        
    }

    IEnumerator StartingCombat()
    {
        fade.SwitchAlphaToAbsolute(0.2f);
        yield return new WaitForSeconds(1);
        combatTweener.TweenScaleTo(Vector3.one, 0.5f, Easings.Ease.EaseOut);
        yield return new WaitForSeconds(0.5f);
        startCombatAnim.Play("StartCombatDisplay");
        yield return new WaitForSeconds(1.2f);
        startCombatAnim.Play("StartCombatDisappear");
        yield return new WaitForSeconds(1f);
        pcombat.mute = false;
        foreach(Enemy e in enemies)
        {
            e.transform.parent = this.transform;
            e.StartCoroutine(e.ActivationDelay());
        }
    }
}


[System.Serializable]
public class EncounterCollection
{
    public int startDepth;
    public List<Encounter> encounters;
    public Encounter GetRandomEncounter()
    {
        float totalWeight = 0;
        foreach (Encounter e in encounters)
        {
            totalWeight += e.weight;
        }
        float p = Random.Range(0f, totalWeight);
        float checkWeight = 0;
        for (int i = 0; i < encounters.Count; i++)
        {
            checkWeight += encounters[i].weight;
            if (p <= checkWeight)
            {
                return encounters[i];
            }
        }
        return encounters[0];
    }
}
