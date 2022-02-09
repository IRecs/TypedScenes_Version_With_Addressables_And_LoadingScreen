#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;

namespace IJunior.TypedScenes
{
    public static class TypedSceneValidator
    {
        public static bool DetectSceneImport(string assetPath, out string validScenePath)
        {            
            validScenePath = null;

            if (Path.GetExtension(assetPath) != TypedSceneSettings.SceneExtension)
                return false;

            using (var analyzableScene = AnalyzableScene.Create(assetPath))
            {
                var validName = GetValidName(analyzableScene.Name);

                if (analyzableScene.Name != validName)
                {
                    var validPath = Path.GetDirectoryName(assetPath) + Path.DirectorySeparatorChar + validName + TypedSceneSettings.SceneExtension;
                    File.Move(assetPath, validPath);
                    File.Delete(assetPath + TypedSceneSettings.MetaExtension);
                    AssetDatabase.ImportAsset(validPath, ImportAssetOptions.ForceUpdate);
                    return false;
                }

                if(!CheckFile(analyzableScene))
                    return false;

                if (!TryAddTypedProcessor(analyzableScene))
                    return false;

                validScenePath = analyzableScene.AssetPath;
                return true;
            }
        }

        private static bool CheckFile(AnalyzableScene analyzableScene)
        {
            if (!TypedSceneStorage.CheckFile(analyzableScene.Name + TypedSceneSettings.TypedProcessor))
            {
                var sourceCodeProcessor = TypedProcessorGenerator.Generate(analyzableScene.Scene);
                TypedSceneStorage.Save(analyzableScene.Name + TypedSceneSettings.TypedProcessor, sourceCodeProcessor);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool TryAddTypedProcessor(AnalyzableScene analyzableScene)
        {
            dynamic type = GetType(analyzableScene);

            if (SceneAnalyzer.TryAddTypedProcessor(analyzableScene.Scene, type))
                return false;

            return true;
        }

        private static dynamic GetType(AnalyzableScene analyzableScene)
        {
            System.Reflection.Assembly assem = typeof(TypedSceneValidator).Assembly;
            string name = $"{TypedSceneSettings.Namespace}.{analyzableScene.Name}{TypedSceneSettings.TypedProcessor}";

            Type type = assem.GetType(name);

            if (type == null)
                throw new Exception($"Type {name} is null");

            return Activator.CreateInstance(type);
        }

        public static bool DetectSceneDeletion(string sceneName)
        {
            var assets = AssetDatabase.FindAssets(sceneName);

            return (assets.Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => new {path, name = Path.GetFileNameWithoutExtension(path)})
                .Where(@t => @t.name == sceneName)
                .Select(@t => @t.path)).Any(path => Path.GetExtension(path) == TypedSceneSettings.SceneExtension);
        }

        private static string GetValidName(string sceneName)
        {
            var stringBuilder = new StringBuilder();

            if (!char.IsLetter(sceneName[0]) && sceneName[0] != '_')
                stringBuilder.Append('_');

            foreach (var symbol in sceneName)
            {
                stringBuilder.Append((char.IsLetterOrDigit(symbol) || symbol == '_') ? symbol : '_');
            }

            return stringBuilder.ToString();
        }
    }
}
#endif
