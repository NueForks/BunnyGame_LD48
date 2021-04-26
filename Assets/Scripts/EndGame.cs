using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D;
using Easings;

public class EndGame : MonoBehaviour
{
    public PlayerStats playerStats;
    public float deathLootMultiplier;
    public ProCamera2DShake camShaker;
    public BackPack backpack;
    public Tweener backPackTweener;
    public BackPack lootChest;
    public Tweener lootChestTweener;
    public SpriteAlphaSwitch fade;
    public Animation gameOverAnim;
    public Tweener combatTweener;
    public Tweener healthTweener;
    public SceneLoader sceneLoader;

    public void EndGameElevator()
    {
        StartCoroutine(EndGameRoutine(true, false));
    }

    public void GameOver(bool combat)
    {
        StartCoroutine(EndGameRoutine(false, combat));
    }

    IEnumerator EndGameRoutine(bool elevator, bool combat)
    {
        lootChest.SetSlotAmountTo(lootChest.goldSlot, playerStats.gold, false);
        lootChest.SetSlotAmountTo(lootChest.emeraldSlot, playerStats.emerald, false);
        lootChest.SetSlotAmountTo(lootChest.rubySlot, playerStats.ruby, false);
        lootChest.SetSlotAmountTo(lootChest.diamondSlot, playerStats.diamond, false);
        healthTweener.TweenScaleTo(Vector3.zero, 0.5f, Ease.EaseOut);
        yield return new WaitForSeconds(2f);
        //gameOverTweener.TweenScaleTo(Vector3.one * 1.2f, 0.5f, Ease.EaseOut);
        if (!elevator)
        {
            gameOverAnim.Play("VictoryDisplay");
        }
        fade.SwitchAlphaToAbsolute(.75f);
        
        yield return new WaitForSeconds(1.5f);
        if (combat)
        {
            combatTweener.TweenScaleTo(Vector3.one * 0, 0.5f, Ease.EaseIn);
            yield return new WaitForSeconds(0.5f);
        }
        //gameOverTweener.TweenScaleTo(Vector3.zero, 0.5f, Ease.EaseIn);
        if (!elevator)
        {
            gameOverAnim.Play("VictoryDisappear");
        }
        yield return new WaitForSeconds(0.4f);
        backPackTweener.TweenPositionTo(backpack.transform.localPosition + new Vector3(0, -3.5f, 0), 2, Ease.SmootherStep, true);
        yield return new WaitForSeconds(1f);
         
        if (!elevator)
        {
            float lootFactor = Mathf.Clamp01(deathLootMultiplier + playerStats.lootKeepPercentageBonus);
            if (backpack.goldSlot.amount>0)
            {
                backpack.SetSlotAmountTo(backpack.goldSlot, Mathf.FloorToInt(backpack.goldSlot.amount * lootFactor), true);
                backpack.goldSlot.death.Play("LootDeathPop");
                camShaker.Shake(0);
                yield return new WaitForSeconds(0.75f);
            }
            if (backpack.emeraldSlot.amount > 0)
            {
                backpack.SetSlotAmountTo(backpack.emeraldSlot, Mathf.FloorToInt(backpack.emeraldSlot.amount * lootFactor), true);
                backpack.emeraldSlot.death.Play("LootDeathPop");
                camShaker.Shake(0);
                yield return new WaitForSeconds(0.75f);
            }
            if (backpack.rubySlot.amount > 0)
            {
                backpack.SetSlotAmountTo(backpack.rubySlot, Mathf.FloorToInt(backpack.rubySlot.amount * lootFactor), true);
                backpack.rubySlot.death.Play("LootDeathPop");
                camShaker.Shake(0);
                yield return new WaitForSeconds(0.75f);
            }
            if (backpack.diamondSlot.amount > 0)
            {
                backpack.SetSlotAmountTo(backpack.diamondSlot, Mathf.FloorToInt(backpack.diamondSlot.amount * lootFactor), true);
                backpack.diamondSlot.death.Play("LootDeathPop");
                camShaker.Shake(0);
                yield return new WaitForSeconds(0.75f);
            }
        }

        lootChestTweener.TweenPositionTo(lootChestTweener.transform.localPosition + new Vector3(0, 5.5f, 0), 1, Ease.EaseOut, true);
        yield return new WaitForSeconds(2);

        if (backpack.goldSlot.amount > 0)
        {
            playerStats.gold += backpack.goldSlot.amount;
            backpack.SetSlotAmountTo(backpack.goldSlot, 0, false);
            lootChest.SetSlotAmountTo(lootChest.goldSlot, playerStats.gold, true);
            yield return new WaitForSeconds(0.25f);
        }
        if (backpack.emeraldSlot.amount > 0)
        {
            playerStats.emerald += backpack.emeraldSlot.amount;
            backpack.SetSlotAmountTo(backpack.emeraldSlot, 0, false);
            lootChest.SetSlotAmountTo(lootChest.emeraldSlot, playerStats.emerald, true);
            yield return new WaitForSeconds(0.25f);
        }
        if (backpack.rubySlot.amount > 0)
        {
            playerStats.ruby += backpack.rubySlot.amount;
            backpack.SetSlotAmountTo(backpack.rubySlot, 0, false);
            lootChest.SetSlotAmountTo(lootChest.rubySlot, playerStats.ruby, true);
            yield return new WaitForSeconds(0.25f);
        }
        if (backpack.diamondSlot.amount > 0)
        {
            playerStats.diamond += backpack.diamondSlot.amount;
            backpack.SetSlotAmountTo(backpack.diamondSlot, 0, false);
            lootChest.SetSlotAmountTo(lootChest.diamondSlot, playerStats.diamond, true);
            yield return new WaitForSeconds(0.25f);
        }

        yield return new WaitForSeconds(0.5f);
        backPackTweener.TweenScaleTo(Vector3.zero, 0.5f, Ease.EaseOut);
        lootChestTweener.TweenScaleTo(Vector3.zero, 0.5f, Ease.EaseOut);
        fade.SwitchAlphaToAbsolute(1);
        yield return new WaitForSeconds(0.5f);
        sceneLoader.LoadScene(0);
    }
}
