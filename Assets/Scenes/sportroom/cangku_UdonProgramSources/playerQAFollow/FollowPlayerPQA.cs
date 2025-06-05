
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FollowPlayerPQA : UdonSharpBehaviour
{
    public TextMeshProUGUI displayname;//外部名字调用 
    private VRCPlayerApi player;
    public void Setplayer() 
    {
        int playerCount = VRCPlayerApi.GetPlayerCount();
        VRCPlayerApi[] players = new VRCPlayerApi[playerCount];
        VRCPlayerApi.GetPlayers(players);
        foreach (VRCPlayerApi p in players)
        {
            if (p.displayName == displayname.text)
            {
                player = p;
                break;
            }
        }
        gameObject.SetActive(true);
    }
    public void Close() 
    {
        player = null;
        gameObject.SetActive(false);
    }
    void Update()
    {
        
        if (player != null && player.IsValid())
        {
            // 获取玩家头部位置
            Vector3 targetPosition = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            //设定y轴高度要高0.3f
            targetPosition.y += 0.25f;
            transform.position = targetPosition;
        }
    }

}
