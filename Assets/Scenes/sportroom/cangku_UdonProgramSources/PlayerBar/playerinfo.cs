
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class playerinfo : UdonSharpBehaviour
{
    public TextMeshProUGUI forstring;
    [UdonSynced] private string saved;
    [UdonSynced] private long datetime;
    private void Start()
    {
        
        if (Networking.IsOwner(gameObject))
        {
            saved = "";
            RequestSerialization();
        }
        SetUpload();
    }
    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        Writein(player,true);
        RequestSerialization();
        SetUpload();
    }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        Writein(player,false);
        RequestSerialization();
        SetUpload();
    }
    public override void OnDeserialization()
    {
        SetUpload();
    }
    private void SetUpload()
    {
        forstring.text = saved;
    }
    private void Writein(VRCPlayerApi player,bool TF)
    {
        datetime = DateTime.UtcNow.Ticks;
        string datetimes = new DateTime(datetime).ToLocalTime().ToString("HH:mm:ss"); 
        string prefix = TF ? "<color=#8BC34A>[join]</color>     " : "<color=#F44336>[leave]</color> ";
        string tag = player.IsUserInVR() ? "[VR]" : "[PC]";
        saved = $"{prefix}[{datetimes}]    [{player.displayName}]<pos=92%>{tag}</pos>\n"+saved;
    }
}
