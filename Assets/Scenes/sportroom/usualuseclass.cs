
using UnityEngine;
using VRC.SDKBase;
using UnityEngine.UI;
using TMPro;

public static class usualuseclass 
{
    public static int RandomWithoutExcept(int min, int max, int[] exception)
    {
        int range = max - min;
        int index = Random.Range(0, range - exception.Length); // 在合法值序列中第 index 个
        int skip = 0;
        foreach (int i in exception)
        {
            if (i <= min + index + skip)
                if(i !=-1)
                    skip++;
        }
        return min + index + skip;
    }
    //将text组件的文本内容获取到string[]
    public static void LoadTextToString(Text getText, ref string[] usingText)
    {
        // 从Text组件中获取所有问题，并按行分割
        string[] lines = getText.text.Split('\n');
        int validCount = 0;
        foreach (string line in lines)
        {
            if (!string.IsNullOrEmpty(line.Trim())) validCount++;
        }
        usingText= new string[validCount];
        int index = 0;
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                usingText[index++] = trimmed;
            }
        }
    }
    //随机打乱一个string[]内的数据
    public static void SetRandomString(ref string[] ForRandom,int length)
    {
        // 创建一个新的数组来存储打乱顺序后的玩家
        string[] shuffledPlayers = ForRandom;

        // 使用Fisher-Yates洗牌算法来打乱顺序
        for (int i = length - 1; i > 0; i--)
        {
            // 生成一个0到i之间的随机数
            int j = Random.Range(0, i);
            // 交换当前元素和随机位置的元素
            string temp = shuffledPlayers[i];
            shuffledPlayers[i] = shuffledPlayers[j];
            shuffledPlayers[j] = temp;
        }
        // 将打乱后的数组赋值回原始数组
        SetStringArrayToStringArray(ref ForRandom, shuffledPlayers, length);
    }
    public static void SetRandomInt(ref int[] ForRandom, int length)
    {
        // 创建一个新的数组来存储打乱顺序后的玩家
        int[] shuffledPlayers = ForRandom;
        // 使用Fisher-Yates洗牌算法来打乱顺序
        for (int i = length - 1; i > 0; i--)
        {
            // 生成一个0到i之间的随机数
            int j = Random.Range(0, i);
            // 交换当前元素和随机位置的元素
            int temp = shuffledPlayers[i];
            shuffledPlayers[i] = shuffledPlayers[j];
            shuffledPlayers[j] = temp;
        }
        // 将打乱后的数组赋值回原始数组
        SetIntArrayToIntArray(ref ForRandom , shuffledPlayers,length);
    }
    public static bool IsSetOwn(GameObject Obj)
    { 
        if (!Networking.IsOwner(Networking.LocalPlayer, Obj))
        {
            Networking.SetOwner(Networking.LocalPlayer, Obj);
        }
        if (Networking.IsOwner(Networking.LocalPlayer, Obj)) { return true; }
        else { return false; }
    }//if (!usualuseclass.IsSetOwn(gameObject)) return;
    public static void SetIntOrder(ref int[] ForOrder)
    {
        for(int i = 0;i< ForOrder.Length; i++) { ForOrder[i] = i; }
    }
    public static void ResetStringToStringArray(ref string[] str,string set)
    {
        for (int i = 0; i < str.Length; i++)
        {
            str[i] = set;
        }
    }
    public static void SetStringArrayToStringArray(ref string[] str, string[] set,int length)
    {
        for (int i = 0; i < length; i++)
        {
            str[i] = set[i];
        }
    }
    public static void SetStringArrayToTMP(ref TextMeshProUGUI[] str, string[] set)
    {
        int j=str.Length;
        for(int i=0;i<j;i++)
        {
            str[i].text = set[i];
        }
    }
    public static void ResetIntToInt(ref int[] FI, int set)
    {
        for (int i = 0; i < FI.Length; i++) { FI[i] = set; }
    }
    public static void SetIntArrayToIntArray(ref int[] str, int[] set, int length)
    {
        for (int i = 0; i < length; i++)
        {
            str[i] = set[i];
        }
    }
    public static void ResetBoolToBool(ref bool[] Bl, bool set)
    {
        for (int i = 0; i < Bl.Length; i++) { Bl[i] = set; }
    }
    public static void ClearAndResetStringOrderFromeSelect(ref string[] str, ref int[] select, int nullint, int clearint)
    {
        int len = str.Length;
        string clearStr=clearint.ToString();
        for (int i = 0; i < len; i++)//清除玩家手牌中打出牌的显示
        {
            if (select[i] != nullint)
            {
                str[i] = clearStr;
                select[i] = nullint;//初始化自身状态
            }
        }
        for (int i = 0; i < len - 1; i++)//整理手牌
        {
            for (int j = 0; j < len - 1 - i; j++)
            {
                if (str[j] == clearStr)//被清除的卡牌
                {
                    str[j] = str[j + 1];
                    str[j + 1] = clearStr;//与后面的卡牌对调
                }
            }
        }
    }
    public static void ClearAndResetIntOrderFromeSelect(ref int[] setInt, ref int[] select,int nullint,int clearint)
    {
        int len= setInt.Length;
        for(int i=0; i < len; i++)//清除玩家手牌中打出牌的显示
        {
            if(select[i] != nullint)
            {
                setInt[i]= clearint;
                select[i]= nullint;//初始化自身状态
            }
        }
        for (int i = 0; i < len-1; i++)//整理手牌
        {
            for (int j = 0; j < len-1 - i; j++)
            {
                if (setInt[j] == clearint)//被清除的卡牌
                {
                    setInt[j] = setInt[j + 1];
                    setInt[j + 1] = clearint;//与后面的卡牌对调
                }
            }
        }
    }
}
