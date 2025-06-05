using UdonSharp;
using UnityEngine;
using VRC.Udon;

namespace VRC_StarterKit.SampleAssets.USharp.Pickup
{
    public class ObjectRespawner : UdonSharpBehaviour
    {
        [SerializeField] private GameObject respawnTargetBase;
        public AudioSource sound;

        public override void Interact()
        {
            if (sound) sound.PlayOneShot(sound.clip);
            Animator[] anims = respawnTargetBase.GetComponentsInChildren<Animator>();
            foreach (Animator target in anims)
            {
                if (target.runtimeAnimatorController.name == "TriggerRespawn")
                {
                    target.enabled = true;
                }
            }
        }
    }
}