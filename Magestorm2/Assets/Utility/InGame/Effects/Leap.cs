using UnityEngine;

namespace Magus
{
    public class Leap : AppliedEffect
    {
        public override void ApplyEffect(Avatar appliedTo)
        {
            base.ApplyEffect(appliedTo);
            if (_appliedToPlayer)
            {
                ComponentRegister.PlayerMovement.MarkLeapPending(_degree);
                //ComponentRegister.PlayerMovement.MarkEntangled(true);
            }
        }

        public override void ReverseEffect()
        {
            base.ReverseEffect();
            //ComponentRegister.PlayerMovement.MarkEntangled(false);
        }
    }
}
