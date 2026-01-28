using System.Collections;
using UnityEngine;

namespace BJ
{
    /**
    * @brief A default transition that runs immediately and exits.
    */
    public class TransitionNone : LevelTransitionEffect
    {
        public override IEnumerator CurtainsDown()
        {
            yield break;
        }

        public override IEnumerator CurtainsUp()
        {
            yield break;
        }

        public override void JumpToCurtainsDown()
        {
            //skip
        }

        public override void JumpToCurtainsUp()
        {
            // skip
        }

        public override IEnumerator UpdateProgress(double progress)
        {
            yield break;
        }
    }
}
