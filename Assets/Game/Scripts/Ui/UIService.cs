using Scripts.GameService;
using Scripts.UI;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    public class UIService : IUiService
    {

        private Dictionary<EWindowID, UIWindow> m_ListOfWindowsCached   = new Dictionary<EWindowID, UIWindow>();

        private UICanvasView m_UiCanvasView;
        private UIConfig m_UiConfig;
        private DiContainer m_Container;

        [Inject] 
        public void Construct(UIConfig config, DiContainer container)
        {
            m_UiConfig = config;
            m_Container = container;

            SpawnMainCanvas();
            CachedWindows();
        }
        public void AddWindow(EWindowID ID, UIWindow window)
        {
            m_ListOfWindowsCached.Add(ID, window);
        }

        private void SpawnMainCanvas()
        {
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

        public void Cleanup()
        {
          
        }

        public void OpenWindow(EWindowID ID)
        {
            m_ListOfWindowsCached[ID].Open();
        }

        public void CloseWindow(EWindowID ID)
        {
            m_ListOfWindowsCached[ID].Close();
        }

        public UIWindow GetWindow(EWindowID ID)
        {
           return m_ListOfWindowsCached[ID];
        }
    }
}
