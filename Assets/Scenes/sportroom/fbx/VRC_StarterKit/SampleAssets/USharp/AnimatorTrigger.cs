using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp
{
    public class AnimatorTrigger : UdonSharpBehaviour
    {
        public Animator self;
        public Animator target;
        
        public override void Interact()
        {
            if (self) self.enabled = true;
            if (target) target.enabled = true;
        }

        public void StopAnimator()
        {
            if (self) self.enabled = false;
        }
    }
}