
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public static class usualuseclass 
{
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
        usingText = new string[validCount];
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
    public static void SetRandomString(ref string[] playerslist)
    {
        // 创建一个新的数组来存储打乱顺序后的玩家
        string[] shuffledPlayers = new string[playerslist.Length];
        // 复制原始玩家数组到临时数组
        System.Array.Copy(playerslist, shuffledPlayers, playerslist.Length);

        // 使用Fisher-Yates洗牌算法来打乱顺序
        for (int i = shuffledPlayers.Length - 1; i > 0; i--)
        {
            // 生成一个0到i之间的随机数
            int j = Random.Range(0, i + 1);
            // 交换当前元素和随机位置的元素
            string temp = shuffledPlayers[i];
            shuffledPlayers[i] = shuffledPlayers[j];
            shuffledPlayers[j] = temp;
        }
        // 将打乱后的数组赋值回原始数组
        System.Array.Copy(shuffledPlayers, playerslist, playerslist.Length);
    }
}
