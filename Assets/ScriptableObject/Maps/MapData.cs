using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapData", menuName = "Map/Data")]
public class MapData : ScriptableObject
{
    public string MapName ; 
    public GameObject Map ; 
    public Enemy[] Enemies ; 
    public Enemy BossPrefab ; 

    
}