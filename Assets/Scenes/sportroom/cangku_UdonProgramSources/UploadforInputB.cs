
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UploadforInputB : UdonSharpBehaviour
{
    public Text forglobal;
    public TextMeshProUGUI forcustom;
    public TextMeshProUGUI whiteboardshow;
    [UdonSynced] private string saved;
    public override void Interact()
    {
        if(!usualuseclass.IsSetOwn(gameObject))return;
        saved = forcustom.text;
        saved += $"\nBy:{Networking.LocalPlayer.displayName}";
        RequestSerialization();
        forglobal.text = saved;
        whiteboardshow.text = saved;
    }
    public override void OnDeserialization()
    {
        forglobal.text = saved;
        whiteboardshow.text = saved;
    }
}
