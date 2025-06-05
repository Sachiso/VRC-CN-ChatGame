
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class exitplayerundisplay : UdonSharpBehaviour
{
    public UdonBehaviour syncedudon;
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if(player.isLocal)
        syncedudon.SendCustomEvent("setcancel");
    }
}
