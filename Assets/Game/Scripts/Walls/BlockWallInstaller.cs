using UnityEngine;
using Zenject;
using static UnityEngine.Rendering.STP;

[CreateAssetMenu(fileName = "BlockWallInstaller", menuName = "Installers/BlockWallInstaller")]
public class BlockWallInstaller : ScriptableObjectInstaller<BlockWallInstaller>
{
     public BlockConfig BlockConfig;
    public override void InstallBindings()
    {
        Container.BindInstance(BlockConfig);
    }
}