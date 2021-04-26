using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;

public class PerkIcon : MonoBehaviour
{
    public VillageManager vmanager;
    [HideInInspector]
    public Tweener desc;
    public Tweener tweener;
    public bool available, bought;
    public SpriteColorSwitch contour;
    public SpriteRenderer sr;
    public Sprite lockedSprite, availableSprite;
    public List<PerkIcon> availablePerks;
    public List<SpriteColorSwitch> lanes;

    public void Initialize()
    {
        if (available )
        {
           Unlock();
        }
        else if (bought)
        {
            Buy();
        }
        else
        {
            Lock();
        }
    }

    public void HoverIn()
    {
        tweener.TweenScaleTo(Vector3.one*1.2f, 0.1f, Ease.EaseOut);

    }

    public void HoverOut()
    {
        tweener.TweenScaleTo(Vector3.one, 0.1f, Ease.EaseOut);
    }

    public void OpenDesc()
    {
        if (desc != null)
        {
            vmanager.OpenPerk(desc);
        }
    }

    public void Lock()
    {
        available = false;
        contour.ResetSpriteToDefaultColor();
        if (lockedSprite != null)
        {
            sr.sprite = lockedSprite;
        }
    }

    public void Unlock()
    {        
        available = true;
        contour.SwitchSpriteColor(0);
        if (availableSprite!=null)
        {
            sr.sprite = availableSprite;
        }
    }

    public void Buy()
    {
        if(bought)
        {
            return;
        }
        bought = true;
        contour.SwitchSpriteColor(1);
        for(int i =0;i<availablePerks.Count;i++)
        {
            availablePerks[i].Unlock();
        }
        for (int i = 0; i < lanes.Count; i++)
        {
            lanes[i].SwitchSpriteColor(0);
        }
    }
}
