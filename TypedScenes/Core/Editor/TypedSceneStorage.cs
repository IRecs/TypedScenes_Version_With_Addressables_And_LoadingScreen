#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace IJunior.TypedScenes
{
    public static class TypedSceneStorage
    {
        public static void Save(string fileName, string sourceCode)
        {
            string path = CreatePath(fileName);

            if (File.Exists(path) && File.ReadAllText(path) == sourceCode)
                return;
                 
            File.WriteAllText(path, sourceCode);
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        public static bool CheckFile(string fileName)
        {
            string path = CreatePath(fileName);
            return File.Exists(path);
        }

        private static string CreatePath(string fileName)
        {
            string directory = GetDirectory(fileName);
            return directory + fileName + TypedSceneSettings.ClassExtension;
        }

        private static string GetDirectory(string fileName)
        {
            string directory;

            if (fileName.Contains(TypedSceneSettings.TypedProcessor))
                directory = TypedSceneSettings.SavingDirectoryTypedProcessor;
            else
                directory = TypedSceneSettings.SavingDirectory;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return directory;
        }

        public static void Delete(string className)
        {
            DeleteFile(TypedSceneSettings.SavingDirectory + className + TypedSceneSettings.ClassExtension);
            DeleteFile(TypedSceneSettings.SavingDirectoryTypedProcessor + className + TypedSceneSettings.TypedProcessor + TypedSceneSettings.ClassExtension);
        }

        private static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                AssetDatabase.DeleteAsset(path);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
#endif
