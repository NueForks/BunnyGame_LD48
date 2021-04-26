using System.Collections;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    public delegate float EaseDelegate(float f);
    public EaseDelegate easeDelegate;

    IEnumerator positionRoutine, rotationRoutine, scaleRoutine;
    
    
    public void TweenPositionTo(Vector3 targetPos, float duration, EaseDelegate ease, bool local)
    {
        if (positionRoutine != null)
        {
            StopCoroutine(positionRoutine);
        }
        positionRoutine = TweeningPositionTo(targetPos, duration, ease, local);
        StartCoroutine(positionRoutine);
    }

    public void TweenEulerTo(Vector3 targetEuler, float duration, EaseDelegate ease, bool local)
    {
        if (rotationRoutine != null)
        {
            StopCoroutine(rotationRoutine);
        }
        rotationRoutine = TweeningEulerTo(targetEuler, duration, ease, local);
        StartCoroutine(rotationRoutine);
    }

    public void TweenScaleTo(Vector3 targetScale, float duration, EaseDelegate ease)
    {
        if (scaleRoutine != null)
        {
            StopCoroutine(scaleRoutine);
        }
        scaleRoutine = TweeningScaleTo(targetScale, duration, ease);
        StartCoroutine(scaleRoutine);
    }

    IEnumerator TweeningPositionTo(Vector3 targetPos, float duration, EaseDelegate ease, bool local)
    {
        Vector3 startPos = local?transform.localPosition:transform.position;
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0, duration); 
            float l = ease(counter / duration); 
            if (local)
            {
                transform.localPosition = Vector3.Lerp(startPos, targetPos, l);
            }
            else
            {
                transform.position = Vector3.Lerp(startPos, targetPos, l);
            }
            yield return null;
        }
    }

    IEnumerator TweeningEulerTo(Vector3 targetEuler, float duration, EaseDelegate ease, bool local)
    {
        Vector2 startEuler = local ? transform.localEulerAngles : transform.eulerAngles;
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0, duration);
            float l = ease(counter / duration);
            if (local)
            {
                transform.localEulerAngles = Vector3.Lerp(startEuler, targetEuler, l);
            }
            else
            {
                transform.eulerAngles = Vector3.Lerp(startEuler, targetEuler, l);
            }

            yield return null;
        }
    }

    IEnumerator TweeningScaleTo(Vector3 targetScale, float duration, EaseDelegate ease)
    {
        Vector2 startScale = transform.localScale;
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            counter = Mathf.Clamp(counter, 0, duration);
            float l = ease(counter / duration); 
            transform.localScale = Vector3.Lerp(startScale, targetScale, l);

            yield return null;
        }
    }

    
}
