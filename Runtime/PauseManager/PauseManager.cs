using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    /**
    * @brief The pause manager is responsible for sending events when the game is paused or unpaused.
    */
    public class PauseManager : SingletonGameObject<PauseManager>
    {
        public static readonly bool STATE_PAUSED = true;
        public static readonly bool STATE_UNPAUSED = false;

        // Internal type definition for events where no argument is appropriate.
        public delegate void PauseManagerEvent();
        // Internal type definition for the OnPauseStateChange, convenient when both situations need to be accounted for similarly.
        public delegate void PauseManagerNotification(bool paused);

        /**
        * @brief Fired when the game state changes from unpaused to paused. Happens after OnPauseStateChange.
        */
        public event PauseManagerEvent OnPauseGame;
        /**
        * @brief Fired when the game state changes from paused to unpaused. Happens after OnPauseStateChange.
        */
        public event PauseManagerEvent OnUnpauseGame;
        /**
        * @brief Fired when the game state changes to something else. Happens before OnPauseGame and OnUnpauseGame.
        * @param paused The state of Paused after the changing event.
        */
        public event PauseManagerNotification OnPauseStateChange;

        /**
        * @brief Fired in the Unity Update tick when the game is not paused.
        */
        public event PauseManagerEvent PausableUpdate;
        /**
        * @brief Fired in the Unity FixedUpdate tick when the game is not paused.
        */
        public event PauseManagerEvent PausableFixedUpdate;
        /**
        * @brief Fired in the Unity FixedUpdate tick when the game is not paused.
        */
        public event PauseManagerEvent PausableLateUpdate;

        /**
        * @brief The master pause bool, if this is true we are paused, if this is false we are unpaused.
        *        Alternatively use the internally defined PAUSED and UNPAUSED constants.
        */
        private bool paused = STATE_UNPAUSED;
        /**
        * @brief The game is paused if Paused is true. PausableUpdate, FixedUpdate, and LateUpdate will not be run.
        *        OnPauseStateChange and either OnPauseGame or OnUnpauseGame when this changes.
        */
        public bool Paused { get => paused; }

        private void Update()
        {
            if (paused == STATE_UNPAUSED)
            {
                PausableUpdate?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (paused == STATE_UNPAUSED)
            {
                PausableFixedUpdate?.Invoke();
            }
        }

        private void LateUpdate()
        {
            if (paused == STATE_UNPAUSED)
            {
                PausableLateUpdate?.Invoke();
            }
        }

        /**
        * @brief If the game is not already paused, pauses the game invoking OnPauseStateChange(true) and then OnPauseGame.
        */
        public void PauseGame()
        {
            if (paused == STATE_UNPAUSED)
            {
                paused = STATE_PAUSED;
                OnPauseStateChange?.Invoke(paused);
                OnPauseGame?.Invoke();
            }
        }

        /**
        * @brief If the game is paused, unpauses the game invoking OnPauseStateChange(false) and then OnUnpauseGame.
        */
        public void UnpauseGame()
        {
            if (paused == STATE_PAUSED)
            {
                paused = STATE_UNPAUSED;
                OnPauseStateChange?.Invoke(paused);
                OnUnpauseGame?.Invoke();
            }
        }
    }
}
