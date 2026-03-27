using DG.Tweening;
using Scripts.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Scripts.Score
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_ScoreText;

         private float HalfScreenHeight;

         private IScoreService m_ScoreService;
         private ScoreConfig m_ScoreConfig;

        [Inject]
        private void Construct(IScoreService scoreService, ScoreConfig config)
        {
            m_ScoreService = scoreService;
            m_ScoreConfig = config;
        }
       
        private void Start()
        {
            HalfScreenHeight = Screen.height / 2;
        }

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

        public void Show()
        {
            m_ScoreText.gameObject.SetActive(true);
            m_ScoreText.DOFade(1, m_ScoreConfig.m_FadeDuration);

            m_ScoreText.rectTransform
                .DOMoveY(HalfScreenHeight + m_ScoreConfig.m_ScoreTextOffsetY, m_ScoreConfig.m_MoveDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(
                () => {
                    Hide();
                });
        }

        public void Hide()
        {
            m_ScoreText.DOFade(0, m_ScoreConfig.m_FadeDuration).
                OnComplete(
                ()=>
                {
                    //resets
                    m_ScoreText.rectTransform.DOMoveY(HalfScreenHeight, 0);
                    m_ScoreText.gameObject.SetActive(false);
                });
        }

        private void UpdateScore(int score)
        {
            m_ScoreText.text = $"Perfect x{score}";
            Show();
        }

        private void Reset()
        {
            m_ScoreText = GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }
}
