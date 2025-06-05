
using System;
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Serialization.OdinSerializer.Utilities;

public class playerlist : UdonSharpBehaviour
{
    [UdonSynced] private int[] playernublist = new int[0];//存储序号
    [UdonSynced] private int[] playerscorelist = new int[0];//存储分数
    [UdonSynced] private string[] playernamelist = new string[0];//存储玩家名
    [UdonSynced] private string ForNubShow = "";
    [UdonSynced] private string ForScoreShow = "";
    public TextMeshProUGUI[] PNSNS;
    private void Start(){ ShowTMP(); }
    private void Setownshipplayer() { Networking.SetOwner(Networking.LocalPlayer,gameObject); }
    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        for (int i = 0; i < playernamelist.Length; i++)
        {
            if (playernamelist[i] == player.displayName)
            {
                playernublist[i] = 0;
                playerscorelist[i] = -1;
                break;
            }
        }
        Setnub();
        SetScore();
        RequestSerialization();
        ShowTMP();
    }
    public void resetall()
    {
        playernublist=new int[0];
        playerscorelist=new int[0];
        playernamelist = new string[0];
        ForNubShow = ForScoreShow = "";
        RequestSerialization();
        ShowTMP();
    }
    public void SetnubRandom()
    {
        Setownshipplayer();
        int setlength=0;//我需要的长度，又或者说拥有序列的数量
        for(int i=0;i<playerscorelist.Length;)
        {
            if (playernublist[i] == 0) break;
            setlength = ++i;
        }
        if (setlength <=1) return;//没有多位玩家随机个什么东西？
        int[] shuffledNub = new int[setlength];
        System.Array.Copy(playernublist, shuffledNub, setlength);// 复制序列数组到临时数组
        // 使用Fisher-Yates洗牌算法来打乱顺序
        for (int i = setlength - 1; i > 0; i--)
        {
            
            // 生成一个0到i之间的随机数
            int j = UnityEngine.Random.Range(0, i + 1);
            // 交换当前元素和随机位置的元素
            int temp = shuffledNub[i];
            shuffledNub[i] = shuffledNub[j];
            shuffledNub[j] = temp;
        }
        // 将打乱后的数组赋值回原始数组
        System.Array.Copy(shuffledNub, playernublist, setlength);
        Setnub();
        RequestSerialization();
        ShowTMP();
    }
    public void Scoreplus()
    {
        Setownshipplayer();
        string LPname = Networking.LocalPlayer.displayName;
        for (int i = 0; i < playernamelist.Length; i++)
        {
            if (playernamelist[i] == LPname)
            {
                if (playerscorelist[i]<98)
                playerscorelist[i]++;
                break;
            }
        }
        SetScore();
        RequestSerialization();
        ShowTMP();
    }
    public void Scoreminus()
    {
        Setownshipplayer();
        string LPname = Networking.LocalPlayer.displayName;
        for (int i = 0; i < playernamelist.Length; i++)
        {
            if (playernamelist[i] == LPname)
            {
                if (playerscorelist[i] > 0)
                    playerscorelist[i]--;
                break;
            }
        }
        SetScore();
        RequestSerialization();
        ShowTMP();

    }
    public void Joingame()
    {
        Setownshipplayer();
        int isme = -1;
        int formin = -1;//用于寻找你在哪儿加入，你是第几个
        string LPname = Networking.LocalPlayer.displayName;//创建一个自己的名字方便调用
        for (int i = 0; i < playernamelist.Length; i++)//用来找你自己，并且确定空数
        {
            if (playernamelist[i] == LPname)//首先找到自己
                if (playernublist[i] != 0) return;//如果不是0那也别继续跑了，你在游戏里。
                else isme = i;//记录你所在的数组位置
            if(playernublist[i]==0) formin++;//增加计数，确定给你的最低序列（一共有10个人，包括自己有3个0？
        }//到这里我们获得了两个数，isme和formin
        if (isme == -1) {  Setnew(); isme = playerscorelist.Length-1;formin++; }//这代表你没找到自己，我们需要给你添加新的数据，isme也是你的位置
        int getlength = playerscorelist.Length;
        playernamelist[isme] = LPname;
        playernublist[isme] = getlength-formin;
        playerscorelist[isme] = 0;
        Setnub();
        SetScore();
        RequestSerialization();
        ShowTMP();
    }
    
    public void Exitresetme()
    {
        Setownshipplayer();
        int isme = -1;
        for (int i = playernamelist.Length-1; i>=0 ; i--)//找到自己
        {
            if (playernamelist[i] == Networking.LocalPlayer.displayName) //找到自己了
                if (playernublist[i] == 0) return;//既然自己本来就没加入，那就别继续跑了
                else { isme = i; break; }//既然找到了，那就记录一下不继续找了
        }
        playernublist[isme] = 0;//nub改为0
        playerscorelist[isme] = -1;
        Setnub();
        SetScore();
        RequestSerialization();
        ShowTMP();
    }
    public override void OnDeserialization()
    {
        ShowTMP();
    }
    private void ShowTMP()
    {
        PNSNS[0].text = PNSNS[2].text = ForNubShow;
        PNSNS[1].text = PNSNS[3].text = ForScoreShow;
    }
    private void Setnub()
    {
        for (int i = 0;i< playernamelist.Length; i++)//排序方法组
        {
            for (int j = 0; j < playernamelist.Length - 1 - i; j++)
            {
                if ((playernublist[j]==0||playernublist[j]>playernublist[j + 1]) && playernublist[j+1]!=0)
                {//当前组的序列为零或大于后一个数，同时下一个数不为0时，向后排
                    int tmp = playernublist[j];
                    int tmp1 = playerscorelist[j];
                    string tmp2 = playernamelist[j];
                    playernublist[j] = playernublist[j + 1];
                    playerscorelist[j] = playerscorelist[j + 1];
                    playernamelist[j] = playernamelist[j + 1];
                    playernublist[j + 1] = tmp;
                    playerscorelist[j+1] = tmp1;
                    playernamelist[j+1] = tmp2;
                }
            }
        }//排序结束
        ForNubShow = "";//初始化显示文本
        for (int i = 0; i < playernamelist.Length; i++) //排完序我们就要开始重算序列号并排序了。
        {
            if (playernublist[i] == 0) break;//因为排过序了，当找到0的时候后续都不用找了
            if (playernublist[i] != i + 1)//既然它和我需要的数字不同，那就操作它
            {
                playernublist[i] = i + 1;//让它变成我需要的数字
            }
            ForNubShow += $"{playernublist[i]} <pos=10%>[{playernamelist[i]}]</pos>\n";//设置NubShow
        }
    }
    private void SetScore()
    {
        for (int i = 0; i < playernamelist.Length; i++)//排序方法组
        {
            for (int j = 0; j < playernamelist.Length - 1 - i; j++)
            {
                if ( playerscorelist[j] < playerscorelist[j + 1])
                {//当前数组的分数
                    int tmp = playernublist[j];
                    int tmp1 = playerscorelist[j];
                    string tmp2 = playernamelist[j];
                    playernublist[j] = playernublist[j + 1];
                    playerscorelist[j] = playerscorelist[j + 1];
                    playernamelist[j] = playernamelist[j + 1];
                    playernublist[j + 1] = tmp;
                    playerscorelist[j + 1] = tmp1;
                    playernamelist[j + 1] = tmp2;
                }
            }
        }//排序结束
        ForScoreShow = "";//初始化显示文本
        for (int i = 0; i < playernamelist.Length; i++) //排完序我们就要开始重算序列号并排序了。
        {
            if (playerscorelist[i] == -1) break;//如果它是-1那之后的排序也就没必要继续了。
            ForScoreShow += $"{playerscorelist[i]} <pos=10%>[{playernamelist[i]}]</pos>\n";
        }
    }

    private void Setnew()
    {
        int[] newint1 = new int[playernublist.Length + 1];
        int[] newint2 = new int[playerscorelist.Length + 1];
        string[] newstring= new string[playernamelist.Length+1];
        System.Array.Copy(playernamelist, newstring, playernamelist.Length);
        System.Array.Copy(playernublist, newint1, playernamelist.Length);
        System.Array.Copy(playerscorelist, newint2, playernamelist.Length);
        playernamelist =newstring;
        playernublist = newint1;
        playerscorelist = newint2;
    }
}
