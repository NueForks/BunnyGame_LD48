using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PerkDesc : MonoBehaviour
{
    public PlayerStats stats;
    public bool bought;
    public BackPack chest;
    public PerkIcon icon;

    [Header("Perk")]
    public string perkName;
    public int miningDamageBonus;
    public int battleDamageBonus;
    public int carotHealBonus;
    public int bombDamagesReduction;
    public int healthBonus;
    public float lootKeepPercentageBonus;
    public float torchGainBonus;
    public float torchLifeBonus;
    public float torchConsumeReductionBonus;

    [Header("Price")]
    public int gold, emerald, ruby, diamond;

    [Header("Desc")]
    public GameObject buyButton, price;
    public MouseReaction buyReaction;
    public SpriteColorSwitch buySPR;
    public TextMeshPro tmp_buy;
    public TextMeshPro tmp_gold, tmp_emerald, tmp_ruby, tmp_diamond;
    Color defaultBuyTextColor;

    private void Start()
    {
        icon.desc = this.GetComponent<Tweener>();
        tmp_gold.text = gold.ToString() ;
        tmp_emerald.text = emerald.ToString();
        tmp_ruby.text = ruby.ToString();
        tmp_diamond.text = diamond.ToString();
        defaultBuyTextColor = tmp_buy.color;
    }

    public void CheckIfCanBuy()
    {
        if (bought)
        {
            buyButton.SetActive(false);
            price.SetActive(false);
            return;
        }
        bool canBuy = true;
        if (stats.gold<gold)
        {
            canBuy = false;
        }
        if (stats.emerald < emerald)
        {
            canBuy = false;
        }
        if (stats.ruby < ruby)
        {
            canBuy = false;
        }
        if (stats.diamond < diamond)
        {
            canBuy = false;
        }
        if (!icon.available)
        {
            canBuy = false;
        }
        if (!canBuy)
        {
            buyReaction.SetReactionSet(1);
            buySPR.SwitchSpriteColor(1);
            tmp_buy.color = new Color(defaultBuyTextColor.r, defaultBuyTextColor.g, defaultBuyTextColor.b, 0.6f);
        }
        else
        {
            buyReaction.SetReactionSet(0);
            buySPR.ResetSpriteToDefaultColor();
            tmp_buy.color = new Color(defaultBuyTextColor.r, defaultBuyTextColor.g, defaultBuyTextColor.b, 1);
        }
    }

    public void BuyPerk()
    {
        if (bought)
        {
            return;
        }
        bought = true;

        stats.miningDamageBonus += miningDamageBonus;
        stats.battleDamageBonus += battleDamageBonus;
        stats.carotHealBonus += carotHealBonus;
        stats.bombDamagesReduction += bombDamagesReduction;
        stats.healthBonus += healthBonus;
        stats.lootKeepPercentageBonus += lootKeepPercentageBonus;
        stats.torchGainBonus += torchGainBonus;
        stats.torchLifeBonus += torchLifeBonus;
        stats.torchConsumeReductionBonus += torchConsumeReductionBonus;

        chest.GainGold(-gold);
        chest.GainEmerald(-emerald);
        chest.GainRuby(-ruby);
        chest.GainDiamond(-diamond);

        stats.gold -= gold;
        stats.emerald -= emerald;
        stats.ruby -= ruby;
        stats.diamond -= diamond;

        buyButton.SetActive(false);
        price.SetActive(false);

        for(int i = 0;i<stats.perkList.Count;i++)
        {
            if (perkName == stats.perkList[i].perkName)
            {
                stats.perkList[i].unlocked = true;
                break;
            }
        }
        icon.Buy();
    }
}
