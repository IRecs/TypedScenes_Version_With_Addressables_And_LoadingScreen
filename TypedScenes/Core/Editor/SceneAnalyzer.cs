#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public static class SceneAnalyzer
    {
        public static IEnumerable<Type> GetLoadingParameters(Scene analyzableScene)
        {
            var loadParameters = new List<Type> {null};
            var componentTypes = GetAllTypes(analyzableScene);

            loadParameters.AddRange(componentTypes
                .Where(type => type.GetInterfaces()
                    .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ISceneLoadHandler<>)))
                .SelectMany(type => type.GetMethods().Where(method => method.Name == "OnSceneLoaded"),
                    (type, method) => method.GetParameters()[0].ParameterType));

            if (loadParameters.Count > 1)
                loadParameters.Remove(null);

            return loadParameters;
        }

        public static bool CheckUnloadingLoadingScene(Scene analyzableScene)
        {
            var types = GetAllTypes(analyzableScene);
            return types.Any(type => type.GetInterfaces().Any(x => x == typeof(IUnloadingLoadingScene)));
        }

        public static bool TryAddTypedProcessor<T>(Scene analyzableScene, T type) where T : TypedProcessor
        {
            var componentTypes = GetAllTypes(analyzableScene);
            
            if (componentTypes.Contains(typeof(T))) return false;

            var gameObject = new GameObject(analyzableScene.name + "TypedProcessor");
            gameObject.AddComponent<T>();
            analyzableScene.GetRootGameObjects().Append(gameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Typed processor added");
            EditorSceneManager.SaveScene(analyzableScene);
            return true;
        }

        private static IEnumerable<Component> GetAllComponents(Scene activeScene)
        {
            var rootObjects = activeScene.GetRootGameObjects();
            var components = new List<Component>();

            foreach (var gameObject in rootObjects)
            {
                components.AddRange(gameObject.GetComponentsInChildren<Component>());
            }

            return components;
        }

        private static IEnumerable<Type> GetAllTypes(Scene activeScene)
        {
            var components = GetAllComponents(activeScene);
            var types = new HashSet<Type>();

            foreach (var component in components)
            {
                types.Add(component.GetType());
            }

            return types;
        }
    }
}
#endif
