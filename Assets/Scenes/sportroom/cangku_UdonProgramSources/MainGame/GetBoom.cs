using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using VRC.Udon;
using VRC.SDK3.Components;

public class GetBoom : UdonSharpBehaviour
{
    [UdonSynced] public bool isover = true;
    [UdonSynced] private bool isreset = true; // 用于标记是否已重置
    [UdonSynced] public string playerName = "";
    [UdonSynced] private float countdowntimelast = 1f;
    [UdonSynced] private float countdowntimestart = 1f;
    [UdonSynced] private int hitcount = 0;
    private Vector3 startPosition;
    public Animator ForMeMusicAni;
    public GameObject Area;
    public GameObject BoomShpere;
    public VRCPickup setVRCPickup; 
    void Start()
    {
        startPosition = BoomShpere.transform.position;
        ForMeMusicAni.SetBool("SetBOn",false);
        ForMeMusicAni.SetBool("countdown", false);
        Area.SetActive(false); // 初始时隐藏区域
    }
    public void Resetme()
    {
        if (!isover) return;
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        isreset=true; // 重置标记
        RequestSerialization();
        Setme();
    }
    public void SetStart()
    {
        if (!isover) return;
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        isover = false;
        isreset=false; // 重置标记
        countdowntimelast =Random.Range(3f, 6f);
        countdowntimestart = Random.Range(5f,24f);
        hitcount = 0;
        RequestSerialization(); // 同步数据
        Setme();
    }
    public override void OnDeserialization()
    {
        if(!isover||isreset)
            Setme(); // 在数据反序列化时调用Setme方法
        else
            setover(); // 如果游戏结束，调用setover方法
    }
    private void Setme()
    {
        if (isreset)
        {
            Area.SetActive(false);
            BoomShpere.transform.position = startPosition;
            setVRCPickup.UseText = "点击开始游戏";
            setVRCPickup.InteractionText= "点击开始游戏";
            isreset = false;
            ForMeMusicAni.SetBool("SetBOn", false);
            ForMeMusicAni.SetBool("countdown", false);
            Area.SetActive(false);
            return;
        }
        setVRCPickup.UseText = "用我砸别人";
        setVRCPickup.InteractionText = "用我砸别人";
        Area.SetActive(true);
        ForMeMusicAni.SetBool("SetBOn", true);
        SendCustomEventDelayedSeconds(nameof(Nset), countdowntimestart);
        
    }
    public void Nset()
    {
        setVRCPickup.UseText = "时间所剩不多了";
        setVRCPickup.InteractionText = "时间所剩不多了";
        ForMeMusicAni.SetBool("countdown", true);
        SendCustomEventDelayedSeconds(nameof(NNSet), countdowntimelast);
    }
    public void NNSet()
    {
        if (!Networking.IsOwner(gameObject)) return;
        isover = true;
        isreset = false; // 重置标记
        RequestSerialization();
        setover();
    }
    private void setover()
    {
        ForMeMusicAni.SetBool("SetBOn", false);
        ForMeMusicAni.SetBool("countdown", false);
        setVRCPickup.UseText = $"游戏结束共击中{hitcount}次，\n{playerName} 被选中了，\n点击开始下轮游戏";
        setVRCPickup.InteractionText = $"游戏结束共击中{hitcount}次，\n{playerName} 被选中了，\n点击开始下轮游戏";
        Area.SetActive(false);
    }
    public void Setname(string Name,bool hit)//不断更换玩家名称和所有权
    {
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        playerName = Name;
        if(hit)hitcount++;
    }
    public override void Interact()
    {
        Resetme();
    }
}
