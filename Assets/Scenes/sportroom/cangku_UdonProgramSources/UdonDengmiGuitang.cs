
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using static usualuseclass;

public class UdonDengmiGuitang : UdonSharpBehaviour
{
    public TextMeshProUGUI[] displayanswer;//显示答案的文本组件
    public TextMeshProUGUI[] displayquestion;//显示问题的文本组件，分别在个人面板和黑板上
    public Text HaiguiAnswer;//存储海龟汤的答案文本组件
    public Text HaiguiQuestion;//存储海龟汤的问题文本组件
    public Text DengmiAnswer;//存储灯谜的答案文本组件
    public Text DengmiQuestion;//存储灯谜的问题文本组件
    public TextMeshProUGUI[] Title;//个人面板的标题文本组件
    string[] DManswers = new string[0];//存储灯谜的答案的数组
    string[] DMquestions = new string[0];//存储灯谜的问题的数组
    string[] HGanswers = new string[0];//存储海龟汤的答案的数组
    string[] HGquestions = new string[0];//存储海龟汤的问题的数组
    [UdonSynced] int DMint = 0;//灯谜的索引
    [UdonSynced] int HGint = 0;//海龟汤的索引
    [UdonSynced] bool isDM = true;//是否为灯谜
    [UdonSynced] bool isShow = false;//是否显示答案

    void Start()
    {
        LoadTextToString(HaiguiAnswer, ref HGanswers);
        LoadTextToString(HaiguiQuestion, ref HGquestions);
        LoadTextToString(DengmiQuestion, ref DMquestions);
        LoadTextToString(DengmiAnswer, ref DManswers);
        isDM = true;
        //将灯谜和海龟汤的问题和答案加载到数组中
    }
    public void GetQuestionAndSetOwner()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        if (isDM)
        {
            DMint--;
            if (DMint < 0) DMint = DMquestions.Length - 1;
            for (int i = 0; i < displayquestion.Length; i++)
            {
                displayquestion[i].text = DMint + "、" + DMquestions[DMint];
                displayanswer[i].text = DMint + "、" + DManswers[DMint];
            }
        }
        else
        {
            HGint--;
            if (HGint < 0) HGint = HGquestions.Length - 1;
            for (int i = 0; i < displayquestion.Length; i++)
            {
                displayquestion[i].text =HGint+"、"+ HGquestions[HGint];
                displayanswer[i].text = HGint + "、" + HGanswers[HGint];
            }
        }
        isShow = false;
        RequestSerialization();
    }
    public void ShowAnswer()
    {
        if (!Networking.IsOwner(gameObject)) return;
        isShow = true;
        RequestSerialization();
    }
    public void ToHG()
    {
        isDM = !isDM;
        if (isDM)
        {
            Title[0].text = "猜灯谜";
            Title[1].text = "海龟汤";
        }
        else
        {
            Title[0].text = "海龟汤";
            Title[1].text = "猜灯谜";
        }
    }
    public override void OnDeserialization()
    {
        if (isDM)
        {
            for (int i = 0; i < displayquestion.Length; i++)
            {
                displayquestion[i].text = DMint + "、" + DMquestions[DMint];
                if (isShow)
                    displayanswer[i].text = DMint + "、" + DManswers[DMint];
                else
                    displayanswer[i].text = "未公开答案的谜底区域";
            }
            Title[0].text = "猜灯谜";
            Title[1].text = "海龟汤";
        }
        else
        {
            for (int i = 0; i < displayquestion.Length; i++)
            {
                displayquestion[i].text = HGint + "、" + HGquestions[HGint];
                if (isShow || Networking.IsOwner(gameObject))
                    displayanswer[i].text = HGint + "、" + HGanswers[HGint];
                else
                    displayanswer[i].text = "未公开答案的谜底区域";
            }
            Title[0].text = "海龟汤";
            Title[1].text = "猜灯谜";
        }
    }
}
