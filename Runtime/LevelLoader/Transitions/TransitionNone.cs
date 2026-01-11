using System.Collections;
using UnityEngine;

namespace BJ
{
    /**
    * @brief A default transition that runs immediately and exits.
    */
    public class TransitionNone : LevelTransitionEffect
    {
        public override IEnumerator CurtainDown()
        {
            yield break;
        }

        public override IEnumerator CurtainUp()
        {
            yield break;
        }

        public override void JumpToCurtainDown()
        {
            //skip
        }

        public override void JumpToCurtainUp()
        {
            // skip
        }

        public override IEnumerator UpdateProgress(double progress)
        {
            yield break;
        }
    }
}
