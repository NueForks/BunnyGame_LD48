using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public Transform animParent;
    public Animation anim;
    public GameEvent enterEvent;

    public void Enter()
    {
        anim.Play("ElevatorUp");
        enterEvent.Raise();
    }
}
