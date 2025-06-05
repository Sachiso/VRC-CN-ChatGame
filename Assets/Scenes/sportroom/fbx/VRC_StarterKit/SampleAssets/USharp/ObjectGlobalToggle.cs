using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC_StarterKit.SampleAssets.USharp
{
    public class ObjectGlobalToggle : UdonSharpBehaviour
    {
        public GameObject target;
        public bool doEnable = true;
        public bool doDisable = true;

        private bool _currentState = false;
    
        // Respawn用
        private Vector3 _initialPos;
        private Quaternion _initialRot;
        
        void Start()
        {
            // Respawn用
            // Startが最初の位置同期よりも先に動くと仮定して初期位置を保存している。
            _initialPos = transform.position;
            _initialRot = transform.rotation;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(Networking.LocalPlayer, gameObject))
            {
                SendCustomNetworkEvent(NetworkEventTarget.All, _currentState ? nameof(TargetEnable) : nameof(TargetDisable));
            }
        }

        public override void OnPickupUseDown()
        {
            bool newState = !_currentState;
        
            if (newState && !doEnable || !newState && !doDisable) return;
        
            SendCustomNetworkEvent(NetworkEventTarget.All, newState ? nameof(TargetEnable) : nameof(TargetDisable));
        }

        public void TargetEnable()
        {
            if (doEnable && !target.activeSelf)
            {
                target.SetActive(true);
                _currentState = true;
            }
        }

        public void TargetDisable()
        {
            if (doDisable && target.activeSelf)
            {
                target.SetActive(false);
                _currentState = false;
            }
        }
    
        // 以下Respawn用
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
