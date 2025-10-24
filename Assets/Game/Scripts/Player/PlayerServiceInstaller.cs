using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "PlayerService", menuName = "Installers/PlayerService")]
public class PlayerServiceInstaller : ScriptableObjectInstaller<PlayerServiceInstaller>
{
    public override void InstallBindings()
    {
    }
}