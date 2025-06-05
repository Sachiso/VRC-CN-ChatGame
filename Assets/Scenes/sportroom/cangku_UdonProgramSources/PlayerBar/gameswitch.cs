
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
    private void Start()
    {
        foreach (GameObject go in gameObjects) go.SetActive(false);//初始化为关闭状态
    }
    //开关函数的主要调用
    private void SetObjectActive(int set, bool setB)
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        setGOint = set;//被控制的物体组索引
        forSw = setB;//是否开启
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
    }
    private VRCPlayerApi GetOwn()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        return Networking.LocalPlayer;
    }
    //下面是每个按钮的开关函数
    public void setfor0() { SetObjectActive(0, true); }
    public void unsetfor0() { SetObjectActive(0, false); }
    public void setfor1() { SetObjectActive(1, true); }
    public void unsetfor1() { SetObjectActive(1, false); }
    public void setfor2() { SetObjectActive(2, true); }
    public void unsetfor2() { SetObjectActive(2, false); }
    public void setfor3() { SetObjectActive(3, true); }
    public void unsetfor3() { SetObjectActive(3, false); }
    public void setfor4() { SetObjectActive(4, true); }
    public void unsetfor4() { SetObjectActive(4, false); }
    public void setfor5() { SetObjectActive(5, true); }
    public void unsetfor5() { SetObjectActive(5, false); }
    public void setfor6() { SetObjectActive(6, true); }
    public void unsetfor6() { SetObjectActive(6, false); }
    public void setfor7() { SetObjectActive(7, true); }
    public void unsetfor7() { SetObjectActive(7, false); }
    public void setfor8() { SetObjectActive(8, true); }
    public void unsetfor8() { SetObjectActive(8, false); }
    public void setfor9() { SetObjectActive(9, true); }
    public void unsetfor9() { SetObjectActive(9, false); }
}
