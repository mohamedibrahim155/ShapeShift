using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/Configs/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public LevelView LevelViewPrefab;
    public float BlockZSpacing = 4;
    public int MaxBlockIterationPerSpawnPoint = 4;
}
