
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class stepudsw : UdonSharpBehaviour
{
    public GameObject GOA;//最下方
    public GameObject GOB;//第二个
    public GameObject GOC;//第三个
    public GameObject GOD;// 最上方
    public GameObject setbackcollider;
    private float transitionDuration = 8f; // 过渡持续时间
    private float Yset = 2.01f; // 显示时的 Y 轴偏移量
    [UdonSynced] private bool isDown = true; // 是否在下方
    Vector3 countchange = new Vector3(0f, 0.5f, 1f);
    Vector3 fixchange = new Vector3(0f, 0f, 0.4f);
    [UdonSynced] private Vector3 startPosition; // 开始位置
    [UdonSynced] private Vector3 targetPosition; // 目标位置
    [UdonSynced] private float transitionTimer = 9f; // 过渡计时器，从0开始一直到上面的持续时间为止

    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        // 如果正在移动，不执行任何操作
        if (transitionTimer < transitionDuration)
        {
            return;
        }
        // 切换墙体状态
        if (isDown)
        {
            // 如果当前是向下，则切换到向上
            startPosition = GOD.transform.position;
            targetPosition = startPosition + new Vector3(0f, Yset, 0f);
        }
        else
        {
            // 如果当前是向上，则切换到向下
            startPosition = GOD.transform.position;
            targetPosition = startPosition + new Vector3(0f, -Yset, 0f);
            
        }

        // 重置过渡计时器
        transitionTimer = 0f;
        isDown = !isDown;
        RequestSerialization();
    }

    void Update()
    {
        // 如果正在过渡
        if (transitionTimer < transitionDuration)
        {
            if (!isDown)
            {
                // 更新过渡计时器
                transitionTimer += Time.deltaTime;
                // 计算插值
                float t1 = transitionTimer / transitionDuration;
                float t2 = (transitionTimer) / (transitionDuration - 2f);
                float t3 = (transitionTimer) / (transitionDuration - 4f);
                float t4 = (transitionTimer) / (transitionDuration - 6f);
                // 移动墙体
                GOD.transform.position = Vector3.Lerp(startPosition, targetPosition, t1);

                if (t2 < 1)
                    GOC.transform.position = Vector3.Lerp(startPosition + new Vector3(0f, 0f, -0.6f), targetPosition - countchange + fixchange, t2);
                if (t3 < 1)
                    GOB.transform.position = Vector3.Lerp(startPosition + new Vector3(0f, 0f, -1.6f), targetPosition - 2 * countchange + fixchange, t3);
                if (t4 < 1)
                    GOA.transform.position = Vector3.Lerp(startPosition + new Vector3(0f, 0f, -2.6f), targetPosition - 3 * countchange + fixchange, t4);
                if (transitionTimer >= transitionDuration)
                        setbackcollider.SetActive(true);
            }
            else if (isDown)
            {
                if(transitionTimer <= transitionDuration) setbackcollider.SetActive(false);
                // 更新过渡计时器
                transitionTimer += Time.deltaTime;
                // 计算插值
                float t1 = transitionTimer / transitionDuration;
                float t2 = (transitionTimer - 2f) / (transitionDuration - 2f);
                float t3 = (transitionTimer - 4f) / (transitionDuration - 4f);
                float t4 = (transitionTimer - 6f) / (transitionDuration - 6f);
                // 移动墙体
                GOD.transform.position = Vector3.Lerp(startPosition, targetPosition, t1);
                if (t2 > 0)
                    GOC.transform.position = Vector3.Lerp(startPosition - countchange + fixchange, targetPosition + new Vector3(0f, 0f, -0.6f), t2);
                if (t3 > 0)
                    GOB.transform.position = Vector3.Lerp(startPosition - 2 * countchange + fixchange, targetPosition + new Vector3(0f, 0f, -1.6f), t3);
                if (t4 > 0)
                    GOA.transform.position = Vector3.Lerp(startPosition - 3 * countchange + fixchange, targetPosition + new Vector3(0f, 0f, -2.6f), t4);
            }

        }
       
    }
}
