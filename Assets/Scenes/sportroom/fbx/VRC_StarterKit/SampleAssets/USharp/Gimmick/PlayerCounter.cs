using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Gimmick
{
    public class PlayerCounter : UdonSharpBehaviour
    {
        [SerializeField] private Text display;

        private const float OnPlayerLeftDelay = 0.5f;
        
        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            UpdatePlayerCount();
        }

        public override void OnPlayerLeft(VRCPlayerApi player)
        {
            UpdatePlayerCount();
            // OnPlayerLeft時点ではPlayerCountがまだ更新されていないのでdelayをかける
            SendCustomEventDelayedSeconds(nameof(UpdatePlayerCount), OnPlayerLeftDelay);
        }

        public void UpdatePlayerCount()
        {
            var count = VRCPlayerApi.GetPlayerCount();
            string countText = $"{count:00}";
            if (count < 0 || 99 < count) countText = "";
            display.text = countText;
        }
    }
}