using UnityEngine;

namespace Scripts.UI
{
    public interface IUiService
    {

        public abstract UIWindow GetWindow(EWindowID ID);
        public abstract void OpenWindow(EWindowID ID);
        public abstract void CloseWindow(EWindowID ID);

        void Cleanup();
    }
}
