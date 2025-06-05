using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC_StarterKit.SampleAssets.USharp.Gimmick
{
    public class SyncToggleCamera : UdonSharpBehaviour
    {
        [SerializeField] private Camera targetCamera;
        public AudioSource audioSource;
        public AudioClip startSound;
        public AudioClip stopSound;
        
        private bool _cameraWorking = false;
        
        public override void OnPickupUseDown()
        {
            if (audioSource)
            {
                if (_cameraWorking)
                {
                    if (stopSound) audioSource.PlayOneShot(stopSound);
                }
                else
                {
                    if (startSound) audioSource.PlayOneShot(startSound);
                }
            }
            SendCustomNetworkEvent(NetworkEventTarget.All, _cameraWorking ? nameof(CameraStop) : nameof(CameraStart));
        }

        public void CameraStart()
        {
            _cameraWorking = true;
            targetCamera.enabled = true;
        }
        public void CameraStop()
        {
            _cameraWorking = false;
            targetCamera.enabled = false;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (!_cameraWorking) return;
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
            
            // 新しいplayerがjoinした場合に既にカメラが起動していたらカメラを起動させる
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(CameraStart));
        }
    }
}