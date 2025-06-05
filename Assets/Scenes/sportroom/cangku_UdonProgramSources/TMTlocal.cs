using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using static usualuseclass;

public class TMTlocal : UdonSharpBehaviour
{
    public Text SavedText; // 挂载的Text组件，包含所有问题
    public Text localdisplayText;  // 用于显示随机选择的问题的Text组件
    private int localshowcount;
    [UdonSynced] private string[] SavedString=new string[0]; // 存储所有问题的数组
    [UdonSynced] private int showcount = 0;//存储获取的问题序列
    
    public override void Interact() // 随机选择一个问题并显示
    {
        useme();
    }
    public void useme()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject); // 设置对象所有权
        if (SavedString.Length == 0)
        {
            LoadTextToString(SavedText, ref SavedString);//从Text读取到questions
            SetRandomString(ref SavedString);//打乱questions数组
            showcount = SavedString.Length;
            localshowcount = SavedString.Length - 1;
        }
        if (localdisplayText.text != "") return;//不让拥有词条的人获取词条。
        showcount--;//设置序列号
        if (showcount < 0)
        {
            LoadTextToString(SavedText, ref SavedString);//从Text读取到questions
            SetRandomString(ref SavedString);//打乱questions数组
            showcount = SavedString.Length - 1;
        }
        RequestSerialization();// 同步状态给所有客户端 
        localshowcount = showcount;
        localdisplayText.text = $"{SavedString[localshowcount]}\n{localshowcount}";
    }
}