
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class gameswitch : UdonSharpBehaviour
{
    //这个脚本主要用来控制游戏的显示
    //每个游戏两个开关：1、开启时关闭其他；2、关闭时只关闭自己；3.同步操作
    public GameObject[] gameObjects;//被控制的物体组
    [UdonSynced] int setGOint = 0;//被控制的物体组索引
    [UdonSynced] bool forSw = false;//是否开启
    public TextMeshProUGUI ForGameTips;
    [UdonSynced] string ForTipsText= "当前游戏为：null\n这里是提示";
    private void Start()
    {
        if (!Networking.IsOwner(gameObject)) return;
        setGOint = 0;//被控制的物体组索引
        forSw = false;//是否开启
        RequestSerialization();//请求同步
        SetGOSW();
    }
    //开关函数的主要调用
    private void SetObjectActive(int set, bool setB,string tips)
    {
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        setGOint = set;//被控制的物体组索引
        forSw = setB;//是否开启
        ForTipsText = tips;
        RequestSerialization();//请求同步
        SetGOSW();
    }
    public override void OnDeserialization()
    {
        SetGOSW(); 
    }
    private void SetGOSW()
    {
        foreach (GameObject go in gameObjects) go.SetActive(false);//关闭所有物体
        gameObjects[setGOint].SetActive(forSw);//开启指定物体
        ForGameTips.text = ForTipsText;
    }
    //下面是每个按钮的开关函数

    public void unsetall() { SetObjectActive(0, false, "当前游戏为：null\n这里是提示"); }
    public void setfor0() { SetObjectActive(0, true, "当前游戏为：NG词\n点击场中红球参与,再次点击显现\n白板单行文本有集成显示"); }
    public void setfor1() { SetObjectActive(1, true, "当前游戏为：灯谜龟汤\n请移步个人面板操作\n白板双文本有显示"); }
    public void setfor2() { SetObjectActive(2, true, "当前游戏为：很久以前故事会\n在桌前参与，手牌可拾取\n默认1号位玩家开始故事"); }
    public void setfor3() { SetObjectActive(3, true, "当前游戏为：干瞪眼\n在桌前参与，无法在单局游戏进行时加入\n尽量不要重置，因为有计分方便玩家查看分数"); }
    public void setfor4() { SetObjectActive(4, true, "当前游戏为：真心话大冒险\n在桌前参与，惩罚玩家请自主选择内容\n可以不消牌，可以自定义2张手牌的意义\n如：无条件消牌、无法填充牌库、跳过惩罚等"); }
    public void setfor5() { SetObjectActive(5, true, "这里是提示"); }
    public void setfor6() { SetObjectActive(6, true, "这里是提示"); }
    public void setfor7() { SetObjectActive(7, true, "这里是提示"); }
    public void setfor8() { SetObjectActive(8, true, "这里是提示"); }
    public void setfor9() { SetObjectActive(9, true, "这里是提示"); }
}
