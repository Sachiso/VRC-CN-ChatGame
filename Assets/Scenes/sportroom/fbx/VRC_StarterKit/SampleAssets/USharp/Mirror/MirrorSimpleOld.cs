using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace VRC_StarterKit.SampleAssets.USharp.Mirror
{
    public class MirrorSimpleOld : UdonSharpBehaviour
    {
        [SerializeField] private int nthState;
        [SerializeField] private Text[] buttonTexts;
        [SerializeField] private GameObject[] targetObjects;
        public AudioSource buttonSound;

        public override void Interact()
        {
            int targetLen = targetObjects.Length;
            nthState = (nthState + 1) % targetLen;
            for (int i = 0; i < targetLen; i++)
            {
                bool isActive = i == nthState;
                buttonTexts[i].enabled = isActive;
                targetObjects[i].SetActive(isActive);
            }
            if (buttonSound) buttonSound.PlayOneShot(buttonSound.clip);
        }

        public void StopAnimator()
        {
            Animator self = this.GetComponent<Animator>();
            if (!self) return;
            self.enabled = false;
        }
    }
}
