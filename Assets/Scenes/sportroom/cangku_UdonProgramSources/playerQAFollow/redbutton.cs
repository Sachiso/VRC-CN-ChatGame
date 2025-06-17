
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class redbutton : UdonSharpBehaviour
{
    public UdonBehaviour udonBehaviour;
    public override void Interact()
    {
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        udonBehaviour.SendCustomEvent("Redbutton");
    }
   
}
