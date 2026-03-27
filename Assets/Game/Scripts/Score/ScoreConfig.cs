using UnityEngine;
namespace Scripts.Score
{
    [CreateAssetMenu(fileName ="Score Config", menuName = "Scriptable Objects/Configs/ScoreConfig")]
    public class ScoreConfig : ScriptableObject
    {
        public ScoreView scoreViewPrefab;
        public int PerCollisionScore = 1;
        public float m_ScoreTextOffsetY = 100;
        public float m_FadeDuration = 0.5f;
        public float m_MoveDuration = 0.5f;
    }
}
