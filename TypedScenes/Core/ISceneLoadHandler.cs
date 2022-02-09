using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace IJunior.TypedScenes
{
    public interface ISceneLoadHandler<T>
    {
        void OnSceneLoaded(T argument);
    }

    public interface IUnloadingLoadingScene
    {
        public void SetLoadingScreen(AsyncOperationHandle<SceneInstance> scene);
    }
}
