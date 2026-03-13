using UnityEngine;

namespace Scripts.UI
{
    public interface IUIService
    {

        public abstract UIWindow GetWindow(EWindowID ID);
        public abstract void OpenWindow(EWindowID ID, float time = 0.5f);
        public abstract void CloseWindow(EWindowID ID, float time = 0.5f);

        void Cleanup();
    }
}
