using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="Custom/MineSegment")]
public class MineSegment : SerializedScriptableObject
{
    public float weight;
    public bool elevatorLeft, elevatorRight, mirror;
    public GameObject[,] structure = new GameObject[7,3];
}
