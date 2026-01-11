using UnityEngine;
using System.Collections;

namespace BJ
{
    /**
     * @brief A Level Transition Effect contains all the information for an animation to "black out" a scene and restore the scene afterwards.
     *        It can be implemented however needed.
     */
    public abstract class LevelTransitionEffect : MonoBehaviour
    {
        /**
         * @brief "Black out" the screen so that level loading can happen in the background.
         *        This is a coroutine so returning from here indicates the screen is "blacked out" and the level can be loaded.
         */
        public abstract IEnumerator CurtainDown ();
        /**
         * @brief Jump to the "Blacked out" state for this animation. It should be ready for Curtain Up immediately after.
         */
        public abstract void JumpToCurtainDown ();
        /**
         * @brief Update the animation based on the level loading progress.
         *        This is a coroutine, delaying here will be fine, the level will load in the background,
         *        but the next progress update will be delayed until the coroutine returns.
         */
        public abstract IEnumerator UpdateProgress (double progress);
        /**
         * @brief Fade in to the loaded scene. Can be animated how you like, returning here indicates the end of the load procedure.
         */
        public abstract IEnumerator CurtainUp ();
        /**
         * @brief Jump to the "Curtain Up" state for this animation. It should be ready for Black Out immediately after.
         */
        public abstract void JumpToCurtainUp ();

        /**
         * @brief Grants access to built in level transition effects.
         */
        public static class Templates
        {
            /**
            * @brief Creates and registers a fade transition with the given properties to the level loader.
            *
            * @param id         The id for the transition system to use.
            * @param duration   How long to play the animation for.
            * @param background The colour to apply to the background during load.
            * @param useLoadBar True to enable the load bar.
            * @param unloaded   The colour to use on the load bar for the right hand (unloaded) side.
            * @param loaded     The colour to use on the load bar for the left hand (loaded) side.
            *
            * @return The game object representing the transition created.
            */
            public static GameObject FadeTransition (string id, float duration, Color background, bool useLoadBar, Color unloaded, Color loaded)
            {
                // Load the prefab resource and make a new one in the world
                GameObject transitionEffect = GameObject.Instantiate (Resources.Load<GameObject> ("DefaultTransitions/FadeTransition"));
                // Init config values
                TransitionFade fade = transitionEffect.GetComponent<TransitionFade> ();
                fade.Init (duration, background, useLoadBar, unloaded, loaded);
                // Register the transition
                LevelLoader.Instance.RegisterTransition (id, fade);
                return transitionEffect;
            }
        }
    }
}
