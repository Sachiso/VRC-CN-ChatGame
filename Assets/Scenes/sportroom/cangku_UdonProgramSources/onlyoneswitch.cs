
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class onlyoneswitch : UdonSharpBehaviour
{
    public GameObject objectAAA; // 物体
    [UdonSynced] private bool oosw;
    public GameObject foron;
    public GameObject foroff;
    void Start()
    {
        oosw=false;
        objectAAA.SetActive(oosw); // 设置物体属性
    }
    public override void Interact() // 点击交互时调用
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject); // 设置对象所有权
        oosw = !oosw;
        if (foron != null && foroff != null)
        {
            foron.SetActive(oosw);
            foroff.SetActive(!oosw);
        }
        RequestSerialization();// 同步状态给所有客户端 
        objectAAA.SetActive(oosw);
    }
    public override void OnDeserialization()// 当状态同步时更新物体状态
    {
        objectAAA.SetActive(oosw);
    }
    public void seton() { oosw = false; }
    public void setoff() { oosw = true; }

}
