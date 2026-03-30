using Scripts.GameplayStates;
using Scripts.UI;
using System;
using Zenject;

namespace Scripts.Score
{
    public class ScoreService : IScoreService
    {
        public ScoreConfig ScoreConfig { get; private set; }
        public ScoreView ScoreView { get; private set; }
        public int CurrentScore { get; private set; }
        public event Action<int> OnScoreChanged;
        public event Action<bool> OnScoreDisplay;


        private GameWindow gameWidow;
        private DiContainer DiContainer;
        
        [Inject]
        public void Construct(ScoreConfig config, DiContainer container)
        {
            ScoreConfig = config;
            DiContainer =container;
        }

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
