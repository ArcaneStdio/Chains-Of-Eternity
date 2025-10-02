using System;
using System.Threading.Tasks;
#if UNITY_EDITOR && PURR_LEAKS_CHECK
using PurrNet.Pooling;
#endif
using UnityEngine;

namespace PurrNet
{
    [DefaultExecutionOrder(32000)]
    public class UnityLatestUpdate : MonoBehaviour
    {
        static UnityLatestUpdate _instance;

        public static event Action onUpdate;

        public static event Action onFixedUpdate;

        public static event Action onLatestUpdate;

        public static Task Yield()
        {
            var promise = new TaskCompletionSource<bool>();

            onUpdate += OnUpdate;

            return promise.Task;

            void OnUpdate()
            {
                if (promise.TrySetResult(true))
                    onUpdate -= OnUpdate;
            }
        }

        public static Task WaitSeconds(float seconds)
        {
            var promise = new TaskCompletionSource<bool>();
            float timer = 0f;

            onUpdate += OnUpdate;

            return promise.Task;

            void OnUpdate()
            {
                timer += Time.deltaTime;
                if (timer >= seconds && promise.TrySetResult(true))
                    onUpdate -= OnUpdate;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnSubsystemRegistration()
        {
            if (_instance)
                return;

            var go = new GameObject("PurrNet_UnityLatestUpdate")
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            DontDestroyOnLoad(go);

            _instance = go.AddComponent<UnityLatestUpdate>();
        }

#if UNITY_EDITOR && PURR_LEAKS_CHECK
        private float _sweep;
#endif

        private void Update()
        {
            onUpdate?.Invoke();
#if UNITY_EDITOR && PURR_LEAKS_CHECK
            _sweep += Time.deltaTime;

            if (_sweep >= 1f)
            {
                _sweep = 0f;
                AllocationTracker.CheckForLeaks();
            }
#endif
        }

        private void FixedUpdate()
        {
            onFixedUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            onLatestUpdate?.Invoke();
        }
    }
}
