using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName =("Custom/PlayerStats"))]
public class PlayerStats : ScriptableObject
{
    public int gold, emerald, ruby, diamond;
    public int miningDamageBonus;
    public int battleDamageBonus;
    public int carotHealBonus;
    public int bombDamagesReduction;
    public int healthBonus;
    public float lootKeepPercentageBonus;
    public float torchGainBonus;
    public float torchLifeBonus;
    public float torchConsumeReductionBonus;

    public List<Perk> perkList;

    [Button]
    public void ResetPerks()
    {
        foreach(Perk p in perkList)
        {
            p.unlocked = false;           
        }
        miningDamageBonus = 0;
        battleDamageBonus = 0;
        carotHealBonus = 0;
        bombDamagesReduction = 0;
        healthBonus = 0;
        lootKeepPercentageBonus = 0;
    }
}
[System.Serializable]
public class Perk
{
    public string perkName;
    public bool unlocked;
}
