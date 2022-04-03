using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public class LoadingProcessor : MonoBehaviour
    {
        private static LoadingProcessor _instance;

        private Action _loadingModelAction;
        private Action _loadingScreenModelAction;

        private Dictionary<string, Action> _loadingModelActions = new Dictionary<string, Action>();
        private Dictionary<string, Action> _loadingScreenModelActions = new Dictionary<string, Action>();

        public static LoadingProcessor Instance
        {
            get
            {
                if (_instance == null)
                    Initialize();

                return _instance;
            }
        }

        private static void Initialize()
        {
            _instance = new GameObject(nameof(LoadingProcessor)).AddComponent<LoadingProcessor>();
            _instance.transform.SetParent(null);
            DontDestroyOnLoad(_instance);
        }

        public void ApplyLoadingModel(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName))
                return;

            if (_loadingModelActions.ContainsKey(sceneName))
            {
                _loadingModelActions[sceneName]?.Invoke();
                _loadingModelActions.Remove(sceneName);
            }
            if (_loadingScreenModelActions.ContainsKey(sceneName))
            {
                _loadingScreenModelActions[sceneName]?.Invoke();
                _loadingScreenModelActions.Remove(sceneName);
            }
        }

        public void RegisterLoadingModel<T>(T loadingModel, Scene scene)
        {
            Action action = () =>
            {
                foreach (var rootObjects in scene.GetRootGameObjects())
                {
                    foreach (var handler in rootObjects.GetComponentsInChildren<ISceneLoadHandler<T>>())
                    {
                        handler.OnSceneLoaded(loadingModel);
                    }
                }
            };
            _loadingModelActions.Add(scene.name, action);
        }

        public void RegisterLoadingScreenModel(Scene targetScene, AsyncOperationHandle<SceneInstance> loadingScreen)
        {
            Action action = () =>
            {
                int count = 0;

                foreach (var rootObjects in targetScene.GetRootGameObjects())
                {
                    foreach (var handler in rootObjects.GetComponentsInChildren<IUnloadingLoadingScene>())
                    {
                        ++count;

                        if (count > 1)
                        {
                            throw new InvalidOperationException($"The {nameof(IUnloadingLoadingScene)} interface must be implemented once per scene \nScene that caused the error {targetScene.name} \nRootObjects {rootObjects.name}");
                        }

                        handler.SetLoadingScreen(loadingScreen);
                    }
                }

                if (count == 0)
                {
                    throw new InvalidOperationException($"No implementation {nameof(IUnloadingLoadingScene)}. Scene {targetScene.name}");
                }
            };

            _loadingScreenModelActions.Add(targetScene.name, action);
        }
    }
}
