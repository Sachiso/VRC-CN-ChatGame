
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class TMTglobal : UdonSharpBehaviour
{
    public Text globaldisplayText; // 挂载的Text组件，包含所有问题
    public Text localdisplayText;  // 用于显示随机选择的问题的Text组件（可选）
    public Text lgbar;
    [UdonSynced] private string saved;
    public override void Interact() // 随机选择一个问题并显示
    {
        useme();
    }
    public void useme()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject); // 设置对象所有权
        if (localdisplayText.text != "") saved = localdisplayText.text;
        else saved = globaldisplayText.text;
        RequestSerialization();// 同步状态给所有客户端 
        localdisplayText.text = "";
        globaldisplayText.text = saved;
        lgbar.text = saved;
    }
    public override void OnDeserialization()// 当状态同步时更新物体状态
    {
        globaldisplayText.text = saved;
        lgbar.text = saved;
    }
}
