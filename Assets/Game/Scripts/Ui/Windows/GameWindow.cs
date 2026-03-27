using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
using Scripts.GameService;
using Scripts.Score;
namespace Scripts.UI
{
    public class GameWindow : UIWindow
    {
        [SerializeField] private Slider Slider;
        [SerializeField] private ScoreView ScoreView;
       

        public override void Reset()
        {
            base.Reset();
            ID = EWindowID.Gameplay;
            Slider = GetComponentInChildren<Slider>();
            ScoreView = GetComponentInChildren<ScoreView>(true);
        }

        public void ResetProgress()
        {
            SetProgress(0);
        }


        public void SetProgress(float progress01)
        {
            Slider.value = Mathf.Clamp01(progress01);
        }
    }
       
}
