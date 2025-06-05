
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class OnceUponATime : UdonSharpBehaviour
{
    public TextMeshProUGUI[] TMPforPlayerInGame;
    public Image[] normalcards;
    public Image[] targetcards;
    public Image[] minecards;
    public Image[] showcards;
    public TextMeshProUGUI[] showcardPN;
    public TextMeshProUGUI TMPForUPN;//显示当前讲述玩家的TMP
    public Image nullimage;
    private int ForSelectMineCardint = -1;//存储当前玩家选择的手牌序列，-1表示没有选择
    private int[] ForSelectNTID = new int[20];//存储当前玩家手牌的ID，0是目标卡牌，1-19是普通卡牌
    private int MineCardscount = 5;//当前玩家手牌数量，默认为5张
    [UdonSynced] private string[] PNInGames= new string[8];//同步的玩家列表，最多8人
    [UdonSynced] private int InGameCount = 0;//当前游戏中的玩家数量，最多8人
    [UdonSynced] private int[] NcardID = new int[100];//普通卡牌的ID序列存储，96-99是加牌卡牌,89919395是换牌
    [UdonSynced] private int Ncardcount = 99;//普通卡牌的序列，表示当前剩余的卡牌数量和序列
    [UdonSynced] private int[] TcardID = new int[8];//目标卡牌的ID序列存储
    [UdonSynced] private int Tcardcount = 7;//目标卡牌的序列，表示当前剩余的卡牌数量和序列
    [UdonSynced] private int[] ShowcardID = new int[5];//显示打出卡牌的ID序列存储，-1表示没有卡牌
    [UdonSynced] private string[] ShowcardPN= new string[5];//打出卡牌对应的玩家名称存储
    [UdonSynced] private int AddCardCount = 1;//加牌数量，默认为1张，可能会变成2或3张
    [UdonSynced] private string UsingPN = "空";//当前讲述人名称，默认为空
    [UdonSynced] private bool gameover = false;//游戏是否结束，默认为false
    [UdonSynced] private bool isadd = false;//是否加牌，默认为false
    public TextMeshProUGUI[] CopyToTMPforPlayerInGame;
    public Image[] CopyTominecards;
    public Image[] CopyToshowcards;
    public TextMeshProUGUI[] CopyToshowcardPN;
    public TextMeshProUGUI CopyToTMPForUPN;
    void Start()//初始化所有空数据
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        for (int i = 0; i < NcardID.Length;i++)
        {
            NcardID[i] = i;
        }
        for (int i = 0; i < TcardID.Length;i++)
        {
            TcardID[i] = i;
        }
        ResetAll();
    }

    public void ResetAll()//重置所有卡牌，一共五个板块，分别是showcards、minecards，usingPN、showcardPN和PNInGames
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        InGameCount = 0;
        gameover = false;
        isadd = false;
        for (int i = 0; i < 20; i++)
        {
            minecards[i].sprite = nullimage.sprite;
            ForSelectNTID[i]= -1;
        }
        for (int i = 0; i < 8; i++)
        {
            PNInGames[i] = "";
        }
        for (int i = 0; i < 5; i++)
        {
            ShowcardID[i] = -1;
            ShowcardPN[i] = "";
        }
        SetCardRandom();
        Ncardcount = 99;
        Tcardcount = 7;
        UsingPN = "空";
        RequestSerialization();
        Setall();
    }
    public void SetPlayerJoinGame()
    {
        if (InGameCount >= 8 || gameover|| Ncardcount <= 3) return;
        for(int i = 0; i < InGameCount; i++)
        {
            if(PNInGames[i] == Networking.LocalPlayer.displayName) return;
        }
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        PNInGames[InGameCount] = Networking.LocalPlayer.displayName;
        if (InGameCount == 0)
        {
            UsingPN = Networking.LocalPlayer.displayName;
        }
        InGameCount++;
        for (int i = 1; i < 19; i++)
        {
            if (i > 4) 
            {
                minecards[i].sprite = nullimage.sprite;//清空手牌超出数量的手牌
            }
            else
            {
                ForSelectNTID[i] = NcardID[Ncardcount - (i-1)];
                minecards[i].sprite = normalcards[ForSelectNTID[i]].sprite;
            }
        }
        ForSelectNTID[0] = TcardID[Tcardcount];
        minecards[0].sprite = targetcards[Tcardcount].sprite;
        MineCardscount = 5;
        Ncardcount -=4;
        Tcardcount--;
        RequestSerialization();
        Setall();
    }
    private void SetminecardForSelect(int ForSelectMCID)
    {
        if(ForSelectMCID > MineCardscount-1) return;
        if (ForSelectMineCardint != ForSelectMCID)
        {
            ForSelectMineCardint = ForSelectMCID;
        }
        else ForSelectMineCardint = -1;
        for (int i = 0; i < 20; i++)
        {
            CopyTominecards[i].color = (minecards[i].color = Color.white);
            
        }
        if (ForSelectMineCardint != -1)
        {
            CopyTominecards[ForSelectMineCardint].color = (minecards[ForSelectMineCardint].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f));
        }
    }
    public void OnPlayerShowCard()
    {
        if (ForSelectMineCardint == -1 || (ForSelectMineCardint == 0 && MineCardscount > 1)||gameover)
        {
            SetminecardForSelect(-1);
            return;
        }
        for (int i = 0; i < 8; i++)
        {
            if (Networking.LocalPlayer.displayName == PNInGames[i]) break;
            else if (i == 7) return;
        }
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        int savedcardID = ForSelectNTID[ForSelectMineCardint];//保存的当前选中卡牌的ID
        if (savedcardID >= 96)//加牌卡牌的条件
        {
            isadd = true;
        }
        else if (TMPForUPN.text==Networking.LocalPlayer.displayName)//讲述人是自己，不用加牌
        {
            isadd = false;
        }
        else//讲述人是其他人，给人加牌
        {
            if (Ncardcount > -1)
            {
                isadd = true;
                Ncardcount--;
                AddCardCount = 1;
            }
            else
            {
                isadd = false;
            }
            UsingPN = Networking.LocalPlayer.displayName;
        }
        if (ForSelectMineCardint == 0)//查询游戏是否结束
        {
            gameover = true;
            minecards[0].sprite = nullimage.sprite;
        }
        for (int i = 4; i > 0; i--) //设置场上的5张显示卡牌
        {
                ShowcardPN[i] = ShowcardPN[i - 1];
                ShowcardID[i] = ShowcardID[i - 1];
        }
        ShowcardID[0] = savedcardID;
        ShowcardPN[0] = Networking.LocalPlayer.displayName;
        MineCardscount--;
        for(int i=ForSelectMineCardint;i<MineCardscount; i++)//这是一个手牌的整理
        {
            ForSelectNTID[i] = ForSelectNTID[i + 1];
            minecards[i].sprite = minecards[i + 1].sprite;
        }
        minecards[MineCardscount].sprite = nullimage.sprite;
        switch (savedcardID)
        {
            case 99: case 98://大王牌，就是给别人加3张牌
                {
                    if (Ncardcount >= 0)//这段逻辑是排查卡组里的牌，然后给自己加一张
                    {
                        ForSelectNTID[MineCardscount] = NcardID[Ncardcount];
                        minecards[MineCardscount].sprite = normalcards[ForSelectNTID[MineCardscount]].sprite;
                        MineCardscount++;
                        Ncardcount--;
                    }
                    if (Ncardcount >= 2)
                    {
                        AddCardCount = 3;
                        Ncardcount -= AddCardCount;
                    }
                    else if (Ncardcount < 0)
                    {
                        isadd = false;
                    }
                    else
                    {
                        AddCardCount = Ncardcount + 1;
                        Ncardcount -= AddCardCount;
                    }
                } 
                break;
            case 97: case 96://小王牌，就是给别人加2张牌
                {
                    if (Ncardcount >= 0)
                    {
                        ForSelectNTID[MineCardscount] = NcardID[Ncardcount];
                        minecards[MineCardscount].sprite = normalcards[ForSelectNTID[MineCardscount]].sprite;
                        MineCardscount++;
                        Ncardcount--;
                    }
                    if (Ncardcount >= 1)
                    {
                        AddCardCount = 2;
                        Ncardcount -= AddCardCount;
                    }
                    else if (Ncardcount < 0)
                    {
                        isadd = false;
                    }
                    else
                    {
                        AddCardCount = Ncardcount + 1;
                        Ncardcount -= AddCardCount;
                    }
                }
                break;
            case 95:case 93:case 91:case 89://换牌卡牌，给讲述人换牌
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (Ncardcount >= 0)
                        {
                            ForSelectNTID[MineCardscount] = NcardID[Ncardcount];
                            minecards[MineCardscount].sprite = normalcards[ForSelectNTID[MineCardscount]].sprite;
                            MineCardscount++;
                            Ncardcount--;
                        }
                    }
                }
                break;
            default: break;
        }
        RequestSerialization();
        Setall();
        SetminecardForSelect(-1);
    }
    public void OnPlayergetcard()//给无法继续讲述故事的人准备的过牌
    {
        if (gameover) return;
        for (int i = 0; i < 8; i++)
        {
            if (Networking.LocalPlayer.displayName == PNInGames[i]) break;
            else if (i == 7) return;
        }
        if (!Networking.IsOwner(GetOwn(),gameObject)) return;
        isadd = false;
        if (Ncardcount >= 0 && MineCardscount <= 19)
        {
            ForSelectNTID[MineCardscount] = NcardID[Ncardcount];
            minecards[MineCardscount].sprite = normalcards[ForSelectNTID[MineCardscount]].sprite;
            MineCardscount++;
            Ncardcount--;
        }
        for (int i = 0; i < 8; i++)
        {
            if (TMPForUPN.text == PNInGames[i])
            {
                if (i == 7) { i = 0; }
                else if (PNInGames[i + 1] == "") { i = 0; }
                else {i++;}
                UsingPN = PNInGames[i];
                break;
            }
        }
        RequestSerialization();
        Setall();
    }
    public override void OnDeserialization()//同步数据后调用
    {
        Setall();
    }
    private void Setall()//更新所有卡牌和玩家信息
    {
        if (isadd && TMPForUPN.text == Networking.LocalPlayer.displayName)//加牌逻辑
        {
            for (int i = 0; i < AddCardCount&&MineCardscount<=19; i++)
            {
                ForSelectNTID[MineCardscount] = NcardID[Ncardcount + i + 1];
                minecards[MineCardscount].sprite = normalcards[ForSelectNTID[MineCardscount]].sprite;//这里要给左侧赋予卡牌
                MineCardscount++;
            }
        }
        SetInGamePlayer();//更新玩家列表信息
        Setshowcards();//更新显示卡牌信息
        CopyToWorld();
    }
    private void Setshowcards()
    {
        for (int i = 0; i < 5; i++)
        {
            showcardPN[i].text = ShowcardPN[i];
        }

        if (gameover)
        {
            showcards[0].sprite = targetcards[ShowcardID[0]].sprite;
            for (int i = 1; i < 5; i++)
            {
                if (ShowcardID[i] == -1)
                {
                    showcards[i].sprite = nullimage.sprite;
                }
                else
                {
                    showcards[i].sprite = normalcards[ShowcardID[i]].sprite;
                }
            }
            TMPForUPN.text = $"游戏结束，请重置游戏";//当前讲述人信息更新
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (ShowcardID[i] == -1)
                {
                    showcards[i].sprite = nullimage.sprite;
                }
                else
                {
                    showcards[i].sprite = normalcards[ShowcardID[i]].sprite;
                }
            }
            TMPForUPN.text = UsingPN;//当前讲述人信息更新
        }
       
    }
    private void SetInGamePlayer()
    {
        for (int i = 0;i<8; i++)
        {
            TMPforPlayerInGame[i].text = $"{i+1}.{PNInGames[i]}";
        }
    }
    private void SetCardRandom()//随机化NcardID和TcardID数组的顺序
    {
        for (int i = 0; i < NcardID.Length; i++)
        {
            int randomIndex = Random.Range(i, NcardID.Length);
            int temp = NcardID[i];
            NcardID[i] = NcardID[randomIndex];
            NcardID[randomIndex] = temp;
        }
        for (int i = 0; i < TcardID.Length; i++)
        {
            int randomIndex = Random.Range(i, TcardID.Length);
            int temp = TcardID[i];
            TcardID[i] = TcardID[randomIndex];
            TcardID[randomIndex] = temp;
        }
    }
    private void CopyToWorld()
    {
        int i;
        for (i = 0; i < TMPforPlayerInGame.Length; i++)
        {
            CopyToTMPforPlayerInGame[i].text = TMPforPlayerInGame[i].text;
        }
        for (i = 0; i < minecards.Length; i++)
        {
            CopyTominecards[i].sprite = minecards[i].sprite;
        }
        for (i = 0; i < showcards.Length; i++)
        {
            CopyToshowcards[i].sprite = showcards[i].sprite;
        }
        for (i = 0; i < showcardPN.Length; i++)
        {
            CopyToshowcardPN[i].text = showcardPN[i].text;
        }
        CopyToTMPForUPN.text = TMPForUPN.text;
    }
    private VRCPlayerApi GetOwn()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        return Networking.LocalPlayer;
    }
    public void Selectminecard0() { SetminecardForSelect(0); }
    public void Selectminecard1() { SetminecardForSelect(1); }
    public void Selectminecard2() { SetminecardForSelect(2); }
    public void Selectminecard3() { SetminecardForSelect(3); }
    public void Selectminecard4() { SetminecardForSelect(4); }
    public void Selectminecard5() { SetminecardForSelect(5); }
    public void Selectminecard6() { SetminecardForSelect(6); }
    public void Selectminecard7() { SetminecardForSelect(7); }
    public void Selectminecard8() { SetminecardForSelect(8); }
    public void Selectminecard9() { SetminecardForSelect(9); }
    public void Selectminecard10() { SetminecardForSelect(10); }
    public void Selectminecard11() { SetminecardForSelect(11); }
    public void Selectminecard12() { SetminecardForSelect(12); }
    public void Selectminecard13() { SetminecardForSelect(13); }
    public void Selectminecard14() { SetminecardForSelect(14); }
    public void Selectminecard15() { SetminecardForSelect(15); }
    public void Selectminecard16() { SetminecardForSelect(16); }
    public void Selectminecard17() { SetminecardForSelect(17); }
    public void Selectminecard18() { SetminecardForSelect(18); }
    public void Selectminecard19() { SetminecardForSelect(19); }
}
