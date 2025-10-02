using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace PurrNet.Modules
{
    public static class SceneObjectsModule
    {
        public static event Action<Scene> onPreSceneLoad;

        public static event Action<Scene> onPostSceneLoad;

        private static readonly List<NetworkIdentity> _sceneIdentities = new List<NetworkIdentity>();

        public static void GetSceneIdentities(Scene scene, List<NetworkIdentity> networkIdentities)
        {
            onPreSceneLoad?.Invoke(scene);

            var rootGameObjects = scene.GetRootGameObjects();

            PurrSceneInfo sceneInfo = null;

            foreach (var rootObject in rootGameObjects)
            {
                if (rootObject.TryGetComponent<PurrSceneInfo>(out var si))
                {
                    sceneInfo = si;
                    break;
                }
            }

            if (sceneInfo)
                rootGameObjects = sceneInfo.rootGameObjects.ToArray();

            for (var i = 0; i < rootGameObjects.Length; i++)
            {
                var rootObject = rootGameObjects[i];

                if (!rootObject || rootObject.scene.handle != scene.handle) continue;

                rootObject.gameObject.GetComponentsInChildren(true, _sceneIdentities);

                if (_sceneIdentities.Count == 0) continue;

                rootObject.gameObject.MakeSureAwakeIsCalled();
                networkIdentities.AddRange(_sceneIdentities);
            }

            onPostSceneLoad?.Invoke(scene);
        }
    }
}
