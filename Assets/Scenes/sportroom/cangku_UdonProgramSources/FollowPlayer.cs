
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using static usualuseclass;

public class FollowPlayer : UdonSharpBehaviour
{
    public Text savedplayername;
    private VRCPlayerApi player;
    void Start()
    {
        string[] savedPN = new string[2];
        LoadTextToString(savedplayername, ref savedPN);
        // 获取所有玩家
        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        // 筛选我需要的玩家的Api
        foreach (VRCPlayerApi letsselectVRCAP in players)
        {
            // 检查玩家是否与savedPN相同
            if (letsselectVRCAP.displayName == savedPN[1])
                player = letsselectVRCAP;
        }
    }
    void Update()
    {
        if (player != null && player.IsValid())
        {
            // 获取玩家头部位置
            Vector3 targetPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            //设定y轴高度要高0.3f
            targetPosition.y += 0.8f;
            transform.position= targetPosition;
        }
        else  Destroy(gameObject);
    }
}
