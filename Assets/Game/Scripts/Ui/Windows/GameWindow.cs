using Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using TMPro;
using Scripts.Level;
using Scripts.GameService;
namespace Scripts.UI
{
    public class GameWindow : UIWindow
    {
        [SerializeField] private Slider Slider;

        public void Reset()
        {
            base.Reset();
            ID = EWindowID.Gameplay;
            Slider = GetComponentInChildren<Slider>();
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
