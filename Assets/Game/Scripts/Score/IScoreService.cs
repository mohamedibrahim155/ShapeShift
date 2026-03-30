using System;

namespace Scripts.Score
{
    public interface IScoreService
    {
        ScoreConfig ScoreConfig { get; }
        ScoreView ScoreView { get; }
        int CurrentScore { get; }
        event Action<int> OnScoreChanged;
        event Action<bool> OnScoreDisplay;
        void AddPoints(int points);
        void Reset();
    }
}
