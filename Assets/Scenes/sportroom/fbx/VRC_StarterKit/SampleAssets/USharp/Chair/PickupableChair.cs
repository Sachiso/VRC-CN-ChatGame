using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC_StarterKit.SampleAssets.USharp.Chair
{
    public class PickupableChair : UdonSharpBehaviour
    {
        [SerializeField] private PickupableChairStation station;
        [SerializeField] private VRC_Pickup pickupSelf;
        [SerializeField] private Collider pickupCollider;

        private bool _isPickupSelf = false;
        
        public override void OnPickup()
        {
            _isPickupSelf = true;
            station.TriggerChairOnPickup();
        }

        public override void OnDrop()
        {
            _isPickupSelf = false;
            station.TriggerChairOnDrop();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (_isPickupSelf) station.TriggerChairPickupHeld();
        }

        public void TriggerStationSeated() 
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(StationSeated));
        }

        public void StationSeated()
        {
            if (pickupCollider) pickupCollider.enabled = false;
        }
        
        public void StationOnEntered(VRCPlayerApi player)
        {
            if (pickupCollider) pickupCollider.enabled = false;
            Drop();
        }

        public void StationOnExited(VRCPlayerApi player)
        {
            if (pickupCollider) pickupCollider.enabled = true;
        }

        public void Drop()
        {
            if (_isPickupSelf) pickupSelf.Drop(Networking.LocalPlayer);
        }

    }
}
