using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

namespace BJ
{
    /**
    * @brief A reusable system for handling scene transitions with different visual effects between levels.
    */
    public class LevelLoader : MonoBehaviour
    {
        public static readonly string DEFAULT_ANIMATION_NONE = "None";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            GameObject levelLoaderGO = new GameObject("LevelLoader");
            levelLoaderGO.AddComponent<LevelLoader>();
        }

        public delegate void SceneTransitionEvent(string scene);
        /**
         * @brief An event fired when any new scene is requested to be loaded, but before the animation starts.
         * @param scene The scene that will be loaded.
         */
        public SceneTransitionEvent OnSceneLoadRequested;
        /**
         * @brief An event fired when any new scene is requested to be loaded, after the blackout animation has played.
         * @param scene The scene that will be loaded.
         */
        public SceneTransitionEvent OnSceneBlackout;
        /**
         * @brief An event fired when a new scene is loaded, but the blackout is still active. Can be used to add load reasons or for initialization.
         * @param scene The scene that was loaded.
         */
        public SceneTransitionEvent OnSceneLoaded;
        /**
         * @brief An event fired when a new scene has loaded, all load reasons are cleared, and the transition out animation has played.
         * @param scene The scene that was loaded.
         */
        public SceneTransitionEvent OnBlackoutLifted;

        private static LevelLoader instance;
        public static LevelLoader Instance { get => instance; }

        private HashSet<string> loadHoldReasons = new HashSet<string>();

        private Dictionary<string, LevelTransitionEffect> transitions;

        private string defaultAnimation = DEFAULT_ANIMATION_NONE;

        // Internal tracker for if a level load operation is in progress.
        private bool loadingLevel = false;

