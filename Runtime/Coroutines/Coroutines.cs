using System;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    /**
     * @brief A collection of coroutine helpers, most of them are based around saving the memory fragmentation costs of creating new WaitFors every frame.
     */
    public class Coroutines
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            GameObject coroutineHelperGO = new GameObject("CoroutineHelper");
            coroutineHelper = coroutineHelperGO.AddComponent<CoroutineHelper>();
        }

        private static CoroutineHelper coroutineHelper;

        public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
        public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

        private static Dictionary<float, WaitForSeconds> waitforSecondsCache = new Dictionary<float, WaitForSeconds>()
        {
            {0f, new WaitForSeconds(0f)},
            {0.1f, new WaitForSeconds(0.1f)},
            {0.2f, new WaitForSeconds(0.2f)},
            {0.25f, new WaitForSeconds(0.25f)},
            {0.3f, new WaitForSeconds(0.3f)},
            {0.4f, new WaitForSeconds(0.4f)},
            {0.5f, new WaitForSeconds(0.5f)},
            {0.6f, new WaitForSeconds(0.6f)},
            {0.7f, new WaitForSeconds(0.7f)},
            {0.75f, new WaitForSeconds(0.75f)},
            {0.8f, new WaitForSeconds(0.8f)},
            {0.9f, new WaitForSeconds(0.9f)},
            {1.0f, new WaitForSeconds(1.0f)},
            {1.5f, new WaitForSeconds(1.5f)},
            {2.0f, new WaitForSeconds(2.0f)},
        };

        // It is not worth the cost of caching random delay values, as such just use normal new WaitForSeconds.
        // The cost is not the RAM of storing it but rather any resize costs incurred on the storage container.
        public static WaitForSeconds WaitforSeconds(float time)
        {
            if (waitforSecondsCache.TryGetValue(time, out WaitForSeconds result))
            {
                return result;
            }
            else
            {
                return new WaitForSeconds(time);
            }
        }

        public static void DoNextFrame(Action action)
        {
            coroutineHelper.DoNextFrame(action);
        }

        public static void DoAtEndOfFrame(Action action)
        {
            coroutineHelper.DoAtEndOfFrame(action);
        }

        public static void DoAtFixedUpdate(Action action)
        {
            coroutineHelper.DoAtFixedUpdate(action);
        }

        public static void DoInSeconds(float seconds, Action action)
        {
            coroutineHelper.DoInSeconds(seconds, action);
        }
    }
}
