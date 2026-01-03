using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    internal class CoroutineHelper : SingletonGameObject<CoroutineHelper>
    {
        internal void DoNextFrame(Action action)
        {
            StartCoroutine(DoNextFrameHelper(action));
        }

        private IEnumerator DoNextFrameHelper(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        internal void DoAtEndOfFrame(Action action)
        {
            StartCoroutine(DoAtEndOfFrameHelper(action));
        }

        private IEnumerator DoAtEndOfFrameHelper(Action action)
        {
            yield return BJ.Coroutines.waitForEndOfFrame;
            action?.Invoke();
        }

        internal void DoAtFixedUpdate(Action action)
        {
            StartCoroutine(DoAtFixedUpdateHelper(action));
        }

        private IEnumerator DoAtFixedUpdateHelper(Action action)
        {
            yield return BJ.Coroutines.waitForFixedUpdate;
            action?.Invoke();
        }

        internal void DoInSeconds(float seconds, Action action)
        {
            StartCoroutine(DoInSecondsHelper(seconds, action));
        }

        private IEnumerator DoInSecondsHelper(float seconds, Action action)
        {
            yield return BJ.Coroutines.WaitforSeconds(seconds);
            action?.Invoke();
        }
    }
}
