using Scripts.Ads;
using Scripts.Audio;
using Scripts.GameService;
using Scripts.Haptics;
using Scripts.Level;
using Scripts.Player;
using Scripts.UI;
using Scripts.UI.Coins;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;
using Zenject;

namespace Scripts.UI
{
    public class UIService : IUIService
    {

        private Dictionary<EWindowID, UIWindow> m_ListOfWindowsCached = new Dictionary<EWindowID, UIWindow>();

        private UICanvasView m_UiCanvasView;
        private UIConfig m_UiConfig;
        private DiContainer m_Container;

        //Services
        private IPlayerService m_PlayerService;
        private ILevelService m_LevelService;
        private IAdService m_AdService;
        private ICoinService m_Coinservice;
        private IHapticService m_HapticService;
        private IAudioService m_AudioService;

        // Controllers
        private IController m_MainMenuController;
        private IController m_LevelCompleteController;
        private IController m_LevelFailedController;
        private IController m_SettingsController;

        private List<IController> m_ListOfControllers = new List<IController>();

        [Inject]
        public void Construct(UIConfig config, DiContainer container,
            IPlayerService playerService,
            ILevelService levelService,
            IAdService AdService,
            ICoinService coinService,
            IHapticService hapticService,
            IAudioService audioService)
        {
            m_UiConfig = config;
            m_Container = container;
            m_PlayerService = playerService;
            m_LevelService = levelService;
            m_AdService = AdService;
            m_Coinservice = coinService;
            m_HapticService = hapticService;
            m_AudioService = audioService;

            SpawnMainCanvas();
            CachedWindows();
            InitControllers();
        }
        public void AddWindow(EWindowID ID, UIWindow window)
        {
            m_ListOfWindowsCached.Add(ID, window);
        }

        private void SpawnMainCanvas()
        {
            Debug.Log("Spawning main canvas");
            m_UiCanvasView = m_Container.InstantiatePrefabForComponent<UICanvasView>(m_UiConfig.m_CanvasView);
        }


        private void CachedWindows()
        {
            Debug.Log("Caching Windows");
            foreach (var window in m_UiCanvasView.m_ListofWindows)
            {
                m_ListOfWindowsCached.Add(window.ID, window);

                if (GetWindow(window.ID).m_OpenOnStart)
                {
                    GetWindow(window.ID).Open(0);
                }
                else
                {
                    GetWindow(window.ID).Close(0);
                }
            }
        }

        private void InitControllers()
        {
            AddController(new MainMenuController(this, m_PlayerService));
            AddController(new LevelCompleteController(this, m_PlayerService, m_LevelService, m_Coinservice));
            AddController(new LevelFailedController(this, m_PlayerService, m_LevelService, m_AdService));
            AddController(new SettingsController(this, m_HapticService, m_AudioService));

        }

        private void AddController(IController controller)
        {
            m_ListOfControllers.Add(controller);
            controller.Initialize();
        }

        public void Cleanup()
        {
            foreach (var controller in m_ListOfControllers)
            {
                controller.Cleanup();
            }
        }

        public void OpenWindow(EWindowID ID, float time = 0.5f)
        {
            m_ListOfWindowsCached[ID].Open(time);
        }

        public void CloseWindow(EWindowID ID, float time = 0.5f)
        {
            m_ListOfWindowsCached[ID].Close(time);
        }

        public UIWindow GetWindow(EWindowID ID)
        {
            return m_ListOfWindowsCached[ID];
        }



        
    }

}
