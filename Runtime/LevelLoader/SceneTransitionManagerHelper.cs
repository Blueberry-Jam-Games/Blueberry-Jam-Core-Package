using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BJ
{
    internal class SceneTransitionManagerHelper : SingletonGameObject<SceneTransitionManagerHelper>
    {
        [Header("Game Object References")]
        [SerializeField] private CanvasGroup Crossfade;

        [Header("Animation Configuration")]
        [SerializeField] private float TransitionTime = 1f;
        [SerializeField] private AnimationCurve EaseTransitionCurve;


        [SerializeField] private Image FadetoBlack;
        [SerializeField] private Image ShowAct;
        [SerializeField] private float WaitTime = 3f;
        private int current_act;
        [SerializeField] private Sprite[] Acts;

        /* ----- Helper Variables -----*/
        private float StartTransitionTime;

        protected override void Awake()
        {
            base.Awake();
            CheckSceneTransitionerManagerSettings();
            Crossfade.blocksRaycasts = false;

            StartTransitionTime = 0.0f;
            current_act = 0;
        }

        internal void LoadNewScene(string SceneName)
        {
            Crossfade.blocksRaycasts = true;
            StartCoroutine(LoadLevelAnim(SceneName));
            Crossfade.blocksRaycasts = false;
        }

        public bool disableCharacterMovement = false;

        private IEnumerator LoadLevelAnim(string SceneName)
        {
            Crossfade.blocksRaycasts = true;
            /* ----- Fade to black (Start) ----- */
            Crossfade.gameObject.SetActive(true);
            ShowAct.enabled = false;
            FadetoBlack.enabled = true;
            disableCharacterMovement = true;

            StartTransitionTime = Time.time;
            while (Time.time - StartTransitionTime < TransitionTime)
            {
                Crossfade.alpha = EaseTransitionCurve.Evaluate(Time.time - StartTransitionTime);
                yield return null;
            }

            if (current_act < Acts.Length)
            {
                FadetoBlack.enabled = false;
                /* ----- Fade to black (End) */

                /* ----- Load Level (Start) ----- */
                ShowAct.sprite = Acts[current_act];
                ShowAct.enabled = true;

                Color image = ShowAct.color;
                image.a = 255;
                ShowAct.color = image;
            }
            
            AsyncOperation load_operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneName);

            while (!load_operation.isDone)
            {
                yield return null;
            }

            StartTransitionTime = Time.time;
            while (Time.time - StartTransitionTime < WaitTime)
            {
                yield return null;
            }

            /* ----- Load Level (End) ----- */

            /* ----- Unfade from black (Start) ----- */
            StartTransitionTime = Time.time;

            while (Time.time - StartTransitionTime < TransitionTime)
            {
                if (current_act < Acts.Length)
                {
                    Color image = ShowAct.color;
                    image.a = EaseTransitionCurve.Evaluate(TransitionTime - (Time.time - StartTransitionTime));
                    ShowAct.color = image;
                }
                else
                {
                    Crossfade.alpha = EaseTransitionCurve.Evaluate(TransitionTime - (Time.time - StartTransitionTime));
                }
                
                yield return null;
            }

            Crossfade.gameObject.SetActive(false);
            /* ----- Unfade from black (End) ----- */
            Scene start_scene = SceneManager.GetSceneByBuildIndex(0);
            Scene current_scene = SceneManager.GetSceneByName(SceneName);
            if (start_scene == current_scene)
            {
                current_act = 0;
            }
            else if (current_act < Acts.Length + 1)

            {
                current_act++;
            }
            else
            {
                Debug.LogError("An act interlude is not available");
            }
            
            disableCharacterMovement = false;
            Crossfade.blocksRaycasts = false;
        }

        private void CheckSceneTransitionerManagerSettings()
        {
            Debug.Assert(Crossfade != null, "Crossfade GameObject is not set. Please assign it so that the Scene Transitioner can fade to black and lift the black when level loading is complete");
            Debug.Assert(EaseTransitionCurve != null, "EaseTransitionCurve is not set. Please specify an AnimationCurve so that you can specify the speed at which the screen fades to black and lifts the blackout");
        }
    }
}