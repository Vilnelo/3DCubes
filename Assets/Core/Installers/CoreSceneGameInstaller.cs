using Common.Canvases.External;
using Core.GridSystem.External;
using Core.GridSystem.Runtime;
using UnityEngine;
using Zenject;

namespace Core.Installers
{
    public class CoreSceneGameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("CoreSceneGameInstaller: Installing core scene services...");

            Container.BindInterfacesTo<MainCanvas>()
                .FromComponentInHierarchy()
                .AsSingle();
            
            Container.BindInterfacesTo<GridConfigSystem>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();

            Debug.Log("CoreSceneGameInstaller: Core scene services installed successfully");
        }
    }
}