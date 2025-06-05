using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class screencurveswitch : UdonSharpBehaviour
{
    public GameObject objectLow; // 第一个物体
    public GameObject objectHigh; // 第二个物体
    private bool isObjectAActive = true; // 同步状态
    private bool isAllHidden = true; // 判定是否全隐藏

    void Start()
    {
        UpdateObjects(); // 设置物体属性
    }

    public override void Interact() // 点击交互时调用
    {
        if (isAllHidden)
        {
            // 切换状态
            isObjectAActive = !isObjectAActive;
            objectLow.SetActive(false);
            objectHigh.SetActive(false);
            isAllHidden = false;
        }
        else
        {
            UpdateObjects();
        }
    }
    void UpdateObjects()
    {
        objectLow.SetActive(isObjectAActive);
        objectHigh.SetActive(!isObjectAActive);
        isAllHidden=true;
    }
}
