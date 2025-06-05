using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Mirror
{
    public class MirrorDistanceFade : UdonSharpBehaviour
    {
        [SerializeField] private Animator fadeAnimator;
        private readonly int _bool = Animator.StringToHash("Bool");

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            fadeAnimator.SetBool(_bool, true);
            fadeAnimator.enabled = true;
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            fadeAnimator.SetBool(_bool, false);
            fadeAnimator.enabled = true;
        }
    }
}
