using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IJunior.TypedScenes
{
    public class TypedProcessor : MonoBehaviour
    {
        public virtual string SceneName { get; }

        private void Awake()
        {
            foreach (var handler in FindObjectsOfType<MonoBehaviour>().OfType<ITypedAwakeHandler>())
            {
                handler.OnSceneAwake();
            }
            LoadingProcessor.Instance.ApplyLoadingModel(SceneName);
        }
    }
}
