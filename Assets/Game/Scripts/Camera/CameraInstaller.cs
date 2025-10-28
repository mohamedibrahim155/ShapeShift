using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "CameraInstaller", menuName = "Installers/CameraInstaller")]
public class CameraInstaller : ScriptableObjectInstaller<CameraInstaller>
{
    public CameraConfig cameraConfig;
    public override void InstallBindings()
    {
        Container.BindInstance(cameraConfig);
        Container.Bind<ICameraService>().To<CameraService>().AsSingle().NonLazy();
    }
}