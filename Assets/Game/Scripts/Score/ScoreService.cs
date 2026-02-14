using System;

namespace Scripts.Score
{
    public class ScoreService : IScoreService
    {
        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;

        public void AddPoints(int points)
        {
            if (points <= 0)
            {
                return;
            }

            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void Reset()
        {
            CurrentScore = 0;
            OnScoreChanged?.Invoke(CurrentScore);
        }
    }
}
