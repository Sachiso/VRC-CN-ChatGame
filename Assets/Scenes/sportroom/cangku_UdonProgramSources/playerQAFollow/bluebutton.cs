
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class bluebutton : UdonSharpBehaviour
{
    public UdonBehaviour udonBehaviour;
    public override void Interact()
    {
        if(!usualuseclass.IsSetOwn(gameObject))return;
        udonBehaviour.SendCustomEvent("Resetall");
    }
}
