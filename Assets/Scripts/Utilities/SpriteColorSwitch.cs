using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof (SpriteRenderer))]
public class SpriteColorSwitch : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color defaultColor;
    public List<Color> colors;

    /*void Awake()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }*/
    /*void OnEnable()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }*/

    public void SwitchSpriteColor(int i)
    {
        spriteRenderer.color = colors[i];
    }

    public void ResetSpriteToDefaultColor()
    {
        if (spriteRenderer == null)
        {
            Debug.Log(this.name + " " + GetInstanceID().ToString());
        }
        spriteRenderer.color = defaultColor;
    }

    [Button(ButtonSizes.Medium)]
    void GetDefaultValues()
    {
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        defaultColor = spriteRenderer.color;
    }
}
