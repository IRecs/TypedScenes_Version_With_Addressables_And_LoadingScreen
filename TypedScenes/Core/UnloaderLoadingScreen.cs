using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace IJunior.TypedScenes
{
    public abstract class UnloaderLoadingScreen : MonoBehaviour, IUnloadingLoadingScene
    {
        private event Action UnloadedLoadingScreen;

        public void SetLoadingScreen(AsyncOperationHandle<SceneInstance> sceneInstance)
        {
            if (UnloadedLoadingScreen == null)
                UnloadedLoadingScreen = () => Addressables.UnloadSceneAsync(sceneInstance);
            else
                throw new NullReferenceException($"{nameof(SceneInstance)} is null.");
        }

        protected void UnloadLoadingScreen()
        {
            UnloadedLoadingScreen?.Invoke();
            UnloadedLoadingScreen = null;
        }
    }
}

