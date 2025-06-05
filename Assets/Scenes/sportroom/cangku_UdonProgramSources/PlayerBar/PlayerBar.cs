
using System.Diagnostics.Eventing.Reader;
using UdonSharp;
using UnityEngine;
using UnityEngine.InputSystem.Users;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class PlayerBar : UdonSharpBehaviour
{
    public GameObject PlayerLocalBar;
    private float lastPressTime = 0f; // 上一次按下时间
    private bool active = false;
    private bool EXSwitch = false;//用于分割长按闪现的开关
    private bool EXSwitch2 = false;
    void Start()
    {
        PlayerLocalBar.SetActive(active);
        SendCustomEventDelayedFrames(null, 3);
    }
    private void SetGOActive()
    {
        if (active)
        {
            active = false;
            PlayerLocalBar.SetActive(active);
        }
        else { Resettransform(); }
    }
    private void Resettransform()
    {
        active = true;
        PlayerLocalBar.SetActive(active);
        Vector3 LPHPos = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
        Quaternion LPHRot = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        Vector3 targetPosition = LPHPos;
        Vector3 offset = new Vector3(0, 0, 1f);
        targetPosition += LPHRot * offset;
        Quaternion setRot = Quaternion.LookRotation(LPHRot * Vector3.forward);
        if (!Networking.LocalPlayer.IsUserInVR())
            setRot = Quaternion.Euler(setRot.eulerAngles.x, setRot.eulerAngles.y, 0);
        PlayerLocalBar.transform.rotation = setRot;
        PlayerLocalBar.transform.position = targetPosition;
    }
    public void setcancel()//给面板关闭的按钮提供的方法
    {
        SetGOActive();
    }

    private void Update()
    {
        if(Networking.LocalPlayer== null) return;
        //判定PC键盘Tab的调出菜单方法
        if (!Networking.LocalPlayer.IsUserInVR())
        {
            if ((Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.E)) && !EXSwitch)
            {
                SetGOActive();
                EXSwitch = true;
            }
            else if (Input.GetKey(KeyCode.Tab))
            {
                if (lastPressTime < 0.1) lastPressTime = Time.time;
                else if (Time.time - lastPressTime > 1.6) lastPressTime = 0;
                else if (Time.time - lastPressTime > 1.5) Resettransform();

            }
            else if (Input.GetKeyUp(KeyCode.Tab)) { lastPressTime = 0; }
            else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.E)) { EXSwitch = false; }
        }
        else if (Networking.LocalPlayer.IsUserInVR())
        {
            float lefttrigger = Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger");
            float righttrigger = Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger");
            float doubleClickThreshold = 0.3f;
            float Ftime = Time.time - lastPressTime;
            //判定左手扳机的双击调出菜单方法
            if (lefttrigger > 0.3f && !EXSwitch && !active)
            {
                EXSwitch = true;
                if (Ftime <= doubleClickThreshold)
                { SetGOActive(); EXSwitch = EXSwitch2 = false; lastPressTime--; }
                else lastPressTime = Time.time;
            }
            else if (lefttrigger < 0.3f) { EXSwitch = false; }

            //判定右手扳机的双击调出菜单方法
            if (righttrigger > 0.3f && !EXSwitch2 && !active)
            {
                EXSwitch2 = true;
                if (Ftime <= doubleClickThreshold)
                { SetGOActive(); EXSwitch = EXSwitch2 = false; lastPressTime--; }
                else lastPressTime = Time.time;
            }
            else if (righttrigger < 0.3f) { EXSwitch2 = false; }

            /*
            if (lefttrigger > 0.3f && !active)
            {
                if (righttrigger > 0.3f )
                {
                    if (!EXSwitch)
                    {
                        lastPressTime = Time.time;
                        EXSwitch = true;
                    }
                    float Ftime = Time.time - lastPressTime;
                    if (Ftime < 1.5)
                        if (Ftime>1)
                            Resettransform();
                }
            }
            else if (lefttrigger < 0.1f|| righttrigger < 0.1f) EXSwitch = false;
            */

        }
    }
}
