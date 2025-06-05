using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Common
{
    public class PlayerMods : UdonSharpBehaviour
    {
        // VRC_Player Mods
        [Header("Jump")]
        [SerializeField] [Tooltip("Jump power")] private float jumpImpulse = 3;
        [Header("Speed")]
        [SerializeField] private float runSpeed = 4;
        [SerializeField] private float walkSpeed = 2;
        [SerializeField] private float strafeSpeed = 2;
        [SerializeField] private float gravityStrength = 1;
        [Header("Locomotion")]
        [Tooltip("Use SDK2 locomotion")]
        [SerializeField] private bool useLegacyLocomotion = true;

        [Space]
        // VRC_Player Audio Override
        [Header("Voice Settings")]
        [Tooltip("Apply Voice Settings")]
        [SerializeField] private bool applyVoiceOptions;
        [SerializeField] private float voiceGain = 15;
        [SerializeField] private float voiceDistanceFar = 25;
        // VRC_Player Audio Override Advanced Options
        [Header("Voice Settings(advanced)")]
        [SerializeField] private bool applyVoiceAdvancedOptions;
        [SerializeField] private float voiceDistanceNear;
        [SerializeField] private float voiceVolumetricRadius;
        [SerializeField] private bool voiceLowpass = true;

        // VRC_Player Audio Override Avatar Audio Limits
        [Header("Audio Limits")]
        [Tooltip("Apply Audio Limits")]
        [SerializeField] private bool applyAvatarAudioLimits;
        [SerializeField] private float avatarAudioGain = 10;
        [SerializeField] private float avatarAudioFarRadius = 40;

        // VRC_Player Audio Override Avatar Audio Limits Advanced Options
        [Header("Audio Limits(advanced)")]
        [SerializeField] private bool applyAvatarAudioAdvancedOptions;
        [SerializeField] private float avatarAudioNearRadius = 40;
        [SerializeField] private float avatarAudioVolumetricRadius = 40;
        [SerializeField] private bool avatarAudioForceSpatial;
        [SerializeField] private bool avatarAudioCustomCurve = true;

        void Start()
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (null == player) return;

            // VRC_Player Mods
            player.SetJumpImpulse(jumpImpulse);
            player.SetRunSpeed(runSpeed);
            player.SetWalkSpeed(walkSpeed);
            player.SetStrafeSpeed(strafeSpeed);
            player.SetGravityStrength(gravityStrength);
            if (useLegacyLocomotion) player.UseLegacyLocomotion();
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (null == player || !player.IsValid() || player.isLocal) return;
            
            // VRC_Player Audio Override
            if (applyVoiceOptions)
            {
                player.SetVoiceGain(voiceGain);
                player.SetVoiceDistanceFar(voiceDistanceFar);

                // Advanced Options
                if (applyVoiceAdvancedOptions)
                {
                    player.SetVoiceDistanceNear(voiceDistanceNear);
                    player.SetVoiceVolumetricRadius(voiceVolumetricRadius);
                    player.SetVoiceLowpass(voiceLowpass);
                }
            }

            // VRC_Player Audio Override Avatar Audio Limits
            if (applyAvatarAudioLimits)
            {
                player.SetAvatarAudioGain(avatarAudioGain);
                player.SetAvatarAudioFarRadius(avatarAudioFarRadius);

                // VRC_Player Audio Override Avatar Audio Limits Advanced Options
                if (applyAvatarAudioAdvancedOptions)
                {
                    player.SetAvatarAudioForceSpatial(avatarAudioForceSpatial);
                    player.SetAvatarAudioNearRadius(avatarAudioNearRadius);
                    player.SetAvatarAudioVolumetricRadius(avatarAudioVolumetricRadius);
                    player.SetAvatarAudioCustomCurve(avatarAudioCustomCurve);
                }
            }
        }
    }
}
