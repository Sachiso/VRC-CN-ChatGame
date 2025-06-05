
using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class PlayerBarsTMT : UdonSharpBehaviour
{ 
    public UdonBehaviour[] udonget;
    public UdonBehaviour[] udonupload;
    public Text[] localtext;
    public Image[] buttonimages;
    public TextMeshProUGUI titletext;
    public TextMeshProUGUI whiteboardshow;
    public Text forlocal;
    public Text forglobal;
    private int SetType=0;
    private void ChangedFromButton(int ForSetType,string Title)
    {
        SetType = ForSetType;
        titletext.text=Title;
        forlocal.text=localtext[SetType].text;
        foreach(Image bt in buttonimages)bt.color= new Color(0.137f, 0.137f, 0.137f);//重置按钮颜色
        buttonimages[ForSetType].color = new Color(0.380f, 0.165f, 0.165f);//设置当前按钮颜色
    }
    public void Useudonget()
    {
        udonget[SetType].SendCustomEvent("useme");
        forlocal.text = localtext[SetType].text;
    }
    public void Useudonupload()
    {
        udonupload[SetType].SendCustomEvent("useme");
        forlocal.text = localtext[SetType].text;
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "setwhiteboard");
        setwhiteboard();
    }
    private void setwhiteboard()
    {
        whiteboardshow.text = forglobal.text+"\nBy:"+Networking.GetOwner(gameObject).displayName;
    } 

    public void udonset0(){ChangedFromButton(0, "辩题");}
    public void udonset1(){ ChangedFromButton(1, "话题"); }
    public void udonset2(){ ChangedFromButton(2, "预留11"); }
    public void udonset3(){ ChangedFromButton(3, "真心话"); }
    public void udonset4(){ ChangedFromButton(4, "大冒险"); }
    public void udonset5(){ ChangedFromButton(5, "国王游戏"); }
    public void udonset6(){ ChangedFromButton(6, "题词1"); }
    public void udonset7(){ ChangedFromButton(7, "题词2"); }
    public void udonset8(){ ChangedFromButton(8, "题词3"); }
    public void udonset9(){ ChangedFromButton(9, "预留31"); }
    public void udonset10(){ ChangedFromButton(10, "预留32"); }
    public void udonset11(){ ChangedFromButton(11, "预留33"); }
}
