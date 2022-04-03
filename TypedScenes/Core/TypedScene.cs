using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public abstract class TypedScene
    {
        protected static void LoadScene(string sceneName, LoadSceneMode loadSceneMode) =>
            LoadScene(new PseudoElement(), sceneName, loadSceneMode);

        protected static void LoadScene<T>(T argument, string sceneName, LoadSceneMode loadSceneMode)
        {
            ScreenCase.LoadingWithResourceCleanup(argument, sceneName, loadSceneMode);
        }

        protected static void LoadSceneWithLoadingScreen(string sceneName, string loadingScreenName) =>
            LoadSceneWithLoadingScreen(new PseudoElement(), sceneName, loadingScreenName);

        protected static void LoadSceneWithLoadingScreen<T>(T argument, string sceneName, string loadingScreenName) 
        {
           ScreenCase.LoadingWithResourceCleanupUsingLoadingScene(argument, sceneName, loadingScreenName);
        }
        
        private struct ScreenCase : IDisposable
        {
            public AsyncOperationHandle<SceneInstance> Handle { get; }

            public Scene Scene => Handle.Result.Scene;

            public ScreenCase(AsyncOperationHandle<SceneInstance> handle)
            {
                if (!handle.IsDone)
                    throw new ArgumentException($"{nameof(AsyncOperationHandle<SceneInstance>)} not loaded");
                Handle = handle;
            }

            private static async Task<ScreenCase> GetCase(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool activateOnLoad = true)
            {
                AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(sceneName, loadSceneMode, activateOnLoad);
                await handle.Task;
                return new ScreenCase(handle);
            }

            public static async void LoadingWithResourceCleanup<T>(T argument, string sceneName,  LoadSceneMode loadSceneMode = LoadSceneMode.Single)
            {
                ScreenCase screenCase = await GetCase(sceneName, loadSceneMode, false);

                if (!(argument is PseudoElement))
                    LoadingProcessor.Instance.RegisterLoadingModel(argument, screenCase.Scene);

                screenCase.Dispose();
            }

            public static async void LoadingWithResourceCleanupUsingLoadingScene<T>(T argument, string sceneName, string loadingScreenName)
            {
                ScreenCase loadingSceneScreenCase = await GetCase(loadingScreenName, LoadSceneMode.Single, true);
                ScreenCase targetScreenCase = await GetCase(sceneName, LoadSceneMode.Additive, false);

                if (!(argument is PseudoElement))
                    LoadingProcessor.Instance.RegisterLoadingModel(argument, targetScreenCase.Scene);

                LoadingProcessor.Instance.RegisterLoadingScreenModel(targetScreenCase.Scene, loadingSceneScreenCase.Handle);

                targetScreenCase.Dispose();
            }

            public void Dispose()
            {
                Handle.Result.ActivateAsync();
                Addressables.Release(Handle);
            }
        }
        
        private class PseudoElement { }
    }
}
