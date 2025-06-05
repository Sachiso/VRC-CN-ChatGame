
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using static usualuseclass;
using TMPro;
using UnityEditor;
/*
通过物体状态进行判断是否刷新题库？
这个按钮赋予玩家头顶NG词，同时自己不可见其他人可见，然后第二次按本地显示并变色，第三次按清除。
第三个按钮是控制玩家获得分数的，也就是排序那边？
逻辑出问题了，不能直接绑定到玩家头部，要用组件去持续追踪玩家，因为无法用parent绑定
那就是之后这边生成上可以是额外确定一个upload组件挂载上去，这个ownplayer也必须一直是同一个，也就是用name追踪玩家api，如果玩家api丢失便删除对象
 */
enum objectstate {none, create, update, clear, }//枚举，0为空的none，1为加载创建，2为刷新改色，3为清空删除
public class NGwordget : UdonSharpBehaviour
{
    public TextMeshProUGUI displayText;  // 用于显示提示词的Text组件
    public Text getText; // 挂载的Text组件，包含所有NG词
    public TextMeshProUGUI PBTMP;//用于在PlayerBar中同步displayText的文本
    public TextMeshProUGUI PBShowAllexceptme;//用于在PlayerBar中显示所有玩家的NG词
    public TextMeshProUGUI whiteboardshow;
    public GameObject textPrefab;//需要创建的文本预制体
    public GameObject forparent;
    //以下一个组不同步，所以重进时新对象不知道他们头顶的词,并且在每次他们重载时重新获取
    private GameObject[] objectlist = new GameObject[0];//物体列表不让同步
    [UdonSynced] private int forNGText=0;//存储获取词条序列，为0时重置
    [UdonSynced] private bool isnew;//是否为新玩家判定
    [UdonSynced] private string ownplayer;//所有者的name存储
    [UdonSynced] private int forlength;//判断ownplayer在哪一行
    [UdonSynced] private string[] playernamelist = new string[0];//玩家name列表
    [UdonSynced] private string[] NGText=new string[0]; // 存储所有NG词的数组
    [UdonSynced] private objectstate[] objectstates = new objectstate[0];//用于识别个体的状态，
    [UdonSynced] private int[] objectNGText = new int[0];//用于获取与玩家绑定的NG词序列，获取物体是赋予值
    //作为新对象加入游戏时重载所有对象物体，同步时不重载，只额外加入自己的。
    private void Start()
    {
        if (NGText.Length == 0)
        {
            LoadTextToString(getText, ref NGText);//加载Text到存储string
            SetRandomString(ref NGText);//随机化NGText
            forNGText = NGText.Length;//初始化NG词序列最大值
        }
    }
    public override void Interact()//点击事件
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject); // 设置对象所有权
        ownplayer = Networking.LocalPlayer.displayName;//获取所有者的名字
        isnew = true;//重置新玩家判定
        //判断自己在不在playernamelist内，也就是说判断自己是不是新玩家
        for (int count = 0; count < playernamelist.Length; count++)//遍历playernamelist
        {
            if (ownplayer == playernamelist[count])//所有者名字是否为名字列表当前count行的名字
            {
                isnew = false;//非新玩家
                forlength = count;//给玩家在列表中定位
                break;//跳出循环
            }
        }
        if (isnew)//是新玩家
        {
            forlength = playernamelist.Length;//定义新玩家在列表中的定位为最新行
            forNGText--;//NG序列设置
            Setnewlist();//设置新的除物品外的表
            if (forNGText < 0)//当题库中无题库时重置并重新随机
            {
                SetRandomString(ref NGText);//随机化NGText
                displayText.text = "当前题库为空，请其他人全部重新获取以防止显示错误";//增加提示词
                forNGText = NGText.Length-1;
            }//设定状态参数。
            objectNGText[forlength] = forNGText;//对应玩家问题序列存储为最新
            //从头开始遍历，从头去生成一遍所有玩家的object
        }
        else
        {
            //进行物体状态判定。
            switch (objectstates[forlength])
            {
                case objectstate.create:
                    {
                        if (objectlist.Length == 0) { objectstates[forlength] = objectstate.clear; break; }
                        if (objectlist[forlength] != null)
                        {
                            objectstates[forlength] = objectstate.update;
                            displayText.text = $"点击清空头顶标签\n你的词为:{objectlist[forlength].transform.GetComponentInChildren<Text>().text}";
                            displayText.color = new Color(0.8f,0f, 0f);
                        }
                        else objectstates[forlength] = objectstate.clear;
                    }
                    break;
                case objectstate.update:
                    {
                        if (objectlist.Length == 0) { objectstates[forlength] = objectstate.clear; break; }
                        objectstates[forlength] = objectstate.clear;
                        displayText.text = "点击获取个人NG词\n↓";
                        displayText.color =Color.white;
                    }
                    break;
                case objectstate.clear:
                    {
                        if (objectlist.Length == 0) { objectlist = new GameObject[objectstates.Length]; }
                        objectstates[forlength] = objectstate.create;
                        forNGText--;
                        if (forNGText < 0)//当题库中无题库时重置并重新随机
                        {
                            LoadTextToString(getText, ref NGText);
                            SetRandomString(ref NGText);
                            displayText.text = "当前题库为空，可选择提醒其他人全部重新获取以防止重复";
                            displayText.color = new Color(0.8f,0.8f,0f);
                            forNGText = NGText.Length;
                            forNGText--;
                        }
                        objectNGText[forlength] = forNGText;
                        displayText.text = "已获取个人NG词";
                        displayText.color = new Color(0f, 0.8f, 0f);
                    }
                    break;
            }
        }
        ShowToNew();
        PBTMP.text = displayText.text;//同步到PlayerBar
        PBTMP.color=displayText.color;
        RequestSerialization();//同步以上参数，随后操纵创建物体，并在OnDeserialization同步执行
        //是新玩家的话给自己添加到数组内并创建与你绑定的物体
        ShowObjectFromList();
    }
    public override void OnDeserialization()
    {
        ShowObjectFromList();
    }
 
    private void ShowObjectFromList()
    {
        GameObject[] objectlistsave = new GameObject[playernamelist.Length];
        System.Array.Copy(objectlist, objectlistsave, objectlist.Length);
        objectlist = objectlistsave;
        //查找物品处于什么状态，进行相应的操作
        switch (objectstates[forlength])
        {
            case objectstate.create://创建物体
                {
                    if (objectlist[forlength] == null)
                    {
                        objectlist[forlength] = CreateTextObject();//创建本地
                        if (playernamelist[forlength] ==Networking.LocalPlayer.displayName) objectlist[forlength].SetActive(false);
                        else objectlist[forlength].SetActive(true);
                    }
                }
                break;
            case objectstate.update:
                {
                    if (objectlist[forlength] != null)
                    {
                        objectlist[forlength].SetActive(true) ;
                        objectlist[forlength].GetComponentInChildren<Text>().color = Color.red;
                    }
                }
                break;
            case objectstate.clear:
                {
                    if (objectlist[forlength] != null)
                    {
                        Destroy(objectlist[forlength]);
                        objectlist[forlength] = null;
                    }
                }
                break;
        }
        PBShowAllexceptme.text = "";//清空文本
        //显示所有玩家的NG词
        for (int wheresme=0;wheresme<objectstates.Length;wheresme++)
        {
            if (playernamelist[wheresme] != Networking.LocalPlayer.displayName)
            {
                if (objectstates[wheresme] ==objectstate.create )
                {
                    PBShowAllexceptme.text += $"{playernamelist[wheresme]}的NG词为：{NGText[objectNGText[wheresme]]}\n";
                }
                else if (objectstates[wheresme] == objectstate.update)
                {
                    PBShowAllexceptme.text += $"{playernamelist[wheresme]}的NG词为：<color=#F44336>{NGText[objectNGText[wheresme]]}</color>\n";
                }
            }
            else {if(objectstates[wheresme]!=objectstate.clear) PBShowAllexceptme.text += "这是你本地玩家行，自己看上面吧。\n"; }
        }
        whiteboardshow.text=PBShowAllexceptme.text;
    }
    private void ShowToNew()
    {
        //用于给新玩家、掉线玩家加入时对object物体同步用，只要自身为创建状态便进入判定重新生成其他人列表
        if (isnew || objectstates[forlength] == objectstate.create)//是新玩家或是状态为获取
        {
            for (int i = 0; i < playernamelist.Length; i++)//遍历playernamelist
            {
                if (ownplayer != playernamelist[i])//跳过ownplayer的物体创建交给同步后执行
                {
                    switch (objectstates[i])//对objectstates的对应参数进行判定
                    {
                        case objectstate.create://创建状态
                            {
                                if (objectlist[i]==null)objectlist[i] = CreateTextObjectForOld(i, playernamelist[i]); 
                                objectlist[i].SetActive(true);
                            }
                            break;
                        case objectstate.update://第二次按按钮状态
                            {
                                if (objectlist[i] == null)
                                {
                                    objectlist[i] = CreateTextObjectForOld(i, playernamelist[i]);
                                    objectlist[i].GetComponentInChildren<Text>().color = Color.red;
                                }
                                objectlist[i].SetActive(true);
                            }
                            break;
                        case objectstate.clear: break;
                    }
                }
            }
        }
    }
    private void Setnewlist() 
    {
        //重载旧数组到长度+1的新数组
        string[] namesave = new string[forlength + 1];//定义一个新的id组
        objectstate[] statessave=new objectstate[namesave.Length];
        int[] NGTextsave=new int[namesave.Length];
        GameObject[] objectlistsave = new GameObject[namesave.Length];
        System.Array.Copy(playernamelist, namesave, forlength);//将旧ID组对应复制过来
        System.Array.Copy(objectstates, statessave, forlength);
        System.Array.Copy(objectNGText, NGTextsave, forlength);
        System.Array.Copy(objectlist, objectlistsave, forlength);
        playernamelist = namesave;//获取新的长度+1的列表
        objectstates = statessave;
        objectNGText = NGTextsave;
        objectlist= objectlistsave;
        playernamelist[forlength] = ownplayer;//玩家列表的最新列存储为当前按按钮的新玩家
        objectstates[forlength] = objectstate.create;//对应玩家物件状态表设置为创建
        objectNGText[forlength] = forNGText;
        displayText.text = "已获取个人NG词\n↓";
        displayText.color = new Color(0f, 0.8f, 0f);
    }
    GameObject CreateTextObject()
    {
        GameObject textObj = Instantiate(textPrefab);
        textObj.transform.parent = forparent.transform;
        Text textComponent = textObj.transform.GetComponentInChildren<Text>();
        // 设置文本内容
        textComponent.text = $"{NGText[forNGText]}\n{ownplayer}";
        return textObj;
    }
    //用另一种方法引用，为了初次载入
    GameObject CreateTextObjectForOld(int i,string ownplayer1) 
    {
        GameObject textObj = Instantiate(textPrefab);
        textObj.transform.parent = forparent.transform;
        Text textComponent = textObj.transform.GetComponentInChildren<Text>();
        textComponent.text = $"{NGText[objectNGText[i]]}\n{ownplayer1}";
        return textObj;
    }
}
