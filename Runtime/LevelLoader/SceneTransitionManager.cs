using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    public class SceneTransitionManager
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            // This config prefab is holds settings that allow th user to customize how the scene transitioner works for them.
            // e.g. What sprites (that communicates to the player that a level is loading) are displayed during scene transitions? How long does fading to black and lifting blackout take? etc...
            GameObject scene_transitioner_config = Resources.Load<GameObject>("SceneTransitionerManagerExamples/SceneTransitionerConfig");
            if (scene_transitioner_config == null)
            {
                Debug.LogError("The configuaration prefab for the SceneTransitioner could not be found. Please ensure that one exists in the Assets/Resources directory of your project");
            }

            GameObject scene_transitioner_config_instance = GameObject.Instantiate(scene_transitioner_config);
            if (scene_transitioner_config_instance == null)
            {
                Debug.LogError("Failed to create a GameObject instance from the configuration prefab");
            }

            if (scene_transitioner_config_instance.TryGetComponent<SceneTransitionManagerHelper>(out SceneTransitionManagerHelper sch))
            {
                mSceneTransitionerHelper = sch;
            }
            else
            {
                Debug.LogError("Failed to get the SceneTransitionerHelper component from the SceneTransitionerHelper GameObject instance, please ensure that a SceneTransitionerHelper script is added to the configuration prefab");
            }
        }

        private static SceneTransitionManagerHelper mSceneTransitionerHelper;

        public static void LoadNewScene(string SceneName)
        {
            mSceneTransitionerHelper.LoadNewScene(SceneName);
        }
    }
}