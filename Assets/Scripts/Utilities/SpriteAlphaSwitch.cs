using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Easings;
using Sirenix.OdinInspector;

public class SpriteAlphaSwitch : MonoBehaviour
{
    public List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    public List<float> defaultAlphas = new List<float>();

    public float transitionTime;

    [Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void InitializeWithChildren()
    {
        sprites.Clear();
        SpriteRenderer[] spritesChildren = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in spritesChildren)
        {
            sprites.Add(sr);
        }
        GetDefaultValues();
    }

    [Button(ButtonSizes.Medium), GUIColor(0, 0.9f, 0.9f)]
    void GetDefaultValues()
    {
        defaultAlphas.Clear();
        for (int i = 0; i < sprites.Count; i++)
        {
            defaultAlphas.Add(sprites[i].color.a);
        }
    }



    public void SwitchAlphaToRelative(float targetAlpha)
    {
        StartCoroutine(SwitchAlphaRelative(targetAlpha));
    }

    public void SwitchAlphaToRelativeImmediate(float targetAlpha)
    {
       // Debug.Log(sprites.Count + " // name :  " + this.name);
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, defaultAlphas[i] * targetAlpha);
        }
    }

    public void SwitchAlphaToAbsolute(float targetAlpha)
    {
        StartCoroutine(SwitchAlphaAbsolute(targetAlpha));
    }

    public void SwitchAlphaToZeroImmediate()
    {
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, 0);
        }
    }

    public void SwitchAlphaToZeroDelayed(float delay)
    {
        StartCoroutine(SwitchAlphaAbsoluteDelay(0, delay));
    }

    IEnumerator SwitchAlphaRelative(float targetAlpha)
    {
        List<float> targetAlphas = new List<float>();
        for (int i = 0; i < defaultAlphas.Count; i++)
        {
            targetAlphas.Add(defaultAlphas[i] * targetAlpha);
        }

        List<float> currentAlphas = new List<float>();
        for (int i = 0; i < sprites.Count; i++)
        {
            currentAlphas.Add(sprites[i].color.a);
        }

        float t = 0;
        float currentLerpTime = 0;
        while (currentLerpTime < transitionTime)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / transitionTime;
            t = Ease.EaseOut(t);

            for (int i = 0; i < sprites.Count; i++)
            {
                float newAlpha = Mathf.Lerp(currentAlphas[i], targetAlphas[i], t);
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, newAlpha);
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, targetAlphas[i]);
            }
        }
    }

    IEnumerator SwitchAlphaAbsolute(float targetAlpha)
    {
        List<float> targetAlphas = new List<float>();
        for (int i = 0; i < sprites.Count; i++)
        {
            targetAlphas.Add(targetAlpha);
        }

        List<float> currentAlphas = new List<float>();
        for (int i = 0; i < sprites.Count; i++)
        {
            currentAlphas.Add(sprites[i].color.a);
        }

        float t = 0;
        float currentLerpTime = 0;
        while (currentLerpTime <= transitionTime)
        {
            currentLerpTime += Time.deltaTime;
            t = currentLerpTime / transitionTime;
            t = Ease.EaseOut(t);

            for (int i = 0; i < sprites.Count; i++)
            {
                float newAlpha = Mathf.Lerp(currentAlphas[i], targetAlphas[i], t);
                sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, newAlpha);
            }
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i < sprites.Count; i++)
        {
            sprites[i].color = new Color(sprites[i].color.r, sprites[i].color.g, sprites[i].color.b, targetAlphas[i]);
        }
    }

    IEnumerator SwitchAlphaAbsoluteDelay(float targetAlpha, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(SwitchAlphaAbsolute(targetAlpha));
    }
}
