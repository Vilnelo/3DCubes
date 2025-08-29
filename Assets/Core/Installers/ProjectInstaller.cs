using Common.AssetManagement.External;
using Common.ConfigSystem.External;
using Common.ConfigSystem.Runtime;
using Common.InputSystem.External;
using Common.LocalizationSystem.External;
using Common.LocalizationSystem.Runtime;
using Core.Scene.External;
using Core.Scene.Runtime;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            LogDebug("Installing global services...");

            InstallSceneController();
            InstallAssetLoader();
            InstallLocalization();
            InstallConfigSystem();
            InstallInputSystem();

            LogDebug("Global services installed successfully");
        }

        private void InstallLocalization()
        {
            Container.Bind<ILocalizationService>()
                .To<LocalizationService>()
                .AsSingle()
                .NonLazy();

            Container.Bind<LocalizationDataLoader>()
                .AsSingle();

            Container.BindInterfacesTo<LocalizationController>()
                .AsSingle()
                .NonLazy();

            LogDebug("LocalizationController bound successfully");
        }

        private void InstallSceneController()
        {
            Container.Bind<ISceneController>()
                .To<SceneController>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            LogDebug("SceneController bound successfully");
        }

        private void InstallAssetLoader()
        {
            Container.Bind<IAssetLoader>()
                .To<AssetLoader>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            LogDebug("AssetLoader bound successfully");
        }
        
        private void InstallInputSystem()
        {
            Container.BindInterfacesAndSelfTo<InputService>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            LogDebug("InputSystem bound successfully");
        }

        private void InstallConfigSystem()
        {
            Container.Bind<IConfigReader>()
                .To<ConfigReader>()
                .AsSingle();

            Container.Bind<IConfigLoader>()
                .To<ConfigLoader>()
                .AsSingle();

            LogDebug("ConfigSystem bound successfully");
        }

        private void LogDebug(string message)
        {
            Debug.Log($"[ProjectInstaller] {message}");
        }
    }
}