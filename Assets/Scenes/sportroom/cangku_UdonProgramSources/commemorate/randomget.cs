
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using static usualuseclass;

public class randomget : UdonSharpBehaviour
{
    public GameObject boxCollider; // 需要设置为包含玩家的BoxCollider
    public GameObject randomcanvas;//text的上级目录，用于控制显隐
    public Text displayText;  // 需要设置为显示文本的TextMesh组件
    [UdonSynced]private string[] playerslist; // 用于存储BoxCollider内的玩家对象
    [UdonSynced]private bool isTextActive = true;

    void Start()
    {
        randomcanvas.SetActive(isTextActive);
    }
    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject); // 设置对象所有权
        isTextActive = !isTextActive;
        if (isTextActive)GetPlayers();// 获取PlayerList，是当前在范围内的玩家和设为空，并乱序
        RequestSerialization();// 同步状态给所有客户端
        randomcanvas.SetActive(isTextActive);
        if (isTextActive)DisplayPlayer();
    }
    public override void OnDeserialization()// 当状态同步时更新物体状态
    {
        randomcanvas.SetActive(isTextActive);
        if(isTextActive)DisplayPlayer();
    }
    private void GetPlayers()
    {
        // 获取所有玩家
        VRCPlayerApi[] players = new VRCPlayerApi[VRCPlayerApi.GetPlayerCount()];
        VRCPlayerApi.GetPlayers(players);

        // 筛选在BoxCollider内的玩家
        string[] playersInBox = new string[players.Length];
        int count = 0;
        foreach (VRCPlayerApi player in players)
        {
            // 检查玩家是否在BoxCollider范围内
            if (IsPlayerInBox(player))
            {
                playersInBox[count] = player.displayName;//将api内容的Name变成string输出          
                count++;
            }
        }
        if (count > 0)
        {
            // 调整数组大小以匹配实际玩家数量
            playerslist = new string[count];
            System.Array.Copy(playersInBox, playerslist, count);
            // 打乱玩家顺序
            SetRandomString(ref playerslist);
        }
        else playerslist = new string[0];

    }
    private void DisplayPlayer()
    {
        displayText.text = "当前随机序列为：\n";
        if (playerslist.Length >= 1)
        {
            for (int a = 0; a < playerslist.Length; a++)
            {
                string forplayername = playerslist[a];
                displayText.text += $"{a + 1}.{forplayername}\n";
            }   
        }
    }
    private bool IsPlayerInBox(VRCPlayerApi player)
    {
        // 获取玩家的位置
        Vector3 playerPosition = player.GetPosition();

        // 检查玩家是否在BoxCollider范围内
        Vector3 boxCenter = boxCollider.transform.position;
        Vector3 boxSize = boxCollider.transform.lossyScale;

        // 调整BoxCollider的范围检查
        Vector3 halfSize = boxSize * 0.5f;
        Vector3 min = boxCenter - halfSize;
        Vector3 max = boxCenter + halfSize;

        return playerPosition.x >= min.x && playerPosition.x <= max.x &&
               playerPosition.y >= min.y && playerPosition.y <= max.y &&
               playerPosition.z >= min.z && playerPosition.z <= max.z;
    }
}

