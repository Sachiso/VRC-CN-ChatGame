
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

public class pickuplock : UdonSharpBehaviour
{
    [SerializeField] private VRCPickup myPickup;  // 在 Inspector 里把目标物体的 VRCPickup 拖进来
    [UdonSynced] bool setunblock = true;
    private void Start()
    {
        myPickup = (VRCPickup)GetComponentInChildren(typeof(VRCPickup));
        myPickup.pickupable = setunblock;
    }
    public override void Interact()
    {
        setunblock = !setunblock;
        RequestSerialization();
        myPickup.pickupable = setunblock;
    }
    public override void OnDeserialization()
    {
        myPickup.pickupable = setunblock;
    }
}
