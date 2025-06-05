using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp
{
    public class StopAnimatorHandler : UdonSharpBehaviour
    {
        public void StopAnimator()
        {
            Animator self = this.GetComponent<Animator>();
            if (!self) return;
            self.enabled = false;
        }
    }
}
