using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace VRC_StarterKit.SampleAssets.USharp.Chair
{
    public class PickupableChairStation : UdonSharpBehaviour
    {
        [SerializeField] private PickupableChair chair;
        [SerializeField] private VRCStation stationSelf;
        [SerializeField] private Collider stationCollider;

        private bool _isInStationSelf = false;
        
        public override void Interact()
        {
            Networking.LocalPlayer.UseAttachedStation();
        }

        public override void OnStationEntered(VRCPlayerApi player)
        {
            if (player.isLocal) _isInStationSelf = true;
            chair.StationOnEntered(player);
        }

        public override void OnStationExited(VRCPlayerApi player)
        {
            if (player.isLocal) _isInStationSelf = false;
            chair.StationOnExited(player);
        }
        
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (_isInStationSelf) chair.TriggerStationSeated();
        }

        public void TriggerChairPickupHeld()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ChairPickupHeld));
        }
        public void TriggerChairOnPickup()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ChairOnPickup));
        }
        public void TriggerChairOnDrop()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(ChairOnDrop));
        }

        public void ChairPickupHeld()
        {
            stationCollider.enabled = false;
        }
        
        public void ChairOnPickup()
        {
            stationCollider.enabled = false;
            ExitStation();
        }

        public void ChairOnDrop()
        {
            stationCollider.enabled = true;
        }

        public void ExitStation()
        {
            if (_isInStationSelf)
            {
                stationSelf.ExitStation(Networking.LocalPlayer);
            }
        }
    }
}
