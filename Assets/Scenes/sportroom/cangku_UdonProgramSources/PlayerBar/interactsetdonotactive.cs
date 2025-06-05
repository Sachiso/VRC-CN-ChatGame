
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class interactsetdonotactive : UdonSharpBehaviour
{
    public UdonBehaviour setactive;
    public override void Interact()
    {
        setactive.SendCustomEvent("setcancel");
    }
}
