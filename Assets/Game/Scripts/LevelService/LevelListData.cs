using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Level
{
    [CreateAssetMenu(fileName = "LevelListData", menuName = "Scriptable Objects/Configs/LevelListData")]
    public class LevelListData : ScriptableObject
    {
        public List<LevelConfig> m_Levels;
        public int m_CurrentLevelIndex;

        public void Reset()
        {
            m_CurrentLevelIndex = 0;
        }
    }
}
