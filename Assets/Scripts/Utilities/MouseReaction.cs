using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseReaction : MonoBehaviour
{
    public ControlSet controls;
    bool leftClicked;

    public int currentSet;

    public List<MouseReactionSet> reactionSets = new List<MouseReactionSet>();

    private void OnMouseEnter()
    {
        if (!controls.disabled)
        {
            reactionSets[currentSet].mouseEnterResponse.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (!controls.disabled)
        {
            reactionSets[currentSet].mouseExitResponse.Invoke();
        }
    }

    private void OnMouseOver()
    {
        if (!controls.disabled)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                reactionSets[currentSet].LeftClickResponse.Invoke();
                leftClicked = true;
            }

            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (leftClicked)
                {
                    reactionSets[currentSet].LeftClickReleaseResponse.Invoke();
                }
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                //reactionSets[currentSet].RightClickResponse.Invoke();
            }
        }
       
    }

    public void SetReactionSet(int i)
    {
        currentSet = i;
    }
}

[Serializable]
public class MouseReactionSet
{
    public string title;
    public UnityEvent mouseEnterResponse;
    public UnityEvent mouseExitResponse;
    public UnityEvent LeftClickResponse;
    public UnityEvent LeftClickReleaseResponse;
    //public UnityEvent RightClickResponse;
}


