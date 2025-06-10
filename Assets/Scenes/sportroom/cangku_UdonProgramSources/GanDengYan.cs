
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

enum pukestate { None,One,Two,Three, TwoTwo,ThreeTwo,BoomOne,Boom,ThreeS,FourS,FiveS,FiveBoom,SixBoom,}
public class GanDengYan : UdonSharpBehaviour
{
    //玩家手牌和展示牌相关的UI元素
    public Image[] minecards;
    public Image[] copytoMinecards;
    public Image[] showcards;
    public Image[] copytoShowcards;
    public TextMeshProUGUI[] playersNameforCard;
    public TextMeshProUGUI[] copytoPlayersNameforCard;
    //玩家信息相关的UI元素
    public TextMeshProUGUI[] playersInGame;
    public TextMeshProUGUI[] copytoPlayersInGame;
    public TextMeshProUGUI[] playersScore;
    public TextMeshProUGUI[] copytoPlayersScore;
    public TextMeshProUGUI[] playersCardCount;
    public TextMeshProUGUI[] copytoPlayersCardCount;
    /// <summary>
    /// 存储所有卡牌的外部图片数组
    /// </summary>
    public Image[] allpuke;
    public TextMeshProUGUI usingPN;//当前使用的玩家名称
    public TextMeshProUGUI copytoUingPN;
    public Image nullimage;
    /// <summary>玩家列表名称</summary>
    [UdonSynced] private string[] PlayersName = new string[8];
    /// <summary>玩家列表分数</summary>
    [UdonSynced] private int[] PlayersScore = new int[8];
    /// <summary>玩家手牌数量</summary>
    [UdonSynced] private int[] PlayersCardCount = new int[8];
    /// <summary>玩家数量</summary>
    [UdonSynced] private int PlayersCount = 0;
    /// <summary>用于存储玩家手牌ID</summary>
    private int[] MyCardsID = new int[6];
    /// <summary>用于存储玩家选择的卡牌ID</summary>
    private int[] MySelectCardsID = new int[6];
    /// <summary>所有卡牌ID,用于乱序索引，id指向allpuke数组</summary>
    [UdonSynced] private int[] AllCardsID = new int[108];
    /// <summary>剩余卡牌数</summary>
    [UdonSynced] private int Cardscount = 107;
    /// <summary>玩家吐牌数字，用作对比,动态变化</summary>
    [UdonSynced] private int[] PlayersPukeNub = new int[0];
    /// <summary>展示牌ID索引，索引到allpuke数组</summary>
    [UdonSynced] private int[] ShowPukeID = new int[6];
    /// <summary>展示牌类型</summary>
    [UdonSynced] private pukestate ShowPukeType;
    /// <summary>展示牌的玩家名称</summary>
    [UdonSynced] private string[] ShowPukePN = new string[6];
    /// <summary>当前使用的玩家名称在 PlayersName 数组中的索引</summary>
    [UdonSynced] private int UsingPNid;
    /// <summary>用于锁定出牌按钮</summary>
    [UdonSynced] private bool GameOver = true;
    /// <summary>判断新游戏初始化</summary>
    [UdonSynced] private bool IsNewGame = false;
    /// <summary>判断加牌逻辑</summary>
    [UdonSynced] private bool IsAdd = false;
    /// <summary>对玩家手牌进行初始化</summary>
    [UdonSynced] private bool Reset = false;
    //以上是游戏数据相关的变量
    public GameObject[] JSbutton;
    void Start()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        for (int i = 0; i < 108;)
        {
            AllCardsID[i] = i++;
        }
        ResetAll();
    }
    private int pukenumber(int CardsID)
    {
        switch (CardsID)
        {
            case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7: return 15;//考虑到对比大小把它改成了15，在判定顺子时可以递归判定
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
    
    private pukestate GetPukeType(in int[] getTypeSave,pukestate PKstate)
    {
        int zerocount = 0;//记录0的数量
        int nubtypecount = 1;//记录数的数量
        int minusintcount = 0;//记录顺子差值合计值
        int[] getspukenumber = new int[getTypeSave.Length];//获取卡牌ID对应的数字的数组
        for (int i = 0; i < getTypeSave.Length; i++)
        {
            getspukenumber[i] = pukenumber(getTypeSave[i]);
            if (getspukenumber[i] == 0) zerocount++;//记录0的数量
            else if (i > 0)
            {
                if (getspukenumber[i] != getspukenumber[i - 1])
                {
                    nubtypecount++;
                }
            }//记录数的数量
        }
        //如果首位是2，我们重新排序一下，保证顺子判定的正确性
        if (getspukenumber[0] == 15 && getspukenumber.Length > 1)
        {
            for (int i = 0; i < getspukenumber.Length - 1 - zerocount; i++)
                if (getspukenumber[i] < 9) getspukenumber[i] += 13;//加倍判定长度，将比较范围重置到10-24
            for (int i = 0; i < getspukenumber.Length - 1-zerocount; i++)
            {
                for (int j = 0; j < getspukenumber.Length - 1 - zerocount - i; j++)
                {
                    if (getspukenumber[j] > getspukenumber[j + 1])
                    {
                        int temp = getspukenumber[j];
                        getspukenumber[j] = getspukenumber[j + 1];
                        getspukenumber[j + 1] = temp;//对获取的卡牌数字进行排序
                    }
                }
            }
        }
        //在使用百搭时除了炸弹，要对ShowPukeType进行优先判定
        switch (getspukenumber.Length)
        {
            case 1://只有单牌
                if (zerocount != 1)
                    return pukestate.One;
                return pukestate.None;
            case 2://只有对子
                if (nubtypecount == 1 && zerocount != 2)
                    return pukestate.Two;
                return pukestate.None;
            case 3://三张一样的或顺子
                if (PKstate == pukestate.Three || PKstate == pukestate.None)
                    if (nubtypecount == 1 && zerocount != 3)
                        return pukestate.Three;
                for (int i = 0; i < getspukenumber.Length - zerocount - 1; i++)//顺子逻辑的四行
                {
                    minusintcount += (getspukenumber[i + 1] - getspukenumber[i]);
                }//记录顺子差值合计值
                if (minusintcount <= getspukenumber.Length-1 && nubtypecount == getspukenumber.Length - zerocount)
                    return pukestate.ThreeS;
                return pukestate.None;
            case 4://炸弹、顺子、连对、
                if (nubtypecount == 1 && zerocount != 4)
                    return pukestate.Boom;
                for (int i = 0; i < getspukenumber.Length - zerocount - 1; i++)//顺子逻辑的四行
                    minusintcount += (getspukenumber[i + 1] - getspukenumber[i]);//记录顺子差值合计值
                if (PKstate == pukestate.TwoTwo || PKstate == pukestate.None)
                    if (nubtypecount == 2 && minusintcount == 1)
                        return pukestate.TwoTwo;
                if (minusintcount <= getspukenumber.Length-1 && nubtypecount == getspukenumber.Length - zerocount)
                    return pukestate.FourS;
                return pukestate.None;
            case 5://炸弹、四带一、三带二、顺子
                if (nubtypecount == 1)
                    return pukestate.FiveBoom;//如果有炸弹则直接返回炸弹类型
                if (nubtypecount == 2)
                {
                    if (PKstate == pukestate.BoomOne || PKstate == pukestate.None)
                    {
                        int samcount = 1, samcount2 = 1;
                        for (int i = 0; i < 4 - zerocount; i++)
                        {
                            if (getspukenumber[i] == getspukenumber[i + 1])
                                samcount++;
                            else
                            {
                                samcount2 = samcount;
                                samcount = 0;
                            }
                        }
                        if (samcount2 > samcount)
                            samcount = samcount2;//获取最大相同数
                        if (samcount == 4 - zerocount)
                            return pukestate.BoomOne;//如果有四带一则返回四带一类型
                    }
                    if(PKstate!=pukestate.FiveS)
                        return pukestate.ThreeTwo;//如果有三带二则返回三带二类型
                }
                for (int i = 0; i < getspukenumber.Length - zerocount - 1; i++)//顺子逻辑的四行
                    minusintcount += (getspukenumber[i + 1] - getspukenumber[i]);//记录顺子差值合计值
                if (minusintcount <= getspukenumber.Length-1 && nubtypecount == getspukenumber.Length - zerocount)
                    return pukestate.FiveS;
                return pukestate.None;//如果没有则返回无效牌型
            case 6://炸弹、五带一、四带对、飞机、三连对、顺子、
                if (nubtypecount == 1)
                    return pukestate.SixBoom;
                if (nubtypecount == 2)//这边判定除了0以外只有两个数
                    return pukestate.SixBoom;//顺子判定
                for (int i = 0; i < getspukenumber.Length - zerocount - 1; i++)//顺子逻辑的四行
                    minusintcount += (getspukenumber[i + 1] - getspukenumber[i]);//记录顺子差值合计值
                if (nubtypecount == 3 && minusintcount == 3)//三连对判定
                    return pukestate.SixBoom;
                if (minusintcount <= getspukenumber.Length-1 && nubtypecount == getspukenumber.Length - zerocount)
                    return pukestate.SixBoom;
                return pukestate.None;//如果没有炸弹则返回无效牌型
            default: return pukestate.None;
        }

    }
    public void ShowCardsFromPrivate()
    {   //进行复杂的三重判定，第一重为玩家是否首发，第二重鉴定牌型是否正确，打出后游戏是否结束
        if (PlayersName[UsingPNid] != Networking.LocalPlayer.displayName || GameOver) return;//判定是否是自己操作与游戏结束
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        int forsavedlength = 0;//用于获取出牌长度
        for (int i = 0; i < 6; i++)
        {
            if (MySelectCardsID[i] != -1)
                forsavedlength++;
        }
        if (forsavedlength == 0) return;//如果没有选择卡牌则不进行任何操作
        int[] savedMineSelectCardID = new int[forsavedlength];
        forsavedlength = 0;
        foreach(int i in MySelectCardsID)
            if (i != -1)
            {
                savedMineSelectCardID[forsavedlength] = i;//存储玩家选择的卡牌ID
                forsavedlength++;
            }
        for (int i = 0; i < savedMineSelectCardID.Length - 1; i++)//按照id重新排序，0在最后2在最前
        {
            for (int j = 0; j < savedMineSelectCardID.Length - 1 - i; j++)
            {
                if (savedMineSelectCardID[j] > savedMineSelectCardID[j + 1])
                {
                    int temp = savedMineSelectCardID[j];
                    savedMineSelectCardID[j] = savedMineSelectCardID[j + 1];
                    savedMineSelectCardID[j + 1] = temp;
                }
            }
        }
        pukestate MinePukeState = GetPukeType(savedMineSelectCardID,pukestate.None);//获取玩家选择的牌型
        if (MinePukeState == pukestate.None)
        {   //如果牌型不合法则初始化选牌
            for (int i = 0; i < 6; i++)
            {
                MySelectCardsID[i] = -1;
                minecards[i].color = Color.white;//取消选择时恢复颜色
                copytoMinecards[i].color = Color.white;
            }
            return;
        }
        if (ShowPukePN[0] == "" || ShowPukePN[0] == Networking.LocalPlayer.displayName)
        {   //首先判断自己是否首发，以及上一次出牌是否是自己
            ShowPukeType = MinePukeState;
        }
        else//三重判定，优先判定炸弹，其次正常判定对比卡牌
        {
            switch (MinePukeState)
            {
                case pukestate.Boom:
                    if (ShowPukeType != pukestate.Boom && ShowPukeType!=pukestate.FiveBoom)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength); 
                        break;
                    }
                    else if(ShowPukeType==pukestate.Boom)
                    {
                        if (pukenumber(savedMineSelectCardID[0]) ==15|| pukenumber(savedMineSelectCardID[0])== PlayersPukeNub[0]+1||(pukenumber(savedMineSelectCardID[0])==3 && PlayersPukeNub[0]==15))
                        {
                            YouCanShow(savedMineSelectCardID, forsavedlength);
                            break;
                        }
                    }
                    return;//如果对方牌比较大则不让出(判定了对方是4炸或5炸的情况与2的情况
                case pukestate.FiveBoom:
                    if (ShowPukeType != pukestate.FiveBoom)
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    else if (pukenumber(savedMineSelectCardID[0]) == 15 || pukenumber(savedMineSelectCardID[0]) == PlayersPukeNub[0] + 1 || (pukenumber(savedMineSelectCardID[0]) == 3 && PlayersPukeNub[0] == 15))
                    {
                        YouCanShow(savedMineSelectCardID, forsavedlength);
                        break;
                    }
                    return;
                default: break;
            }//炸弹判定完成
            MinePukeState = GetPukeType(savedMineSelectCardID, ShowPukeType);//重新获取玩家选择的牌型
            if (MinePukeState == ShowPukeType)
            {
                if(!CanIShow(savedMineSelectCardID, forsavedlength))return;
            }
            else return;//如果牌型不相同则不进行任何操作
        }
        YouCanShow(savedMineSelectCardID, forsavedlength);
        //最后判定游戏是否结束，如果玩家手牌数为0，则游戏结束并计算分数
        PlayersCardCount[UsingPNid] -= forsavedlength;//更新玩家手牌数量记录
        if (PlayersCardCount[UsingPNid] == 0)
        {
            GameOver = true;//如果玩家手牌数为0，则游戏结束
            for (int i = 0; i < 8; i++)
            {
                if (PlayersCardCount[i] == 5)//被春天后的分数计算
                {
                    PlayersScore[i] -= 10;
                    PlayersScore[UsingPNid] += 10;
                }
                else
                {
                    PlayersScore[i] -= PlayersCardCount[i];//根据剩余卡牌数扣分
                    PlayersScore[UsingPNid] += PlayersCardCount[i];//给赢家加分
                }
            }
        }
        else
            UsingPNid = (UsingPNid + 1) % PlayersCount;//切换到下一个玩家
        IsNewGame = false;
        IsAdd = false;
        RequestSerialization();
        for (int i = 0; i < 6; i++)//清除玩家手牌中打出牌的显示
        {
            if (MySelectCardsID[i] != -1)
            {
                MyCardsID[i] = -2;
                MySelectCardsID[i] = -1;//初始化自身状态
            }
        }
        for (int i = 0; i < 5; i++)//清除玩家手牌中打出牌的显示
        {
            for (int j = 0; j < 5-i; j++)
            {
                if(MyCardsID[j] == -2)//如果有重复的卡牌则将其置为-2
                {
                    MyCardsID[j] = MyCardsID[j+1];
                    MyCardsID[j + 1] = -2;//将重复的卡牌置为-2
                }
            }
        }
        Setall();
    }
    private bool CanIShow(int[] savedMineSelectCardID, int forsavedlength)
    {
        int MyZeroCount = 0, PlayerZeroCount = 0;//记录0的数量
        int[] getMyPukeNum = new int[forsavedlength];//获取卡牌ID对应的数字的数组
        int[] getPlayerPukeNum = PlayersPukeNub;
        for (int i = 0; i < forsavedlength; i++)
        {
            getMyPukeNum[i] = pukenumber(savedMineSelectCardID[i]);
            if (getMyPukeNum[i] == 0) MyZeroCount++;//记录0的数量
            if (getPlayerPukeNum[i]==0) PlayerZeroCount++;
        }//给获取卡牌数字赋值，并记录两组卡的0的数量和数的数量
        //如果首位是2，我们重新排序一下，保证顺子判定的正确性
        ReSetForPukeNum(ref getMyPukeNum, MyZeroCount,forsavedlength);
        ReSetForPukeNum(ref getPlayerPukeNum, PlayerZeroCount, forsavedlength);
        int minusIntCount = 0;//记录顺子差值合计值
        switch (ShowPukeType)
        {
            case pukestate.One:
            case pukestate.Two:
            case pukestate.Three:
            case pukestate.TwoTwo:
                minusIntCount = getMyPukeNum[0] - getPlayerPukeNum[0];
                if (minusIntCount == 1 || minusIntCount == -12 || (getMyPukeNum[0] == 15 && ShowPukeType != pukestate.TwoTwo&& PlayersPukeNub[0] !=15))//如果玩家打出的牌比上家大1
                {
                    return true;
                }
                break;
            case pukestate.ThreeS:
            case pukestate.FourS:
            case pukestate.FiveS:
                minusIntCount = forsavedlength - 1 - (getMyPukeNum[forsavedlength - 1 - MyZeroCount] - getMyPukeNum[0]);//记录顺子差值合计值
                int mymin = getMyPukeNum[0] - minusIntCount;//获取玩家打出的牌的最小值
                minusIntCount = forsavedlength - 1 - (getPlayerPukeNum[forsavedlength - 1 - PlayerZeroCount] - getPlayerPukeNum[0]);//记录顺子差值合计值
                int playersmin = getPlayerPukeNum[0] - minusIntCount;//获取场上puke判定的最小值
                if (mymin >= playersmin + 1 && getMyPukeNum[0] <= getPlayerPukeNum[0] + 1) return true;//如果玩家打出的牌比上家大1
                break;
            case pukestate.BoomOne:
            case pukestate.ThreeTwo://需要记录哪个数的数量更多，然后根据数量更多的数来进行比较

                int[] mynub,playernub;
                int MyIntType, PlayerIntType;//记录玩家和对手的最大类型
                if (MyZeroCount > 1) mynub = new[] { getMyPukeNum[0], getMyPukeNum[4 - MyZeroCount] };
                else
                {
                    MyIntType = GetMaxType(getMyPukeNum,MyZeroCount);
                    if (MyIntType == -1) mynub = new[] { getMyPukeNum[0], getMyPukeNum[4 - MyZeroCount] };
                    else mynub = new[] { getMyPukeNum[MyIntType] };
                }
                if (PlayerZeroCount > 1) playernub = new[] { getPlayerPukeNum[0], getPlayerPukeNum[4 - PlayerZeroCount] };
                else
                {
                    PlayerIntType = GetMaxType(getPlayerPukeNum,PlayerZeroCount);
                    if (PlayerIntType == -1) playernub = new[] { getPlayerPukeNum[0], getPlayerPukeNum[4 - PlayerZeroCount] };
                    else playernub = new[] { getPlayerPukeNum[PlayerIntType] };
                }

                if (mynub.Length == 2)
                {
                    if (playernub.Length == 2)
                        if (mynub[0] == playernub[0] + 1 || mynub[0] == playernub[1] + 1
                             || mynub[1] == playernub[0] + 1 || mynub[1] == playernub[1] + 1
                             ) return true;
                        else return false;
                    else
                    {
                        if (mynub[0] == playernub[0] + 1 || mynub[1] == playernub[0] + 1
                            ) return true;
                    }
                }
                else
                {
                    if (playernub.Length == 2)
                    {
                        if (mynub[0] == playernub[0] + 1 || mynub[0] == playernub[0] + 1
                            ) return true;
                    }
                    else
                    {
                        if (mynub[0] == playernub[0] + 1)return true;
                    }
                }
                break;
                    default: return false;
        }
        return false;
    }
    private int GetMaxType(int[] getPukeNum,int zerocount)
    {
        int samcount = 1, samcount2 = 1;//记录相同数
        for (int i = 0; i < 3; i++)
        {
            if (getPukeNum[i + 1] != 0)
                if (getPukeNum[i] == getPukeNum[i + 1])
                    samcount++;
                else
                {
                    samcount2 = samcount;
                    samcount = 0;
                }
            if (i == 3)
            {
                if (samcount > samcount2) samcount = 4-zerocount;
                else if (samcount2 > samcount) samcount = 0;
                else samcount = -1;
            }
        }
        return samcount;
    }
    /// <summary>
    /// 对获取的卡牌数字进行排序，重置顺子判定的范围
    /// </summary>
    private void ReSetForPukeNum(ref int[] getPukeNum, int ZeroCount, int forsavedlength) {
        if (getPukeNum[0] == 15 && forsavedlength > 1)
        {
            for (int i = 0; i < forsavedlength - 1 - 5; i++)
            {
                if (getPukeNum[i] < 9) getPukeNum[i] += 13;//加倍判定长度，将比较范围重置到10-24
            }
            for (int i = 0; i < forsavedlength - 1 - ZeroCount; i++)
            {
                for (int j = 0; j < forsavedlength - 1 - ZeroCount - i; j++)
                {
                    if (getPukeNum[j] > getPukeNum[j + 1])
                    {
                        int temp = getPukeNum[j];
                        getPukeNum[j] = getPukeNum[j + 1];
                        getPukeNum[j + 1] = temp;//对获取的卡牌数字进行排序
                    }
                }
            }
        }
    }
    /// <summary>
    /// 修改PlayersPukeNub和ShowPukeID与ShowPukePN的值
    /// </summary>
    /// <param name="savedMineSelectCardID">操作的存储数组</param>
    /// <param name="forsavedlength">卡牌长度</param>
    private void YouCanShow(int[] savedMineSelectCardID,int forsavedlength)
    {
        int savedcount = 0;
        PlayersPukeNub = new int[savedMineSelectCardID.Length];
        foreach (int i in savedMineSelectCardID)//存储出牌的数字
        {
            PlayersPukeNub[savedcount]= pukenumber(i);
            savedcount++;
        }
        for (int i = 5; i >= 0; i--)//排序ShowPuke
        {
            if (forsavedlength>i )
            {
                ShowPukeID[i] = savedMineSelectCardID[i];
                ShowPukePN[i] = Networking.LocalPlayer.displayName;
            }
            else
            {
                ShowPukeID[i] = ShowPukeID[i - forsavedlength];
                ShowPukePN[i] = ShowPukePN[i - forsavedlength];
            }
        }
    }
    public void PassMyTurn()
    {
        //当我是上一个打出者、我不是当前操作玩家、游戏结束时不进行任何操作
        if (ShowPukePN[0] == Networking.LocalPlayer.displayName
            ||PlayersName[UsingPNid] != Networking.LocalPlayer.displayName || GameOver) return;
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        UsingPNid = (UsingPNid + 1) % PlayersCount;//切换到下一个玩家
        if (PlayersName[UsingPNid] == ShowPukePN[0])//当下一位玩家是上一个打牌者时，给他加一张
        {
            IsAdd = true;
            Cardscount--;
            PlayersCardCount[UsingPNid]++;
            if (Cardscount == -2)
            {
                SetCardRandom();
                Cardscount = 106;
            }
        }
        else
            IsAdd = false;
        RequestSerialization();
        Setall();
    }
    public void JoinGame()
    {
        if (!GameOver||PlayersCount==8) return;
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        PlayersName[PlayersCount] = Networking.LocalPlayer.displayName;
        PlayersCount++;
        RequestSerialization();
        Setall();
    }
    private void SetCardRandom()
    {
        for (int i = 0; i < 108; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, AllCardsID.Length);
            int temp = AllCardsID[i];
            AllCardsID[i] = AllCardsID[randomIndex];
            AllCardsID[randomIndex] = temp;
        }
    }
    public void SetNewGame()
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        SetCardRandom();//针对AllCardsID随机洗牌
        string[] CPPlayersName = new string[8] {"", "", "", "", "", "", "", "",};//用于重新排序玩家
        int setresetint = UsingPNid;
        for (int i = 0; i < PlayersCount; i++)//重设玩家序列
        {
            CPPlayersName[i] = PlayersName[setresetint];
            setresetint = (setresetint+1)%8;//防止溢出
            if (PlayersName[setresetint] == "")
            {
                setresetint = 0;
            }
        }
        PlayersName = CPPlayersName;
        for (int i = 0; i < PlayersCount; i++)
        {
            if (PlayersName[i] == "") break;
            PlayersCardCount[i] = 5;
        }
        PlayersCardCount[0] = 6;//设置玩家手牌数量显示
        Cardscount = 107 - 6 - 5 * PlayersCount;//设置卡牌书
        PlayersPukeNub = new int[0];
        ShowPukeID = new[] { -1, -1, -1, -1, -1, -1 };
        ShowPukeType = 0;
        ShowPukePN = new[] { "", "", "", "", "", "" };//初始化展示牌的四变量
        UsingPNid = 0;
        GameOver = false;
        IsNewGame = true;
        IsAdd = false;
        Reset = false;
        RequestSerialization();
        Setall();
    }//重置游戏数据
    public void ResetAll()
    {
        if (!Networking.IsOwner(GetOwn(), gameObject)) return;
        for (int i = 0; i < 8; i++)//初始化玩家列表变量
        {
            PlayersName[i]="";
            PlayersScore[i] = 100;
            PlayersCardCount[i] = 0;
        }
        UsingPNid = 0;//重置使用玩家ID
        PlayersCount = 0;
        PlayersPukeNub = new int[0];//初始化玩家打出的牌
        for (int i = 0; i < 6; i++)
        {
            ShowPukeID[i] = -1;//初始化展示牌ID
        }
        GameOver = true;//锁定出牌按钮
        IsNewGame =false;//不让新游戏开始
        Reset = true;//清空玩家手牌
        RequestSerialization();
        Setall();
    }
    public override void OnDeserialization()//同步数据后调用
    {
        Setall();
    }
    private void Setall()
    {
        SetJSButton();
        if (IsAdd&& PlayersName[UsingPNid] == Networking.LocalPlayer.displayName)//当加牌时
        {
                MyCardsID[PlayersCardCount[UsingPNid]-1] = AllCardsID[Cardscount + 1];
        }
        usingPN.text = PlayersName[UsingPNid];//更新当前使用的玩家名称
        SetMe();
        SetInGamePlayer();//更新玩家列表信息
        Setshowcards();//更新显示卡牌信息
        CopyToWorld();
    }
    /// <summary>计算并设置按钮状态</summary>
    private void SetJSButton()
    {
        bool ishide = true;
        foreach (string s in PlayersName)
            if (s == Networking.LocalPlayer.displayName)
            {
                JSbutton[0].SetActive(false);
                JSbutton[1].SetActive(true);
                JSbutton[2].SetActive(false);
                JSbutton[3].SetActive(true);
                ishide = false;
                break;

            }
        if (ishide)
        {
            JSbutton[0].SetActive(true);
            JSbutton[1].SetActive(false);
            JSbutton[2].SetActive(true);
            JSbutton[3].SetActive(false);
        }
    }
    private void SetMe()
    {
        if (Reset)
        {
            for (int i = 0; i < 6; i++)
            {
                MyCardsID[i] = -1;//初始化玩家手牌ID
                MySelectCardsID[i] = -1;//初始化玩家选择的卡牌ID
            }
        }
        else if (IsNewGame)//新游戏开始时重置玩家手牌
        {
            for (int i = 0; i < 8; i++)
            {
                if (Networking.LocalPlayer.displayName == PlayersName[i])
                {
                    if (i == 0)
                    {
                        for (int j = 0; j < 6; j++)
                        {
                            MyCardsID[j] = AllCardsID[107 - j];//存储玩家手牌ID
                            MySelectCardsID[j] = -1;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            MyCardsID[j] = AllCardsID[101 - 5 * (i - 1) - j];//存储玩家手牌ID
                            MySelectCardsID[j] = -1;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < 6; i++)
        {
            if (MyCardsID[i] == -1 || MyCardsID[i]==-2)minecards[i].sprite = nullimage.sprite;//如果玩家手牌ID为-1、-2，则显示空白图片
            else minecards[i].sprite = allpuke[MyCardsID[i]].sprite;
            if(MySelectCardsID[i] != -1) minecards[i].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);//初始化玩家选择的卡牌ID
            else minecards[i].color = Color.white;
        }
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
        for (i = 0; i < 8; i++)
        {
            copytoPlayersInGame[i].text = playersInGame[i].text;
            copytoPlayersScore[i].text = playersScore[i].text;
            copytoPlayersCardCount[i].text=playersCardCount[i].text;
        }
        for (i = 0; i < 6; i++)
        {
            copytoMinecards[i].sprite = minecards[i].sprite;
            copytoMinecards[i].color = minecards[i].color;
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
        if (GameOver|| MyCardsID[ForSelectMineCardID] == -2) return;
        if (MyCardsID[ForSelectMineCardID] == MySelectCardsID[ForSelectMineCardID])
        {
            MySelectCardsID[ForSelectMineCardID] = -1;
            minecards[ForSelectMineCardID].color = Color.white;//取消选择时恢复颜色
            copytoMinecards[ForSelectMineCardID].color = Color.white;
        }//如果选择的卡牌ID和手牌ID相同，则取消选择
        else
        {
            MySelectCardsID[ForSelectMineCardID] = MyCardsID[ForSelectMineCardID];
            minecards[ForSelectMineCardID].color = new Color(0xF1 / 255f, 0xA5 / 255f, 0xA5 / 255f, 1f);//选择时变为黄色
            copytoMinecards[ForSelectMineCardID].color = minecards[ForSelectMineCardID].color;
        }
    }
    public void Selectminecard0() { SetminecardForSelect(0); }
    public void Selectminecard1() { SetminecardForSelect(1); }
    public void Selectminecard2() { SetminecardForSelect(2); }
    public void Selectminecard3() { SetminecardForSelect(3); }
    public void Selectminecard4() { SetminecardForSelect(4); }
    public void Selectminecard5() { SetminecardForSelect(5); }
}
