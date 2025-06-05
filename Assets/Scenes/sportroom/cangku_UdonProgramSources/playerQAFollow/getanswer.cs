
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class getanswer : UdonSharpBehaviour
{
    //这个方法里面我们要做什么？
    //按
    public GameObject GOA;//redbutton
    public GameObject GOB;//resetall
    public TextMeshProUGUI displayname;//抢答者显示
    private VRCPlayerApi player;
    public UdonBehaviour followplayerPQA;//跟随脚本
    [UdonSynced] private int playerid;
    [UdonSynced] bool isBlue = false;//是否是GOB进行按钮，也是蓝色按钮
    [UdonSynced] bool isButton = true;//是否是按钮，非为个人面板的图标操作
    public GameObject GORCB;//右侧check图标显示
    public GameObject GOLCB;//左侧check图标显示
    public GameObject AllGO;//全局getanswer显隐
    [UdonSynced] private bool isLeft = true;//左右图标显示的状态
    private void Start()
    {
        GORCB.SetActive(true);
        GOLCB.SetActive(false);
        AllGO.SetActive(false);
        GOA.SetActive(true);
        GOB.SetActive(true);
        player = null;
        displayname.text = "已重置抢答者"; //重置显示名称
        followplayerPQA.SendCustomEvent("Close"); //关闭跟随脚本
    }
    public void Redbutton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        playerid = Networking.LocalPlayer.playerId;
        isBlue = false;
        isButton = true;
        RequestSerialization();
        ODL();
    }
    public void Resetall()//重置所有
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        isBlue = true;
        isButton = true;
        RequestSerialization();
        ODL();
    }
    public void SetGOleftbutton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        isLeft = true;
        isButton = false;
        RequestSerialization();
        ODL();
    }
    public void SetGOrightbutton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        isLeft = false;
        isButton = false;
        RequestSerialization();
        ODL();
    }
    public override void OnDeserialization()
    {
        ODL();
    }
    private void ODL()
    {
        if (isButton)
        {
            if (isBlue)
            {
                GOA.SetActive(true);
                GOB.SetActive(true);
                player = null;
                displayname.text = "已重置抢答者"; //重置显示名称
                followplayerPQA.SendCustomEvent("Close"); //关闭跟随脚本
                                                          //重置走到这边
            }
            else
            {
                player = VRCPlayerApi.GetPlayerById(playerid);
                //找到要跟随的playerApi
                GOA.SetActive(false);
                displayname.text = player.displayName; //显示抢答者名称
                followplayerPQA.SendCustomEvent("Setplayer"); //设置跟随脚本
            }
        }
        else
        {
            GOA.SetActive(true);
            GOB.SetActive(true);
            player = null;
            displayname.text = "已重置抢答者"; //重置显示名称
            followplayerPQA.SendCustomEvent("Close");
            if (isLeft)
            {
                GORCB.SetActive(false);
                GOLCB.SetActive(true);
                AllGO.SetActive(true);
            }
            else
            {
                GORCB.SetActive(true);
                GOLCB.SetActive(false);
                AllGO.SetActive(false);
            }
        }
    }

}
