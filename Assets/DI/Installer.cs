using UnityEngine;

namespace DI
{
    public abstract class Installer : MonoBehaviour
    {
        protected Container Container;

        public void Init()
        {
            Container = new();
            InstallBindings();
            InjectSceneMonobehaviours();
        }

        private void InjectSceneMonobehaviours()
        {
            var sceneMonoBehaviours = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var monoBehaviour in sceneMonoBehaviours)
            {
                Container.InjectMonoBehaviour(monoBehaviour);
            }
        }

        protected abstract void InstallBindings();
    }
}
