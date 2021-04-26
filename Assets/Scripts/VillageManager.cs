using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;

public class VillageManager : MonoBehaviour
{
    public ControlSet controls;
    public PlayerStats stats;
    public Tweener chestTweener;
    public BackPack lootChest;
    public SpriteAlphaSwitch fade;
    public SceneLoader loader;
    Tweener openedPerk;
    public List<PerkDesc> perks;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        controls.Enable();
        lootChest.SetSlotAmountTo(lootChest.goldSlot, stats.gold, false);
        lootChest.SetSlotAmountTo(lootChest.emeraldSlot, stats.emerald, false);
        lootChest.SetSlotAmountTo(lootChest.rubySlot, stats.ruby, false);
        lootChest.SetSlotAmountTo(lootChest.diamondSlot, stats.diamond, false);
        chestTweener.TweenScaleTo(Vector3.one, 1, Ease.SmoothStep);

        for(int i =0;i<stats.perkList.Count;i++)
        {
            if (stats.perkList[i].unlocked)
            {
                
                for (int j = 0; j < perks.Count; j++)
                {
                    if (perks[j].perkName == stats.perkList[i].perkName)
                    {
                       
                        perks[j].icon.Buy();
                        perks[j].bought = true;
                        break;
                    }
                }
            }            
        }

        for(int i = 0;i<perks.Count;i++)
        {
            perks[i].icon.Initialize();
        }
    }

    public void OpenPerk(Tweener perk)
    {
        if(openedPerk!=null)
        {
            openedPerk.TweenScaleTo(Vector3.zero, 0.1f, Ease.EaseIn);
        }
        openedPerk = perk;
        perk.TweenScaleTo(Vector3.one, 0.25f, Ease.EaseOut);
        PerkDesc pdesc = perk.GetComponent<PerkDesc>();
        pdesc.CheckIfCanBuy();
    }

    public void EnterMine()
    {
        StartCoroutine(EnteringMine());
    }

    IEnumerator EnteringMine()
    {
        if (openedPerk != null)
        {
            openedPerk.TweenScaleTo(Vector3.zero, 0.1f, Ease.EaseIn);
        }
        controls.Disable();
        fade.SwitchAlphaToAbsolute(1);
        yield return new WaitForSeconds(1);
        loader.LoadSceneAsync(0);
    }
}
