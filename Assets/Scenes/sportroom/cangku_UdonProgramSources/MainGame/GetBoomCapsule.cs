
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class GetBoomCapsule : UdonSharpBehaviour
{
    public GetBoom getBoom;
    public GameObject Sphere;
    private VRCPlayerApi local;
    public void OnTriggerEnter(Collider other)
    {
        if (getBoom.isover) return;
        if(other.gameObject == Sphere)
        getBoom.Setname(local.displayName,true);
    }
    void Start()
    {
        local = Networking.LocalPlayer;
    }
    void Update()
    {
        if (local != null && local.IsValid())
        {
            // 获取玩家头部位置
            Vector3 targetPosition = local.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            targetPosition.y -= 0.8f;
            transform.position = targetPosition;
        }
    }
}
