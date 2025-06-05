using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class AreaPlayerBiggestVoice : UdonSharpBehaviour
{
    public int voicenearfar;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(voicenearfar);
        player.SetVoiceDistanceNear(voicenearfar);
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(25);
        player.SetVoiceDistanceNear(2);
    }
}
