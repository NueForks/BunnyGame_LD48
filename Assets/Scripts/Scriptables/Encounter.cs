using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Encounter")]
public class Encounter : ScriptableObject
{
    public float weight; 
    public List<EnemySetup> enemies = new List<EnemySetup>();
    public int goldGain, emeraldGain, rubyGain;
}

[System.Serializable]
public class EnemySetup
{
    public float delay;
    public GameObject enemyPrefab;
    public Vector2Int startCell;
}
