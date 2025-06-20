using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ZDHDMX : UdonSharpBehaviour
{
    public Text showZXHText;//读取的真心话
    public Text showDMXText;//读取的大冒险
    public Text showNormalText;
    public GanDengYan GanDengYan;//读取空卡牌和扑克牌：GanDengya.nullimage与GanDengyan.allpuke
    public Image ZXH;//读取的真心话卡面
    public Image DMX;//读取的大冒险卡面
    public Image Normal;

    public Image[] minecards;//我的两张卡牌的显示，沿用allpuke来赋予
    public Image[] cpminecards;
    public Image showMyCards;//展示的卡牌图片显示
    public Image cpshowMyCards;
    public Image[] ZXHDMXimages;//真心话大冒险的展示卡面
    public Image[] cpZXHDMXimages;
    public TextMeshProUGUI[] ZXHDMXtext;//真心话大冒险的文本显示
    public TextMeshProUGUI[] cpZXHDMXtext;
    public TextMeshProUGUI usingPN;//展示Owner的角色名
    public TextMeshProUGUI cpuingPN;
    public Image SetOwnBut;//获取所有权的but色彩
    public Image cpSetOwnBut;
    public TextMeshProUGUI[] SetButTex;//是上面对应的文本，0和1,1是type。
    public TextMeshProUGUI[] cpSetButTex;

    /// <summary>全部文本内容的索引</summary>
    [UdonSynced] private int[] GetZXHCardsID=new int[0];//真心话序列化文本索引
    [UdonSynced] private int[] GetDMXCardsID=new int[0];//大冒险序列化文本索引
    [UdonSynced] private int[] GetNormalCardsID=new int[0];
    [UdonSynced] private int[] ZDCardcount=new int[3];//索引的序列
    [UdonSynced] private int[] ShowZDCardsID=new int[5];//真心话展示索引
    [UdonSynced] private int[] ZXHType = new int[5]; //真心话还是大冒险？
    [UdonSynced] private bool ReallyRandom = false;
    /// <summary>手牌文本的索引</summary>
    private int[] MinecardsID=new int[2] { -1,-1};
    private int[] OnSelectMeAndAll=new int[2] {-1,-1};
    [UdonSynced] string UsingPN = "";
    /// <summary>显示的手牌</summary>
    [UdonSynced] private int ShowMyCardsID;
    void Start()
    {
        minecards[0].sprite=GanDengYan.nullimage.sprite;
        minecards[1].sprite=GanDengYan.nullimage.sprite;
        usualuseclass.ResetIntToInt(ref MinecardsID, -1);
        usualuseclass.ResetIntToInt(ref OnSelectMeAndAll, -1);
        if (Networking.IsOwner(gameObject)) {
             Resetall();
        }
    }
    public void SetOwn()
    {
        if (!usualuseclass.IsSetOwn(gameObject)) return;
        UsingPN = Networking.LocalPlayer.displayName;
        RequestSerialization();
        SetMe();
        SetShow();
    }//单独一个获取所有权的按钮
    private void SetMe()
    {
        for (int i = 0; i < 2; i++)
        {
            if (MinecardsID[i] == -1)
            {
                int setR = Random.Range(0, 108);
                MinecardsID[i] = setR;
                minecards[i].sprite = GanDengYan.allpuke[setR].sprite;
            }
            else
            {
                minecards[i].sprite = GanDengYan.allpuke[MinecardsID[i]].sprite;
            }
            minecards[i].color = Color.white;
        }
        
    }//只有在设置所有权的时候会对自己的数组初始化操作
    public void Resetall()//重设所有基础数据
    {
        if (!Networking.IsOwner(gameObject)) return;
        string[] temps=new string[0];
        usualuseclass.LoadTextToString(showZXHText,ref temps);
        ZDCardcount[0] = temps.Length - 1;
        GetZXHCardsID= new int[temps.Length];
        usualuseclass.SetIntOrder(ref GetZXHCardsID);
        usualuseclass.LoadTextToString(showDMXText, ref temps);
        ZDCardcount[1] = temps.Length - 1;
        GetDMXCardsID= new int[temps.Length];
        usualuseclass.SetIntOrder(ref GetDMXCardsID);
        usualuseclass.LoadTextToString(showNormalText,ref temps);
        ZDCardcount[2]= temps.Length - 1;
        GetNormalCardsID= new int[temps.Length];
        usualuseclass.SetIntOrder(ref GetNormalCardsID);
        
        ReallyRandom = false;
        usualuseclass.ResetIntToInt(ref ShowZDCardsID, -1);
        usualuseclass.ResetIntToInt(ref ZXHType,0);
        ShowMyCardsID = -1;
        UsingPN = "";
        RequestSerialization();
        SetShow();
    }
    public override void OnDeserialization()
    {
        SetShow();
    }//同步用的
    private void SetShow()
    {
        if (ShowMyCardsID == -1) { showMyCards.sprite = GanDengYan.nullimage.sprite; }
        else showMyCards.sprite = GanDengYan.allpuke[ShowMyCardsID].sprite;
        for (int i = 0; i < 5; i++)
        {
            if (ShowZDCardsID[i] == -1)
            {
                ZXHDMXimages[i].sprite = GanDengYan.nullimage.sprite;
                ZXHDMXtext[i].text = "";
            }
            else
            {
                switch (ZXHType[i])
                {
                    case 1:
                        {
                            string[] tempZXH = new string[0];
                            usualuseclass.LoadTextToString(showZXHText, ref tempZXH);
                            ZXHDMXimages[i].sprite = ZXH.sprite;
                            ZXHDMXtext[i].text = tempZXH[ShowZDCardsID[i]];
                            ZXHDMXtext[i].color = new Color(66f / 255f, 66f / 255f, 0f);
                        }
                        break;
                    case -1:
                        {
                            string[] tempDMX = new string[0];
                            usualuseclass.LoadTextToString(showDMXText, ref tempDMX);
                            ZXHDMXimages[i].sprite = DMX.sprite;
                            ZXHDMXtext[i].text = tempDMX[ShowZDCardsID[i]];
                            ZXHDMXtext[i].color = new Color(166f / 255f, 166f / 255f, 0f);
                        }
                        break;
                    case 0:
                        {
                            string[] tempNormal = new string[0];
                            usualuseclass.LoadTextToString(showNormalText, ref tempNormal);
                            ZXHDMXimages[i].sprite = Normal.sprite;
                            ZXHDMXtext[i].text = tempNormal[ShowZDCardsID[i]];
                            ZXHDMXtext[i].color = new Color(50f / 50f, 166f / 50f, 0f);
                        }
                        break;
                    default:break;
                }
            }
            ZXHDMXimages[i].color = Color.white;
        }
        if (Networking.IsOwner(gameObject)) { SetButTex[0].text = "已为执行者"; SetOwnBut.color = new Color(60f / 255f, 38f / 255f, 37f / 255f); }
        else { SetButTex[0].text = "成为执行者"; SetOwnBut.color= new Color(37f / 255f, 60f / 255f, 37f / 255f); }
        if (ReallyRandom) SetButTex[1].text = "当前模式：<color=#C17777>真随机</color>";
        else SetButTex[1].text = "当前模式：<color=#C17777>序列化</color>";
        usingPN.text = UsingPN;
        CPTOWORLD();
    }//同步与自己的全局现实内容设置
    private void CPTOWORLD()
    {
        for (int i = 0; i < 2; i++)
        {
            cpminecards[i].sprite = minecards[i].sprite;
            cpminecards[i].color = minecards[i].color;
        }
        cpshowMyCards.sprite = showMyCards.sprite;
        for (int i = 0; i < 5; i++)
        {
            cpZXHDMXimages[i].sprite = ZXHDMXimages[i].sprite;
            cpZXHDMXimages[i].color = ZXHDMXimages[i].color;
            cpZXHDMXtext[i].text = ZXHDMXtext[i].text;
        }
        cpuingPN.text = UsingPN;
        cpSetOwnBut.color = SetOwnBut.color;
        for (int i = 0; i < 2; i++)
        {
            cpSetButTex[i].text = SetButTex[i].text;
        }
    }
    public void OrderRandomSwitch()
    {
        if (!Networking.IsOwner(gameObject)) return;
        if(!ReallyRandom)return;
        ReallyRandom = false;
        RequestSerialization();
        SetShow();
    }//设置随机模式
    public void ReallyRandomSwitch() { 
        if (!Networking.IsOwner(gameObject)) return;
        if (ReallyRandom) return;
        ReallyRandom = true;
        RequestSerialization();
        SetShow();
    }//设置随机模式
    public void GetZXH()
    {
        if (!Networking.IsOwner(gameObject)) return;
        int nullimage = 0;
        foreach (int SCID in ShowZDCardsID) { if (SCID == -1) break; nullimage++; }
        if (nullimage == 5) return;
        if (!ReallyRandom) { 
            ShowZDCardsID[nullimage] = GetZXHCardsID[ZDCardcount[0]];
            if (ZDCardcount[0] == 0) ZDCardcount[0]=GetZXHCardsID.Length;
            ZDCardcount[0]--;
        }
        else
        {
            int[] tempints = new int[ShowZDCardsID.Length];
            usualuseclass.SetIntArrayToIntArray(ref tempints,ShowZDCardsID,ShowZDCardsID.Length);
            int count = 0;
            foreach (int b in ZXHType) { if (b!=1) tempints[count] = -1; count++; }//对每个Type操作，
            ShowZDCardsID[nullimage] = usualuseclass.RandomWithoutExcept(0, GetZXHCardsID.Length, tempints);
        }
        ZXHType[nullimage] = 1;
        RequestSerialization();
        SetShow();
    }//获取真心话卡牌
    public void GetDMX()
    {
        if (!Networking.IsOwner(gameObject)) return;
        int nullimage = 0;
        foreach (int SCID in ShowZDCardsID) { if (SCID == -1) break; nullimage++; }
        if (nullimage == 5) return;
        if (!ReallyRandom)
        {
            ShowZDCardsID[nullimage] = GetDMXCardsID[ZDCardcount[1]];
            if (ZDCardcount[1] == 0) ZDCardcount[1] = GetDMXCardsID.Length;
            ZDCardcount[1]--;
        }
        else
        {
            int[] tempints = new int[ShowZDCardsID.Length];
            usualuseclass.SetIntArrayToIntArray(ref tempints, ShowZDCardsID, ShowZDCardsID.Length);
            int count = 0;
            foreach (int b in ZXHType) { if (b!=-1) tempints[count] = -1; count++; }
            ShowZDCardsID[nullimage] = usualuseclass.RandomWithoutExcept(0, GetDMXCardsID.Length, tempints);
        }
        ZXHType[nullimage] = -1;
        RequestSerialization();
        SetShow();
    }//获取大冒险卡牌
    public void GetNormal()
    {
        if (!Networking.IsOwner(gameObject)) return;
        int nullimage = 0;
        foreach (int SCID in ShowZDCardsID) { if (SCID == -1) break; nullimage++; }
        if (nullimage == 5) return;
        if (!ReallyRandom)
        {
            ShowZDCardsID[nullimage] = GetNormalCardsID[ZDCardcount[1]];
            if (ZDCardcount[1] == 0) ZDCardcount[1] = GetNormalCardsID.Length;
            ZDCardcount[1]--;
        }
        else
        {
            int[] tempints = new int[ShowZDCardsID.Length];
            usualuseclass.SetIntArrayToIntArray(ref tempints, ShowZDCardsID, ShowZDCardsID.Length);
            int count = 0;
            foreach (int b in ZXHType) { if (b != 0) tempints[count] = -1; count++; }
            ShowZDCardsID[nullimage] = usualuseclass.RandomWithoutExcept(0, GetNormalCardsID.Length, tempints);
        }
        ZXHType[nullimage] = 0;
        RequestSerialization();
        SetShow();
    }//获取普通卡牌
    public void ShowMyCard()
    {
        if(!Networking.IsOwner(gameObject)) return;
        if(OnSelectMeAndAll[0]==-1) return;
        ShowMyCardsID = MinecardsID[OnSelectMeAndAll[0]];
        RequestSerialization();
        MinecardsID[OnSelectMeAndAll[0]] = -1;
        minecards[OnSelectMeAndAll[0]].sprite = GanDengYan.nullimage.sprite;
        minecards[OnSelectMeAndAll[0]].color = Color.white;
        OnSelectMeAndAll[0] = -1;
        SetShow();
    }//显示自己的单张牌按钮的设置
    public void ClearZXHDMXCard()
    {
        if (!Networking.IsOwner(gameObject)) return;
        if (OnSelectMeAndAll[1] == -1) return;
        ShowZDCardsID[OnSelectMeAndAll[1]]=-1;
        RequestSerialization();
        SetShow();
    }//清除选择的真心话大冒险的卡牌
    private void SelectMe(int i)//自身卡牌的选择
    {
        if (!Networking.IsOwner(gameObject)) return;
        for (int j = 0; j < 2; j++) { minecards[j].color = Color.white; }
        if (OnSelectMeAndAll[0] == i)
        {
            OnSelectMeAndAll[0] = -1;
            minecards[i].color = Color.white;
        }
        else
        {
            if (MinecardsID[i] == -1) { return; }
            else
            {
                OnSelectMeAndAll[0] = i;
                minecards[i].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);
            }
        }
        CPTOWORLD();
    }
    public void SelectMe0() { SelectMe(0); }
    public void SelectMe1() { SelectMe(1); }
    private void SelectZXHDMXCards(int i)
    {
        if (!Networking.IsOwner(gameObject)) return;
        for(int j=0;j<5;j++) {ZXHDMXimages[j].color= Color.white; }
        if (OnSelectMeAndAll[1] == i)
        {
            OnSelectMeAndAll[1] = -1;
        }
        else
        {
            if (ShowZDCardsID[i] == -1) { OnSelectMeAndAll[1] = -1; return; }
            else
            {
                OnSelectMeAndAll[1] = i;
                ZXHDMXimages[i].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);
            }
        }
        CPTOWORLD();
    }//真心话大冒险的卡牌的选择
    public void SelectZXHDMXCards0() { SelectZXHDMXCards(0); }
    public void SelectZXHDMXCards1() { SelectZXHDMXCards(1); }
    public void SelectZXHDMXCards2() { SelectZXHDMXCards(2); }
    public void SelectZXHDMXCards3() { SelectZXHDMXCards(3); }
    public void SelectZXHDMXCards4() { SelectZXHDMXCards(4); }

}
