using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ZDHDMX : UdonSharpBehaviour
{
    public Image nullimage;//读取的空图片
    public Text showZXHText;//读取的真心话
    public Text showDMXText;//读取的大冒险
    public GanDengYan GanDengYan;//读取空卡牌和扑克牌：GanDengya.nullimage与GanDengyan.allpuke
    public Image ZXH;//读取的真心话卡面
    public Image DMX;//读取的大冒险卡面

    public Image[] minecards;//我的两张卡牌的显示，沿用allpuke来赋予
    public Image showMyCards;//展示的卡牌图片显示
    public TextMeshProUGUI[] playersName;//玩家名单
    public Image[] ZXHDMXimages;//真心话大冒险的展示卡面
    public TextMeshProUGUI[] ZXHDMXtext;//真心话大冒险的文本显示
    public TextMeshProUGUI usingPN;

    /// <summary>全部文本内容的索引</summary>
    [UdonSynced] private int[] GetZXHCardsID=new int[100];//真心话序列化文本索引
    [UdonSynced] private int[] GetDMXCardsID=new int[100];///大冒险序列化文本索引
    [UdonSynced] private int ZXHLength = 0;
    [UdonSynced] private int DMXLength = 0;
    [UdonSynced] private int[] ZDCardcount=new int[2];//索引的序列
    [UdonSynced] private int[] ShowZDCardsID=new int[5];//真心话展示索引
    [UdonSynced] private bool[] IsZXH=new bool[5]; //真心话还是大冒险？
    /// <summary>手牌文本的索引</summary>
    private int[] MinecardsID=new int[2];
    private int[] OnSelectMeAndAll=new int[2];
    [UdonSynced] string UsingPN = "";
    /// <summary>显示的手牌</summary>
    [UdonSynced] private int ShowMyCardsID;
    [UdonSynced] string[] PlayersName=new string[8];

    void Start()
    {
        if(Networking.IsOwner(gameObject))Resetall();
    }
    public void Resetall()
    {
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        string[] temps=new string[0];
        usualuseclass.LoadTextToString(showZXHText,ref temps, ref ZXHLength);
        usualuseclass.SetIntOrder(ref GetZXHCardsID);
        usualuseclass.LoadTextToString(showDMXText, ref temps, ref DMXLength);
        usualuseclass.SetIntOrder(ref GetDMXCardsID);

        ZDCardcount[0] = ZXHLength - 1;ZDCardcount[1] = DMXLength - 1;
        usualuseclass.ResetIntToInt(ref ShowZDCardsID, -1);
        usualuseclass.ResetBoolToBool(ref IsZXH,false);
        usualuseclass.ResetIntToInt(ref MinecardsID, -1);
        usualuseclass.ResetStringToStringArray(ref PlayersName, "");
        ShowMyCardsID = -1;
        RequestSerialization();
        Setall();
    }
    public override void OnDeserialization()
    {
        Setall();
    }
    private void Setall()
    {
        SetMe();
        SetShow();
    }
    private void SetMe()
    {
        if (MinecardsID[0] == -1)
        {
            if (UsingPN == Networking.LocalPlayer.displayName)
                minecards[0].sprite = GanDengYan.nullimage.sprite;
            else
            {
                int i = UnityEngine.Random.Range(0, 107);
                MinecardsID[0] = i;
                minecards[0].sprite = GanDengYan.allpuke[i].sprite;
            }
        }
        else
        {
            minecards[0].sprite = GanDengYan.allpuke[MinecardsID[0]].sprite;
        }
        if (MinecardsID[1] == -1)
        {
            if (UsingPN == Networking.LocalPlayer.displayName)
                minecards[1].sprite = GanDengYan.nullimage.sprite;
            else
            {
                int i = UnityEngine.Random.Range(0, 107);
                MinecardsID[1] = i;
                minecards[1].sprite = GanDengYan.allpuke[i].sprite;
            }
        }
        else
        {
            minecards[1].sprite = GanDengYan.allpuke[MinecardsID[1]].sprite;
        }
        minecards[0].color = Color.white;
        minecards[1].color = Color.white;
    }
    private void SetShow()
    {
        int tempint = 0;
        string[] tempZXH = new string[0];
        usualuseclass.LoadTextToString(showZXHText, ref tempZXH,ref tempint);
        string[] tempDMX = new string[0];
        usualuseclass.LoadTextToString(showDMXText, ref tempDMX, ref tempint);


        if(ShowMyCardsID==-1) {showMyCards.sprite=GanDengYan.nullimage.sprite;}
        else showMyCards.sprite = GanDengYan.allpuke[ShowMyCardsID].sprite;
        usualuseclass.SetStringArrayToTMP(ref playersName, PlayersName);

    }
}
