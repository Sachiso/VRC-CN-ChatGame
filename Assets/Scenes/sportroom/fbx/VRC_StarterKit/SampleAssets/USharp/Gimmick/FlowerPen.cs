using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC_StarterKit.SampleAssets.USharp.Gimmick
{
    public class FlowerPen : UdonSharpBehaviour
    {
        [SerializeField] private TrailRenderer trailRenderer;
        
        private Vector3 _initialPos;
        private Quaternion _initialRot;
        
        void Start()
        {
            // Startが最初の位置同期よりも先に動くと仮定して初期位置を保存している。
            _initialPos = transform.position;
            _initialRot = transform.rotation;
        }

        public override void Interact()
        {
            
        }
        
        public override void OnPickupUseDown()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PenDrawStart));
        }

        public override void OnPickupUseUp()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PenDrawStop));
        }

        public void TriggerPenDrawClear()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PenDrawClear));
        }

        public void PenDrawStart()
        {
            trailRenderer.emitting = true;
        }

        public void PenDrawStop()
        {
            trailRenderer.emitting = false;
        }

        public void PenDrawClear()
        {
            trailRenderer.Clear();
        }
        
        public void TriggerRespawn()
        {
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                Respawn();
            }
            else
            {
                SendCustomNetworkEvent(NetworkEventTarget.Owner, nameof(Respawn));
            }
        }
        
        public void Respawn()
        {
            if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
            transform.SetPositionAndRotation(_initialPos, _initialRot);
        }
        
        public void StopAnimator()
        {
            GetComponent<Animator>().enabled = false;
        }
    }
}
