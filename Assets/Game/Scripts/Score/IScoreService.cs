using System;

namespace Scripts.Score
{
    public interface IScoreService
    {
        int CurrentScore { get; }
        event Action<int> OnScoreChanged;
        void AddPoints(int points);
        void Reset();
    }
}
