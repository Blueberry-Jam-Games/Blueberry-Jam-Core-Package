using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BJ
{
    public class TransitionFade : LevelTransitionEffect
    {
        [Header("Animation Components")]
        [SerializeField]
        private CanvasGroup overallImage;
        [SerializeField]
        private Slider loadBar;

        [Header("Configuration Components")]
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Image unloadedColour;
        [SerializeField]
        private Image loadedColour;

        private float animationTime = 1f;
        private bool useLoadBar = true;

        public override IEnumerator CurtainDown()
        {
            float time = 0f;

            while (time < animationTime)
            {
                time += Time.deltaTime;
                float newAlpha = Mathf.Clamp01(time / animationTime);
                overallImage.alpha = newAlpha;

                yield return null;
            }

            overallImage.alpha = 1.0f;
        }

        public override IEnumerator CurtainUp()
        {
            float time = 0f;

            while (time < animationTime)
            {
                time += Time.deltaTime;
                float newAlpha = 1.0f - Mathf.Clamp01(time / animationTime);
                overallImage.alpha = newAlpha;

                yield return null;
            }

            overallImage.alpha = 0.0f;
        }

        public override void JumpToCurtainDown()
        {
            overallImage.alpha = 1.0f;
            loadBar.value = 1.0f;
        }

        public override void JumpToCurtainUp()
        {
            overallImage.alpha = 0.0f;
            loadBar.value = 0.0f;
        }

        public override IEnumerator UpdateProgress(double progress)
        {
            loadBar.value = (float)progress;
            yield break;
        }

        public void Init(float animationTime, Color background, bool progressBar, Color unloaded, Color loaded)
        {
            this.animationTime = animationTime;
            this.backgroundImage.color = background;
            this.useLoadBar = progressBar;
            this.loadBar.gameObject.SetActive(useLoadBar);
            this.unloadedColour.color = unloaded;
            this.loadedColour.color = loaded;
            JumpToCurtainUp();
        }
    }
}
