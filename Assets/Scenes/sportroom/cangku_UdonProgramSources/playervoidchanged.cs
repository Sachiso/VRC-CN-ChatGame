using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

enum whoami { me,xe,other }//用于标记当前函数的执行者
public class playervoidchanged : UdonSharpBehaviour
{
    public UdonBehaviour VCUdon;
    public Text saved;
    public float InMefar = 150;//在本地函数里设置其他人听到的我的和在相同范围内其他人的声音范围
    public float MeXefar = 150;//在被调用函数里设置我听到的其他人的声音范围
    public float XeMefar = 2;//在被调用函数里设置我的声音范围
    [UdonSynced] private string setPN;//用于在同步时传输玩家名称
    [UdonSynced] private whoami isme;//用于标记当前函数的执行者
    private VRCPlayerApi[] players=new VRCPlayerApi[0];//获取所有玩家
    private float otherplayerfar = 25;

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)//当玩家进入时触发
    {
        if (Networking.LocalPlayer != player) { return; }//我需要从进入玩家开始操作
        Networking.SetOwner(Networking.LocalPlayer, gameObject);//设置所有权
        setPN = player.displayName;//获取进入玩家的名称
        Setvoice();//在本地函数里设置其他人听到的我的声音范围
        isme = whoami.me;//标记当前函数的执行者
        RequestSerialization();//同步
        VCUdon.SendCustomEvent("SetOvoice");//执行被调用函数设置我听到的其他人的声音范围
    }
    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer != player) return;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);//设置所有权
        setPN = player.displayName;
        Resetvoice();
        isme = whoami.other;
        RequestSerialization();
    }
    private void Setvoice()
    {
        setnewplayers();//获取所有玩家
        foreach (VRCPlayerApi player in players)if(IsThisPlayerInside(player)) player.SetVoiceDistanceFar(InMefar);
    }
    public void SetOvoice()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);//设置所有权
        //在被调用函数里设置我听到的其他人的声音范围
        setnewplayers();
        foreach (VRCPlayerApi player in players) if (IsThisPlayerInside(player)) player.SetVoiceDistanceFar(MeXefar);
        isme = whoami.xe;
        RequestSerialization();
    }
    private void Resetvoice()
    {
        setnewplayers();
        foreach (VRCPlayerApi player in players)player.SetVoiceDistanceFar(otherplayerfar);
    }
    public override void OnDeserialization()
    {
        //现在我们获取到了需要操作的API
        if (isme == whoami.me)//在自己的范围内时在当前函数同步中
        {
            if (!IsThisPlayerInside(Networking.LocalPlayer))//仅操作在范围内的玩家
            {
                saved.text = setPN;//如果不在操作范围内，那就在外部给一个玩家名称方便调用
                return;
            }
            setnewplayers();
            VRCPlayerApi setPNApi = null;//获取需要操作的对象
            foreach (VRCPlayerApi player in players)
                if (player.displayName == setPN) { setPNApi = player; break; }
            setPNApi.SetVoiceDistanceFar(InMefar);
            
        }
        else if (isme == whoami.xe)//在被调用函数同步中
        {
            if (!IsThisPlayerInside(Networking.LocalPlayer)) return;
            setnewplayers();
            VRCPlayerApi setPNApi = null;//获取需要操作的对象
            setPN = saved.text;//用于在SetOvoice方法的同步里同步该数据
            foreach (VRCPlayerApi player in players)
                if (player.displayName == setPN) { setPNApi = player; break; }
            setPNApi.SetVoiceDistanceFar(XeMefar);
        }
        else
        {
            setnewplayers();
            foreach (VRCPlayerApi player in players) player.SetVoiceDistanceFar(otherplayerfar);
        }
    }

    private void setnewplayers()//获取所有玩家到players
    {
        int playerCount = VRCPlayerApi.GetPlayerCount();
        players = new VRCPlayerApi[playerCount];
        VRCPlayerApi.GetPlayers(players);
    }
    private bool IsThisPlayerInside(VRCPlayerApi player)//判断玩家是否在范围内
    {
        return GetComponent<Collider>().bounds.Contains(player.GetPosition());
    }
}
