using Modules.MainModule.Scripts.InputServices;
using Modules.MainModule.Scripts.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Modules.MainModule.Scripts.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [Header("INPUT SERVICES")]
        [SerializeField] private MobileInputService mobileInputService;
        [SerializeField] private PcInputService pcInputService;
        
        [Header("SCENE MANAGEMENT")]
        [SerializeField] private SceneLoader sceneLoader;
        
        [Header("USER INTERFACE")]
        [SerializeField] private UIManager uiManager;
        
        public override void InstallBindings()
        {
            BindInputService();
            
            BindUIManager();

            BindSceneLoader();
        }
        
        private void BindInputService()
        {
            InputService inputServiceInstance;

#if UNITY_ANDROID

            inputServiceInstance = Container
            .InstantiatePrefabForComponent<MobileInputService>(mobileInputService);

#endif
            
#if UNITY_EDITOR
            
            inputServiceInstance = Container
                .InstantiatePrefabForComponent<PcInputService>(pcInputService);
#endif

            Container
                .Bind<InputService>()
                .FromInstance(inputServiceInstance)
                .AsSingle();
        }

        private void BindSceneLoader()
        {
            var sceneLoaderInstance = Container
                .InstantiatePrefabForComponent<SceneLoader>(sceneLoader);

            Container
                .Bind<SceneLoader>()
                .FromInstance(sceneLoaderInstance)
                .AsSingle();
        }
        
        private void BindUIManager()
        {
            var uiManagerInstance = Container
                .InstantiatePrefabForComponent<UIManager>(uiManager);

            Container
                .Bind<UIManager>()
                .FromInstance(uiManagerInstance)
                .AsSingle();
        }
    }
}