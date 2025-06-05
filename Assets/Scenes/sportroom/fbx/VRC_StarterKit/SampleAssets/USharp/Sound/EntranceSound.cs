using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Sound
{
    public class EntranceSound : UdonSharpBehaviour
    {
        public AudioSource sound;

        private float _startTime;
        
        private const float StartingPeriodSec = 1f;
        
        private void Start()
        {
            _startTime = UnityEngine.Time.time;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            // 一定時間経過後のみ入場音を再生
            if ((UnityEngine.Time.time - _startTime) > StartingPeriodSec) {
                sound.PlayOneShot(sound.clip);
            }
        }
    }
}