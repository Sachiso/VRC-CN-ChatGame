using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp.Sound
{
    public class AudioToggle : UdonSharpBehaviour
    {
        [SerializeField] private Animator buttonSelf;
        [SerializeField] private Animator[] buttonOthers;
        [SerializeField] private AudioSource targetSelf;
        [SerializeField] private AudioSource[] targetOthers;
        private readonly int _bool = Animator.StringToHash("Bool");

        public override void Interact()
        {
            bool value = !buttonSelf.GetBool(_bool);
            buttonSelf.SetBool(_bool, value);
            buttonSelf.enabled = true;
            
            foreach (Animator otherButton in buttonOthers)
            {
                otherButton.SetBool(_bool, false);
                otherButton.enabled = true;
            }

            if (targetSelf) targetSelf.enabled = value;
            foreach (AudioSource otherTarget in targetOthers)
            {
                otherTarget.enabled = false;
            }
        }

        public void StopAnimator()
        {
            buttonSelf.enabled = false;
        }
    }
}