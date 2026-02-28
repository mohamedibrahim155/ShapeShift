using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/Configs/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public List<LevelView> LevelPrefabs;
    public float BlockZSpacing = 4;
    public int MaxBlockIterationPerSpawnPoint = 4;
}
