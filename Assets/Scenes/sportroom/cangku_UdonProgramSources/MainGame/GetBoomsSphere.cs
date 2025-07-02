
using UdonSharp;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using VRC.SDKBase;
using VRC.Udon;
using static VRC.Dynamics.CollisionShapes;

public class GetBoomsSphere : UdonSharpBehaviour
{
    public GetBoom getBoom;
    public override void OnPickup()
    {
        if (getBoom.isover) return;
        getBoom.Setname(Networking.LocalPlayer.displayName,false); //获得捡起物品玩家名
    }
    public override void OnPickupUseUp()
    {
        getBoom.SetStart();
    }
}