        private void Awake()
        {
            if (instance == null)
            {
                DontDestroyOnLoad(this.gameObject);
                instance = this;
                transitions = new Dictionary<string, LevelTransitionEffect>();

                GameObject defaultTransition = new GameObject("DefaultTransition");
                TransitionNone baseTransition = defaultTransition.AddComponent<TransitionNone>();
                RegisterTransition(DEFAULT_ANIMATION_NONE, baseTransition);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            OnSceneLoaded?.Invoke(SceneManager.GetActiveScene().name);
        }

        /**
         * @brief Adds a new transition to the list of available transitions. It will be marked to not destroy on load and deactivated.
         * @param id         The text id to refer to the transition by.
         * @param transition A reference to the transition effect, the game object the transition effect is attached to must be the root of the effect.
         *
         * @return True if the register was successful.
         */
        public bool RegisterTransition(string id, LevelTransitionEffect transition)
        {
            if (transitions.ContainsKey(id))
            {
                Debug.LogError($"ID {id} already exists.");
                return false;
            }
            else
            {
                transitions[id] = transition;
                DontDestroyOnLoad(transition.gameObject);
                transition.gameObject.SetActive(false);
                return true;
            }
        }

        /**
         * @brief Removes a transition from the list of available transitions. It can optionally be destroyed. The DontDestroyOnLoad trigger is still active.
         * @param id      The text id of the transition to delete.
         * @param destroy True to destroy the transition being removed.
         */
        public void RemoveTransition(string id, bool destroy)
        {
            if (id.Equals(DEFAULT_ANIMATION_NONE))
            {
                Debug.LogWarning("Removing the NONE animation is not allowed.");
            }
            else if (transitions.ContainsKey(id))
            {
                LevelTransitionEffect transition = transitions[id];
                transitions.Remove(id);
                if (destroy)
                {
                    Destroy(transition.gameObject);
                }

                if (id.Equals(defaultAnimation))
                {
                    defaultAnimation = DEFAULT_ANIMATION_NONE;
                    Debug.LogWarning("Removed the default animation {id}, default is reverting to NONE.");
                }
            }
            else
            {
                Debug.LogError($"ID {id} does not exist.");
            }
        }

        /**
         * @brief Runs the sequence of level loading with a different transition for blackout and curtains up.
         *        The handoff happens immediately after the 100% transition progress update completes.
         * @param level      The new level to load.
         * @param blackOut   The animation to play for curtains down.
         * @param curtainsUp The animation to play for curtains up.
         */
        public void LoadLevel(string level, string blackOut, string curtainsUp)
        {
            if (!transitions.ContainsKey(blackOut))
            {
                Debug.LogError ($"Blackout Effect {blackOut} does not exist, using default.");
                blackOut = defaultAnimation;
            }
            // not else, can be both
            if (!transitions.ContainsKey(curtainsUp))
            {
                Debug.LogError ($"Curtain Up Effect {curtainsUp} does not exist, using default.");
                curtainsUp = defaultAnimation;
            }

            StartCoroutine (InternalLevelLoad(level, blackOut, curtainsUp));
        }

        /**
         * @brief Runs the sequence of level loading with the given transition.
         * @param level     The new level to load.
         * @param transtion The animation to play.
         */
        public void LoadLevel(string level, string transtion)
        {
            if (!transitions.ContainsKey(transtion))
            {
                Debug.LogError ($"Transition Effect {transtion} does not exist, using default.");
                transtion = defaultAnimation;
            }

            LoadLevel(level, transtion, transtion);
        }

        /**
         * @brief Assigns a transition to be used if for whatever reason the requested transition does not exist.
         *
         * @param newDefault The new default transition to play, must be previously registered.
         */
        public void SetDefaultTransition(string newDefault)
        {
            if (!transitions.ContainsKey(newDefault))
            {
                Debug.LogError($"Attempted to set default transition to {newDefault} but it has not been registered.");
            }
            else
            {
                defaultAnimation = newDefault;
            }
        }

        /**
         * @brief Internally run the level loading sequence. If blackOut is the same as curtainsUp there is no handoff.
         * @param level      The new level to load.
         * @param blackOut   The animation to play for curtains down.
         * @param curtainsUp The animation to play for curtains up.
         */
        private IEnumerator InternalLevelLoad(string level, string blackOut, string curtainsUp)
        {
            if (loadingLevel)
            {
                Debug.LogError("Level Load attempted while another load is in progress.");
                yield break; // Exit early
            }
            loadingLevel = true;

            Debug.Log($"Loading level {level}, starting curtains down.");

            OnSceneLoadRequested?.Invoke(level);

            LevelTransitionEffect transitionOut = transitions[blackOut];
            transitionOut.gameObject.SetActive(true);
            transitionOut.JumpToCurtainUp();

            yield return transitionOut.CurtainDown();

            Debug.Log ($"Loading level {level}, completed curtains down.");

            OnSceneBlackout?.Invoke(level);

            // Do level loading
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(level);

            while (!loadOperation.isDone)
            {
                // Leaves some space for load holds
                double dynamicProgress = loadOperation.progress / 1.1;
                yield return transitionOut.UpdateProgress(dynamicProgress);
                yield return null;
            }

            OnSceneLoaded?.Invoke(level);
            // Wait for load reasons
            while (loadHoldReasons.Count != 0)
            {
                yield return null;
            }

            LevelTransitionEffect transitionIn = transitionOut;

            // Guarentee you get a call at 100%
            yield return transitionOut.UpdateProgress(1.0);

            Debug.Log ($"Loading level {level}, starting curtains up.");

            // If the transition out and in are different this is where the handoff happens, otherwise it is skipped.
            if (!blackOut.Equals(curtainsUp))
            {
                transitionIn = transitions[curtainsUp];

                transitionOut.gameObject.SetActive(false);
                transitionIn.gameObject.SetActive(true);
                transitionIn.JumpToCurtainDown();
            }

            yield return transitionIn.CurtainUp();

            transitionIn.gameObject.SetActive(false);

            Debug.Log($"Loading level {level}, completed curtains up.");

            OnBlackoutLifted?.Invoke(level);

            // We can now load a new level
            loadingLevel = false;
        }
    }
}
