using UnityEngine;
using Zenject;

namespace Modules.GameModule.Scripts.Installers
{
    public class PlayerCameraInstaller : MonoInstaller
    {
        [SerializeField] private PlayerCamera playerCamera;
        public override void InstallBindings()
        {
            Container.Bind<PlayerCamera>().FromInstance(playerCamera);
        }
    }
}