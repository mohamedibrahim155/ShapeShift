using UnityEngine;

public interface ILevelService
{
    void CreateLevel( int levelNo );
    LevelView GetLevel( int levelNo );
}
