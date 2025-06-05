using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class mirrorbasecontrol : UdonSharpBehaviour
{
    public GameObject wallTransform; // 墙体的 Transform
    //private float moveSpeed = 0.5f; // 移动速度
    private float transitionDuration = 1f; // 过渡持续时间
    private float YOffset = 3.2f; // 显示时的 Y 轴偏移量
    private bool isHidden = true; // 是否隐藏的标志
    private Vector3 startPosition; // 开始位置
    private Vector3 targetPosition; // 目标位置
    private float transitionTimer; // 过渡计时器
    private bool setHidden = false;//用于判定
    public GameObject foron;
    public GameObject foroff;

    void Start()
    {
        // 设置默认隐藏状态
        startPosition = wallTransform.transform.position;
        targetPosition = startPosition + new Vector3(0f, -YOffset, 0f);
        wallTransform.transform.position = targetPosition;
        wallTransform.SetActive(!isHidden);
    }

    public override void Interact()
    {
        // 如果正在移动，不执行任何操作
        if (transitionTimer < transitionDuration) return;
        // 切换墙体状态
        else if (setHidden == isHidden) return;
        if (isHidden)
        {
            // 如果当前是隐藏状态，则切换到显示状态
            wallTransform.SetActive(isHidden);
            startPosition = wallTransform.transform.position;
            targetPosition = startPosition + new Vector3(0f, YOffset, 0f);
        }
        else
        {
            // 如果当前是显示状态，则切换到隐藏状态
            startPosition = wallTransform.transform.position;
            targetPosition = startPosition + new Vector3(0f, -YOffset, 0f);
        }
        if (foron != null && foroff != null)
        {
            foron.SetActive(isHidden);
            foroff.SetActive(!isHidden);
        }
        // 重置过渡计时器
        transitionTimer = 0f;
        isHidden = !isHidden;
        setHidden = !isHidden;
    }
    public void seton()
    {
        if (transitionTimer < transitionDuration) return;
        setHidden = false;
    }
    public void setoff()
    {
        if (transitionTimer < transitionDuration) return;
        setHidden = true;
    }

    void Update()
    {
        // 如果正在过渡
        if (transitionTimer < transitionDuration)
        {
            // 更新过渡计时器
            transitionTimer += Time.deltaTime;

            // 计算插值
            float t = transitionTimer / transitionDuration;

            // 移动墙体
            wallTransform.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            // 如果过渡完成，设置墙体的显隐状态
            if (transitionTimer >= transitionDuration)
            {
                if (isHidden)
                {
                    wallTransform.SetActive(false);
                }
            }
        }
    }
}