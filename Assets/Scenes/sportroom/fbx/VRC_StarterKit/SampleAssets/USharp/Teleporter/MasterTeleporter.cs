using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Teleporter
{
    public class MasterTeleporter : UdonSharpBehaviour
    {
        public AudioSource sound;
        public VRC_SceneDescriptor.SpawnOrientation spawnOrientation;
        public bool lerpOnRemote;
        public Vector3 teleportOffset = new Vector3(0f, 1f, -1f);
        
        public override void Interact()
        {
            if (sound) sound.PlayOneShot(sound.clip);
            VRCPlayerApi master = Networking.GetOwner(gameObject); // Object owner is Master if not modified.
            if (null == master) return;

            Vector3 teleportPos = master.GetPosition();
            Quaternion teleportRot = master.GetRotation();
            teleportPos += teleportRot * teleportOffset;
            Networking.LocalPlayer.TeleportTo(teleportPos, teleportRot, spawnOrientation, lerpOnRemote);
        }
    }
}
