using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Teleporter
{
    public class Teleporter : UdonSharpBehaviour
    {
        public GameObject teleportPos;
        public AudioSource sound;
        public VRC_SceneDescriptor.SpawnOrientation spawnOrientation;
        public bool lerpOnRemote;

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player != Networking.LocalPlayer) return;
            if (sound) sound.PlayOneShot(sound.clip);
            player.TeleportTo(teleportPos.transform.position, teleportPos.transform.rotation, spawnOrientation, lerpOnRemote);
        }
    }
}