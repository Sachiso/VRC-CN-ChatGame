using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp
{
    public class ObjectToggle : UdonSharpBehaviour
    {
        [SerializeField] private Animator buttonSelf;
        [SerializeField] private Animator[] buttonOthers;
        [SerializeField] private Animator allToggleController;
        [SerializeField] private GameObject targetSelf;
        [SerializeField] private GameObject[] targetOthers;
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

            bool allToggle = null != allToggleController;
            if (allToggle)
            {
                allToggleController.SetBool(_bool, value);
                allToggleController.enabled = true;
            }
            
            if (targetSelf) targetSelf.SetActive(allToggle || value);
            foreach (GameObject otherTarget in targetOthers)
            {
                otherTarget.SetActive(false);
            }
        }

        public void StopAnimator()
        {
            buttonSelf.enabled = false;
        }
    }
}