using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Score
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private Text m_ScoreText;

        [Inject] private IScoreService m_ScoreService;

        private void OnEnable()
        {
            if (m_ScoreText == null)
            {
                Debug.LogWarning("ScoreView missing Text reference.");
                return;
            }

            m_ScoreService.OnScoreChanged += UpdateScore;
            UpdateScore(m_ScoreService.CurrentScore);
        }

        private void OnDisable()
        {
            if (m_ScoreText == null || m_ScoreService == null)
            {
                return;
            }

            m_ScoreService.OnScoreChanged -= UpdateScore;
        }

        private void UpdateScore(int score)
        {
            m_ScoreText.text = $"Score: {score}";
        }
    }
}
