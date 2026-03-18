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


        private IPlayerService m_PlayerService;
        private IUIService m_UIService;
        private ILevelService m_LevelService;
        private IGameLoopService m_GameLoop;

        [Inject]
        private void Construct(IPlayerService playerService, IUIService uiService, ILevelService levelService, IGameLoopService gameLoop)
        {
            m_PlayerService = playerService;
            m_UIService = uiService;

            m_GameLoop = gameLoop;

            m_GameLoop.OnUpdateTick += OnGameLoopUpdate;
        }

        public void Reset()
        {
            base.Reset();
            ID = EWindowID.Gameplay;
            Slider = GetComponentInChildren<Slider>();
        }

        private void OnDestroy()
        {
            m_GameLoop.OnUpdateTick -= OnGameLoopUpdate;
        }


        private void ResetSlider()
        {
            Slider.value = 0;
        }
        private void OnGameLoopUpdate()
        {
            float playerDistance = m_PlayerService.GetPlayerProgressedDistance();
            float totalDistance = m_PlayerService.GetTotalProgressedDistance();

            float playerProgress = (totalDistance - playerDistance) / totalDistance;


            if (playerProgress > 0)
            {
                if (playerDistance < 0.001f)
                {
                    Slider.value = 1;
                    m_GameLoop.OnUpdateTick -= OnGameLoopUpdate;

                }
                Slider.value = Mathf.Clamp(playerProgress, 0, 1);
            }
            else
            {
                ResetSlider();
            }
        }


    }
     
  

       
}
