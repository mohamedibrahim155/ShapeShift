using Scripts.UI;
using UnityEngine;
using Zenject;

namespace Scripts.UI
{
    [CreateAssetMenu(fileName = "UIInstaller", menuName = "Installers/UIInstaller")]
    public class UIInstaller : ScriptableObjectInstaller<UIInstaller>
    {
        public UIConfig UIConfig;
        public override void InstallBindings()
        {
            Container.BindInstances(UIConfig);
            Container.Bind<IUIService>().To<UIService>().AsSingle().NonLazy();
            Container.Bind<PlayerProgressController>().AsSingle().NonLazy();
        }
    }
}