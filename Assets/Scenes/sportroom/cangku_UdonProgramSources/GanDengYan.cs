
using System.Diagnostics.Eventing.Reader;
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

enum pukestate { None,One,Two,Three, TwoTwo,ThreeTwo,BoomOne,BoomTwo,Boom,ThreeS,FourS,FiveS,SixS,FiveBoom,SixBoom,TwoTwoTwo,ThreeThree}
public class GanDengYan : UdonSharpBehaviour
{
    public Image[] minecards;
    public Image[] copytoMinecards;
    public Image[] showcards;
    public Image[] copytoShowcards;
    public TextMeshProUGUI[] playersNameforCard;
    public TextMeshProUGUI[] copytoPlayersNameforCard;
    //以上是玩家手牌和展示牌相关的UI元素
    public TextMeshProUGUI[] playersInGame;
    public TextMeshProUGUI[] copytoPlayersInGame;
    public TextMeshProUGUI[] playersScore;
    public TextMeshProUGUI[] copytoPlayersScore;
    public TextMeshProUGUI[] playersCardCount;
    public TextMeshProUGUI[] copytoPlayersCardCount;
    //以上是玩家信息相关的UI元素
    public Image[] allpuke;
    public TextMeshProUGUI usingPN;
    public TextMeshProUGUI copytoUingPN;
    public Image nullimage;
    [UdonSynced] private string[] PlayersName=new string[8];//玩家名称
    [UdonSynced] private int[] PlayersScore=new int[8];//玩家分数
    [UdonSynced] private int[] PlayersCardCount=new int[8];//玩家手牌数量
    [UdonSynced] private int PlayersCount = 0;//玩家数量
    private int MineCardsCount;//自身剩余卡牌数
    private int[] MyCardsID = new int[6];//用于存储玩家手牌ID
    private int[] MySelectCardsID = new int[6];//用于存储玩家选择的卡牌ID
    [UdonSynced] private int[] AllCardsID=new int[108];//所有卡牌ID,用于乱序索引，id指向allpuke数组
    [UdonSynced] private int Cardscount=107;//剩余卡牌数
    [UdonSynced] private int[] PlayersPukeNub;//玩家吐牌数字，用作对比,动态变化
    [UdonSynced] private int[] ShowPukeID = new int[6];//展示牌ID索引，索引到allpuke数组
    [UdonSynced] private pukestate ShowPukeType;//展示牌类型
    [UdonSynced] private string[] ShowPukePN=new string[6];//展示牌的玩家名称
    [UdonSynced] private int UsingPNid;//当前使用的玩家名称在 PlayersName 数组中的索引
    [UdonSynced] private bool GameOver=true;
    [UdonSynced] private bool IsNewGame = false;
    [UdonSynced] private bool IsAdd = false;
    //以上是游戏数据相关的变量
    public GameObject[] JSbutton;
    [UdonSynced] private bool ResetButton = true;//加入游戏按钮状态
    void Start()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        for (int i = 0; i < 108; i++) 
        {
            AllCardsID[i] = i;
        }
        ResetAll();
    }
    private int pukenumber(int CardsID)
    {
        switch (CardsID)
        {
            case 0: case 1: case 2:case 3:case 4:case 5:case 6:case 7:return 15;//考虑到对比大小把它改成了15，在判定顺子时可以递归判定
            case 8: case 9: case 10: case 11: case 12: case 13: case 14: case 15: return 3;
            case 16: case 17: case 18: case 19: case 20: case 21: case 22: case 23: return 4;
            case 24: case 25: case 26: case 27: case 28: case 29: case 30: case 31: return 5;
            case 32: case 33: case 34: case 35: case 36: case 37: case 38: case 39: return 6;
            case 40: case 41: case 42: case 43: case 44: case 45: case 46: case 47: return 7;
            case 48: case 49: case 50: case 51: case 52: case 53: case 54: case 55: return 8;
            case 56: case 57: case 58: case 59: case 60: case 61: case 62: case 63: return 9;
            case 64: case 65: case 66: case 67: case 68: case 69: case 70: case 71: return 10;
            case 72: case 73: case 74: case 75: case 76: case 77: case 78: case 79: return 11;
            case 80: case 81: case 82: case 83: case 84: case 85: case 86: case 87: return 12;
            case 88: case 89: case 90: case 91: case 92: case 93: case 94: case 95: return 13;
            case 96: case 97: case 98: case 99: case 100: case 101: case 102: case 103: return 14;
            case 104: case 105: case 106: case 107: return 0;//大小王
            default: return -1;//错误ID
        }
    }
    private pukestate GetPukeType(in int[] getTypeSave)
    {
        int zerocount = 0;//记录0的数量
        for (int i = 0; i < getTypeSave.Length; i++)//去除重复的卡牌ID
        {
            getTypeSave[i] = pukenumber(getTypeSave[i]);
            if (getTypeSave[i] == 0) zerocount++;//记录0的数量
        }
        //在使用百搭时除了炸弹，要对ShowPukeType进行优先判定
        switch (getTypeSave.Length)
        {
            case 1:
                if (getTypeSave[0] != 0)
                    return pukestate.One;
                else
                    return pukestate.None;
            case 2:
                if (getTypeSave[0] == getTypeSave[1]
                    || zerocount==1)
                    return pukestate.Two;
                else
                    return pukestate.None;
                /*
            case 3:
                if (getTypeSave[0] == (getTypeSave[1])
                    && ((getTypeSave[1]) == (getTypeSave[2])|| (getTypeSave[2]) == 0))
                    return pukestate.Three;
                else if ((getTypeSave[0] == getTypeSave[1] - 1||getTypeSave[1]==0)
                    && (getTypeSave[1] == getTypeSave[2] - 1|| getTypeSave[2] == 0))
                    return pukestate.ThreeS;
                else
                    return pukestate.None;
            case 4:
                if (getTypeSave[0] == getTypeSave[1]
                    && getTypeSave[1] == getTypeSave[2]
                    && (getTypeSave[2] == getTypeSave[3]
                    || getTypeSave[3] == 0))
                    return pukestate.Boom;
                else if (getTypeSave[0] == getTypeSave[1]
                    && getTypeSave[1] != getTypeSave[2]
                    && (getTypeSave[2] == getTypeSave[3]
                    || getTypeSave[3] == 0))
                    return pukestate.TwoTwo;
                else if (getTypeSave[0] == getTypeSave[1 - 1]
                    && getTypeSave[1] == getTypeSave[2] - 1
                    && (getTypeSave[2] == getTypeSave[3] - 1
                    || getTypeSave[3] == 0))
                    return pukestate.FourS;
                else
                    return pukestate.None;
            case 5:
                if (getTypeSave[0] == getTypeSave[1]
                    && getTypeSave[1] == getTypeSave[2]
                    && getTypeSave[2] == getTypeSave[3])
                    return pukestate.BoomOne;
                return pukestate.ThreeTwo;
                return pukestate.FiveS;
                return pukestate.FiveBoom;
                return pukestate.None;
            case 6:
                return pukestate.SixBoom;
                return pukestate.SixS;
                return pukestate.TwoTwoTwo;
                return*/
           default: return pukestate.None;
        }

    }
    public void ShowCardsFromPrivate()
    {   //进行复杂的三重判定，第一重为玩家是否首发，第二重鉴定牌型是否正确，打出后游戏是否结束
        if (usingPN.text != Networking.LocalPlayer.displayName || GameOver) return;
        int forsavedlength = 0;
        for (int i = 0; i < 6; i++)
        {
            if (MySelectCardsID[i] != -1)
                forsavedlength++;
        }
        if (forsavedlength == 0) return;//如果没有选择卡牌则不进行任何操作
        int[] savedMineSelectCardID = new int[forsavedlength];
        forsavedlength = 0;
        for (int i = 0; i < 6; i++)
        {
            if (MySelectCardsID[i] != -1)
            {
                savedMineSelectCardID[forsavedlength] = MySelectCardsID[i];//存储玩家选择的卡牌ID
                forsavedlength++;
            }
        }
        for (int i = 0; i < savedMineSelectCardID.Length - 1; i++)//从小到大排序方便我们进行牌型判断
        {
            if (savedMineSelectCardID[i] > savedMineSelectCardID[i + 1])
            {
                int temp = savedMineSelectCardID[i];
                savedMineSelectCardID[i] = savedMineSelectCardID[i + 1];
                savedMineSelectCardID[i + 1] = temp;//对玩家选择的卡牌ID进行排序
            }
        }
        if (GetPukeType(savedMineSelectCardID) == pukestate.None) return;//如果牌型不合法则不进行任何操作
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        if (ShowPukePN[0] == "" || ShowPukePN[0] == Networking.LocalPlayer.displayName)
        {
            YouCanShow(savedMineSelectCardID, forsavedlength);
        }
        else//三重判定，正常判定对比卡牌，炸弹判定对比卡牌，超量炸弹判定对比卡牌
        {
            switch (GetPukeType(savedMineSelectCardID))
            {
                case pukestate.Boom:
                    if (ShowPukeType != pukestate.Boom)
                    {
                        switch (ShowPukeType)
                        {
                            case pukestate.FiveBoom:
                            case pukestate.SixBoom: return;//对方牌比较大，不让出
                            default: YouCanShow(savedMineSelectCardID, forsavedlength); break;
                        }
                        break;
                    }
                    else
                    {
                        if (pukenumber(savedMineSelectCardID[0]) > PlayersPukeNub[0])
                        {
                            YouCanShow(savedMineSelectCardID, forsavedlength);
                            break;
                        }
                        else return;
                    }
                case pukestate.FiveBoom:
                    if (ShowPukeType != pukestate.FiveBoom)
                    {
                        switch (ShowPukeType)
                        {
                            case pukestate.SixBoom: return;//对方牌比较大，不让出
                            default: YouCanShow(savedMineSelectCardID, forsavedlength); break;
                        }
                        break;
                    }
                    else
                    {
                        if (pukenumber(savedMineSelectCardID[0]) > PlayersPukeNub[0])
                        {
                            YouCanShow(savedMineSelectCardID, forsavedlength);
                            break;
                        }
                        else return;
                    }
                default: break;
            }//炸弹判定完成
            if (GetPukeType(savedMineSelectCardID) == ShowPukeType)
            {
                CanIShow(savedMineSelectCardID, forsavedlength);
            }
            else return;//如果牌型不相同则不进行任何操作
        }
        //最后判定游戏是否结束
        MineCardsCount -= forsavedlength;//更新玩家手牌数量记录
        if (MineCardsCount == -1) GameOver = true;//如果玩家手牌数为-1，则游戏结束
        else
        {
            UsingPNid = (UsingPNid + 1) % PlayersCount;//切换到下一个玩家
        }
        RequestSerialization();
        Setall();
    }
    private void CanIShow(int[] savedMineSelectCardID,int forsavedlength)
    {
        switch (ShowPukeType)
        {
            case pukestate.One:
            case pukestate.Two:
            case pukestate.Three:
            case pukestate.TwoTwo:
                if (pukenumber(savedMineSelectCardID[0]) == PlayersPukeNub[0] + 1
                    || (pukenumber(savedMineSelectCardID[0]) == 15 && PlayersPukeNub[0] != 15)
                    || (pukenumber(savedMineSelectCardID[0]) == 3 && PlayersPukeNub[0] == 15))
                {
                    YouCanShow(savedMineSelectCardID, forsavedlength);//如果牌型相同且玩家选择的牌比展示牌大1，则可以展示
                    break;
                }
                else return;//如果牌型相同但玩家选择的牌不比展示牌大1，则不进行任何操作
            case pukestate.ThreeTwo:
                int PlayersNubCountFor0 = 0, PlayersNubCountFor1 = 0, MyNubCountFor0 = 0, MyNubCountFor1 = 0;
                int savedPlayersNub0 = -1, savedPlayerNub1 = -1, savedMineNub0 = -1, savedMineNub1 = -1;
                PlayerAndMineNubSet(savedMineSelectCardID,
                    PlayersNubCountFor0, PlayersNubCountFor1, MyNubCountFor0, MyNubCountFor1,
                    savedPlayersNub0, savedPlayerNub1, savedMineNub0, savedMineNub1);
                SetSavedNubThreeTwo(MyNubCountFor0, MyNubCountFor1, savedMineNub0, savedMineNub1);
                SetSavedNubThreeTwo(PlayersNubCountFor0, PlayersNubCountFor1, savedPlayersNub0, savedPlayerNub1);
                if (MyNubCountFor1 == 0 && PlayersNubCountFor1 == 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayerNub1 == savedMineNub0 - 1
                        || savedPlayersNub0 == savedMineNub1 - 1 || savedPlayerNub1 == savedMineNub1 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;//如果玩家选择的牌不比展示牌大1，则不进行任何操作
                }
                else if (MyNubCountFor1 != 0 && PlayersNubCountFor1 == 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayerNub1 == savedMineNub0 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;
                }
                else if (MyNubCountFor1 == 0 && PlayersNubCountFor1 != 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayersNub0 == savedMineNub1 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;
                }
                else if (savedPlayersNub0 == savedMineNub0 - 1)
                {
                    YouCanShow(savedMineSelectCardID, forsavedlength);
                    break;
                }
                else return;
            case pukestate.BoomOne:
                PlayersNubCountFor0 = 0; PlayersNubCountFor1 = 0; MyNubCountFor0 = 0; MyNubCountFor1 = 0;
                savedPlayersNub0 = -1; savedPlayerNub1 = -1; savedMineNub0 = -1; savedMineNub1 = -1;
                PlayerAndMineNubSet(savedMineSelectCardID,
                    PlayersNubCountFor0, PlayersNubCountFor1, MyNubCountFor0, MyNubCountFor1,
                    savedPlayersNub0, savedPlayerNub1, savedMineNub0, savedMineNub1);
                SetSavedNubBoomOne(MyNubCountFor0, MyNubCountFor1, savedMineNub0, savedMineNub1);
                SetSavedNubBoomOne(PlayersNubCountFor0, PlayersNubCountFor1, savedPlayersNub0, savedPlayerNub1);
                if (MyNubCountFor1 == 0 && PlayersNubCountFor1 == 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayerNub1 == savedMineNub0 - 1
                        || savedPlayersNub0 == savedMineNub1 - 1 || savedPlayerNub1 == savedMineNub1 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;//如果玩家选择的牌不比展示牌大1，则不进行任何操作
                }
                else if (MyNubCountFor1 != 0 && PlayersNubCountFor1 == 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayerNub1 == savedMineNub0 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;
                }
                else if (MyNubCountFor1 == 0 && PlayersNubCountFor1 != 0)
                {
                    if (savedPlayersNub0 == savedMineNub0 - 1 || savedPlayersNub0 == savedMineNub1 - 1)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else return;
                }
                else if (savedPlayersNub0 == savedMineNub0 - 1)
                {
                    YouCanShow(savedMineSelectCardID, forsavedlength);
                    break;
                }
                else return;
            case pukestate.ThreeS:
            case pukestate.FourS:
            case pukestate.FiveS:
                int MineMinNub=99,MineMaxNub=0, MineZeroCount = 0,
                    PlayersMinNub=99,PlayersMaxNub=0,PlayersZeroCount=0;
                SetSavedNubTFFS(MineMinNub, MineMaxNub, MineZeroCount, PlayersMinNub, PlayersMaxNub, PlayersZeroCount, forsavedlength, savedMineSelectCardID);
                
            default: return;
        }
    }
    private void SetSavedNubTFFS(int MineMinNub, int MineMaxNub, int MineZeroCount, int PlayersMinNub, int PlayersMaxNub, int PlayersZeroCount,int forsavedlength, int[]savedMineSelectCardID)
    {
        for (int i = 0; i < forsavedlength; i++)
        {
            if (PlayersPukeNub[i] == 0) PlayersZeroCount++;
            else 
            {
                if (PlayersPukeNub[i] > PlayersMaxNub) PlayersMaxNub = PlayersPukeNub[i];
                if (PlayersPukeNub[i] < PlayersMinNub) PlayersMinNub = PlayersPukeNub[i];
            }
            if (pukenumber(savedMineSelectCardID[i]) == 0) MineZeroCount++;
            else
            {
                if (pukenumber(savedMineSelectCardID[i]) > MineMaxNub) MineMaxNub = pukenumber(savedMineSelectCardID[i]);
                if (pukenumber(savedMineSelectCardID[i]) < MineMinNub) MineMinNub = pukenumber(savedMineSelectCardID[i]);
            }
        }
        if(MineMaxNub-MineMinNub>forsavedlength-1)
    }
    private void SetSavedNubThreeTwo(int NubCountFor0, int NubCountFor1, int savedNub0, int savedNub1)
    {
        switch (NubCountFor0, NubCountFor1)
        {
            case (0, 3): case (1, 3): break;
            case (0, 2):case (1, 1):
                int temp = savedNub0;
                savedNub0 = savedNub1;
                savedNub1 = temp;
                break;
            case (1, 2):case (2, 2):case (2, 1):
                savedNub0 = 0; break;
            default:break;
        }
    }
    private void SetSavedNubBoomOne(int NubCountFor0, int NubCountFor1, int savedNub0, int savedNub1)
    {
        switch (NubCountFor0, NubCountFor1)
        {
            case (0, 4): case (1, 3): case (2, 2): break;
            case (0, 1):case(1,1):case(2,1):
                int temp = savedNub0;
                savedNub0 = savedNub1;
                savedNub1 = temp;
                break;
            case (3, 1):
                savedNub0=0; 
                break;
            default: break;
        }
    }
    private void PlayerAndMineNubSet(int[] savedMineSelectCardID,
        int PlayersNubCountFor0, int PlayersNubCountFor1,
        int MyNubCountFor0, int MyNubCountFor1,
        int savedPlayersNub1, int savedPlayerNub2,
        int savedMineNub1, int savedMineNub2)
    {
        for (int i = 0; i < 5; i++)
        {
            if (PlayersPukeNub[i] == 0) PlayersNubCountFor0++;
            else if (PlayersPukeNub[i] == savedPlayersNub1 || savedPlayersNub1 == -1)
            {
                PlayersNubCountFor1++; savedPlayersNub1 = PlayersPukeNub[i];
            }
            else { savedPlayerNub2 = PlayersPukeNub[i]; }
            if (pukenumber(savedMineSelectCardID[i]) == 0) MyNubCountFor0++;
            else if (pukenumber(savedMineSelectCardID[i]) == savedMineNub1 || savedMineNub1 == -1)
            {
                MyNubCountFor1++; savedMineNub1 = pukenumber(savedMineSelectCardID[i]);
            }
            else { savedMineNub2 = pukenumber(savedMineSelectCardID[i]); }
        }
    }
    private void YouCanShow(int[] savedMineSelectCardID,int forsavedlength)
    {
        int savedcount = 0;
        foreach (int i in savedMineSelectCardID)
        {
            PlayersPukeNub[savedcount]= pukenumber(i);
            savedcount++;
        }
        for (int i = 5; i >= 0; i--)
        {
            if (i < forsavedlength)
            {
                ShowPukeID[i] = savedMineSelectCardID[i];
                ShowPukePN[i] = Networking.LocalPlayer.displayName;
            }
            else
            {
                ShowPukeID[i] = ShowPukeID[i - 1];
                ShowPukePN[i] = ShowPukePN[i - 1];
            }
        }
        ShowPukeType = GetPukeType(savedMineSelectCardID);//获取展示牌类型
    }
    public void PassMyTurn()
    {   //漏了一个判定，当下一位玩家是上一个打牌者时，给他加一张
        if (usingPN.text != Networking.LocalPlayer.displayName || GameOver) return;
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        UsingPNid = (UsingPNid + 1) % PlayersCount;//切换到下一个玩家
        if (PlayersName[UsingPNid] == playersNameforCard[0].text)
        {
            IsAdd = true;
            Cardscount--;
            if (Cardscount == -2)
            {
                SetCardRandom();
                Cardscount = 106;
                PlayersCardCount[UsingPNid]++;
            }
        }
        else
        {
            IsAdd = false;
        }
        RequestSerialization();
        Setall();
    }
    public void JoinGame()
    {
        if (!GameOver) return;
        bool isjoin = false;
        for (int i = 0; i < 8; i++)
        {
            if (PlayersName[i] == "")
            {
                PlayersName[i] = Networking.LocalPlayer.displayName;
                isjoin = true;
                if (i == 0) UsingPNid = 0;
                break;
            }
        }
        if (!Networking.IsOwner(GetOwn(), gameObject)||!isjoin) return;
        JSbutton[0].SetActive(false);//隐藏加入游戏按钮
        JSbutton[2].SetActive(false);
        JSbutton[1].SetActive(true);//显示开始游戏按钮
        JSbutton[3].SetActive(true);
        PlayersCount++;
        RequestSerialization();
        Setall();
    }
    private void SetCardRandom()
    {
        for (int i = 0; i < 108; i++)
        {
            int randomIndex = Random.Range(i, AllCardsID.Length);
            int temp = AllCardsID[i];
            AllCardsID[i] = AllCardsID[randomIndex];
            AllCardsID[randomIndex] = temp;
        }
    }
    public void SetNewGame()
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        SetCardRandom();
        GameOver = false;
        string[] CPPlayersName = new string[8];
        for (int i = 0; i < 8; i++)//重设玩家信息
        {
            CPPlayersName[i] = "";
        }
        int setresetint = UsingPNid;
        int playercount = 0;
        for (int i = 0; i < 8; i++)//重设玩家序列
        {
            CPPlayersName[i] = PlayersName[setresetint];
            if (setresetint + 1 < 8 && PlayersName[setresetint + 1] != "")
            {
                setresetint++;
            }
            else if (setresetint + 1 >= 8)
            {
                setresetint = 0;
            }
            else if (PlayersName[setresetint + 1] == "")
            {
                i += (8 - setresetint);
                setresetint = 0;
            }
            playercount++;
        }
        PlayersName = CPPlayersName;
        for (int i = 0; i < 8; i++)
        {
            if (PlayersName[i] == "") break;
            PlayersCardCount[i] = 5;
        }
        PlayersCardCount[0] = 6;
        IsNewGame = true;
        Cardscount = 107 - 6 - 5 * playercount;
        UsingPNid = 0;
        IsAdd = false;
        RequestSerialization();
        Setall();
    }//重置游戏数据
    public void ResetAll()
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        for (int i = 0; i < 8; i++)
        {
            PlayersName[i]="";
            PlayersScore[i] = 100;
            PlayersCardCount[i] = 0;
        }
        PlayersPukeNub = new int[0];
        GameOver = true;
        usingPN.text = "";
        for (int i = 0; i < 6; i++)
        {
            minecards[i].sprite = nullimage.sprite;
            minecards[i].color = Color.white;
            showcards[i].sprite = nullimage.sprite;
            playersNameforCard[i].text = "";
        }
        ShowPukeID=new int[6] { -1, -1, -1, -1, -1, -1 };
        MineCardsCount = 0;
        MyCardsID = new int[6] { -1, -1, -1, -1, -1, -1 };
        MySelectCardsID = new int[6] { -1,-1,-1,-1,-1,-1 };//初始化玩家选择的卡牌ID
        IsNewGame=false;
        ResetButton = true;
        PlayersCount = 0;
        RequestSerialization();
        Setall();
    }
    public override void OnDeserialization()//同步数据后调用
    {
        Setall();
    }
    private void Setall()
    {
        if (GameOver)//游戏结束后的分数计算
        {
            int winplayer = 0;
            for (; winplayer < 8; winplayer++)//寻找赢家
            {
                if (PlayersCardCount[winplayer]==0)
                {
                    break;
                }
            }
            for (int i = 0; i < 8; i++)
            {
                PlayersScore[i] -= PlayersCardCount[i];//根据剩余卡牌数扣分
                PlayersScore[winplayer] += PlayersCardCount[i];//给赢家加分
                if (PlayersCardCount[i] == 5)//被春天后额外扣分加分
                {
                    PlayersScore[i] -= 5;
                    PlayersScore[winplayer] += 5;
                }
            }
        }
        if (IsNewGame)//新游戏开始时重置玩家手牌
        {
            for (int i=0;i<8;i++) 
            {
                if (Networking.LocalPlayer.displayName == PlayersName[i])
                {
                    if (i == 0)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            minecards[j].sprite = allpuke[AllCardsID[107 - j]].sprite;
                            MyCardsID[j] = AllCardsID[107 - j];//存储玩家手牌ID
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            minecards[j].sprite = allpuke[AllCardsID[107 - 5 * i - j]].sprite;
                            MyCardsID[j] = AllCardsID[107 - 5 * i - j];//存储玩家手牌ID
                        }
                    }
                }
            }
            IsNewGame = false;
        }
        if (ResetButton)
        {
            JSbutton[0].SetActive(true);//显示加入游戏按钮
            JSbutton[1].SetActive(false);//隐藏开始游戏按钮
            JSbutton[2].SetActive(true);
            JSbutton[3].SetActive(false);
            ResetButton = false;
        }
        if (IsAdd)//当加牌时
        {
            if (PlayersName[UsingPNid] == Networking.LocalPlayer.displayName)
            {
                MineCardsCount=PlayersCardCount[UsingPNid];//更新玩家手牌数量记录
                MyCardsID[MineCardsCount] = AllCardsID[Cardscount + 1];
                minecards[MineCardsCount].sprite = allpuke[MyCardsID[MineCardsCount]].sprite;//更新玩家手牌显示
            }
        }
        usingPN.text = PlayersName[UsingPNid];//更新当前使用的玩家名称
        SetInGamePlayer();//更新玩家列表信息
        Setshowcards();//更新显示卡牌信息
        CopyToWorld();
    }
    private void SetInGamePlayer()
    {
        for (int i = 0; i < 8; i++)
        {
            playersInGame[i].text = PlayersName[i];
            playersScore[i].text = PlayersScore[i].ToString();
            playersCardCount[i].text = PlayersCardCount[i].ToString();
        }
    }
    private void Setshowcards()
    {
        for (int i = 0; i < 6; i++)
        {
            if (ShowPukeID[i] == -1)
            {
                showcards[i].sprite = nullimage.sprite;
            }
            else
            {
                showcards[i].sprite = allpuke[ShowPukeID[i]].sprite;
            }
            playersNameforCard[i].text=ShowPukePN[i];
        }
    }
    private void CopyToWorld()
    {
        int i;
        for (i = 0; i < playersInGame.Length; i++)
        {
            copytoPlayersInGame[i].text = playersInGame[i].text;
            copytoPlayersScore[i].text = playersScore[i].text;
            copytoPlayersCardCount[i].text=playersCardCount[i].text;
        }
        for (i = 0; i < minecards.Length; i++)
        {
            copytoMinecards[i].sprite = minecards[i].sprite;
            copytoShowcards[i].sprite = showcards[i].sprite;
            copytoPlayersNameforCard[i].text = playersNameforCard[i].text;
        }
        copytoUingPN.text = usingPN.text;
    }
    private VRCPlayerApi GetOwn()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        return Networking.LocalPlayer;
    }
    private void SetminecardForSelect(int ForSelectMineCardID)
    {
        if (GameOver) return;
        if (MyCardsID[ForSelectMineCardID] == MySelectCardsID[ForSelectMineCardID])
        {
            MySelectCardsID[ForSelectMineCardID] = -1;
            minecards[ForSelectMineCardID].color = Color.white;//取消选择时恢复颜色
            copytoMinecards[ForSelectMineCardID].color = Color.white;
            MineCardsCount--;
        }//如果选择的卡牌ID和手牌ID相同，则取消选择
        else
        {
            MySelectCardsID[ForSelectMineCardID] = MyCardsID[ForSelectMineCardID];
            minecards[ForSelectMineCardID].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);//选择时变为黄色
            copytoMinecards[ForSelectMineCardID].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);
            MineCardsCount++;
        }
    }
    public void Selectminecard0() { SetminecardForSelect(0); }
    public void Selectminecard1() { SetminecardForSelect(1); }
    public void Selectminecard2() { SetminecardForSelect(2); }
    public void Selectminecard3() { SetminecardForSelect(3); }
    public void Selectminecard4() { SetminecardForSelect(4); }
    public void Selectminecard5() { SetminecardForSelect(5); }
}
