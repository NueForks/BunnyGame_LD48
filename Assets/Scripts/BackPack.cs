using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BackPack : MonoBehaviour
{
    public float slotXSize = 1f;
    public float resizeTime = 0.25f;
    public int activeSlots = 0;

    public itemSlot goldSlot, emeraldSlot, rubySlot, diamondSlot;

    private void Awake()
    {
        goldSlot.amount = 0;
        goldSlot.tmp_amount.text = "0";
        emeraldSlot.amount = 0;
        emeraldSlot.tmp_amount.text = "0";
        rubySlot.amount = 0;
        rubySlot.tmp_amount.text = "0";
        diamondSlot.amount = 0;
        diamondSlot.tmp_amount.text = "0";
    }

    public void GainGold(int amount)
    {
        ChangeSlotAmount(goldSlot, amount);
    }
    public void GainEmerald(int amount)
    {
        ChangeSlotAmount(emeraldSlot, amount);
    }
    public void GainRuby(int amount)
    {
        ChangeSlotAmount(rubySlot, amount);
    }
    public void GainDiamond(int amount)
    {
        ChangeSlotAmount(diamondSlot, amount);
    }

    public void ChangeSlotAmount(itemSlot slot, int amount)
    {
        slot.amount += amount;
        slot.amount = Mathf.Clamp(slot.amount, 0, 100000);
        slot.tmp_amount.text = slot.amount.ToString();
        if (amount != 0)
        {
            slot.anim.Play("SlotShake");
        }
        if(slot.amount == 0 && slot.active)
        {
            //ActivateSlot(slot, false);
        }
        else if (!slot.active)
        {
            //ActivateSlot(slot, true);
        }
    }
    public void SetSlotAmountTo(itemSlot slot, int amount, bool shake)
    {
        if(shake)
        {
            slot.anim.Play("SlotShake");
        }
        slot.amount = amount;
        slot.amount = Mathf.Clamp(slot.amount, 0, 1000);
        slot.tmp_amount.text = slot.amount.ToString();        
    }

    void ActivateSlot(itemSlot slot, bool on)
    {
        if (on)
        {
            slot.anim.Play("SlotAppear");
        }
        else
        {
            slot.anim.Play("SlotDisappear");
        }
    }
}

[System.Serializable]
public class itemSlot
{
    public GameObject visual;
    public Animation anim;
    public int amount;
    public TextMeshPro tmp_amount;
    public Animation death;
    [HideInInspector]
    public bool active;
}
