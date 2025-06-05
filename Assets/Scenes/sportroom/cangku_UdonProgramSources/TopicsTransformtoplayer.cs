using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TopicsTransformtoplayer : UdonSharpBehaviour
{
    public float rotationSpeed = 90f; // 旋转速度
    private VRCPlayerApi localPlayer;
    private Vector3 targetPosition; // 目标位置

    void Start()
    {
        // 获取本地玩家实例
        localPlayer = Networking.LocalPlayer;
    }

    void Update()
    {
        // 确保本地玩家存在且有效
        if (localPlayer != null && localPlayer.IsValid() && Networking.LocalPlayer != null)
        {
            // 获取玩家头部位置
            targetPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            
            // 计算方向向量（忽略Y轴）
            Vector3 direction = targetPosition - transform.position;
            /*direction.y = 0;*/
            
            // 反转方向向量，使文字板朝向相反方向
            direction = -direction;

            // 如果方向有效（避免除以零）
            if (direction.sqrMagnitude > 0.001f)
            {
                // 计算目标旋转
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                
                // 平滑旋转到目标方向
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}