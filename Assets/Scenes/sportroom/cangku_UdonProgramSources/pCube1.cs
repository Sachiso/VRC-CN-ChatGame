using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;
public class pCube1 : UdonSharpBehaviour
{
    /*简单学习一下：
   a= RigidbodyConstraints.FreezeRotationX|RigidbodyConstraints.FreezePotationX;//这里是两个二进制符号相加0010|0010时为0010,用+则是0100
   a|=RigidbodyConstraints.FreezeRotationY;//这里相当于+=
   a &= ~RigidbodyConstraints.FreezeRotationY;//这里相当于-=  */
    public Animator PPV;
    [UdonSynced] private bool setback=false;
    [UdonSynced] private float starttime = 0;
    public GameObject TMPMID;
    public GameObject TMPDOWN;
    public GameObject TMPUP;
    [UdonSynced] private int show = 0;
    public override void Interact()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (!Networking.IsOwner(gameObject)) return;
        if (Time.time - starttime <= 1.5f) return;
        setback = !setback;
        starttime=Time.time;
        RequestSerialization();
        PPV.SetBool("setback", setback);
    }
    public override void OnDeserialization()
    {
        PPV.SetBool("setback", setback);
        showthat();
    }
    private void ForButton(int setint)
    {
        show = setint;
        RequestSerialization();
        showthat();
    }
    private void showthat()
    {
        if (show == 0)
        {
            TMPMID.SetActive(false);
            TMPDOWN.SetActive(false);
            TMPUP.SetActive(false);
        }
        else if (show == 1)
        {
            TMPMID.SetActive(true);
            TMPDOWN.SetActive(false);
            TMPUP.SetActive(false);
        }
        else if (show == 2)
        {
            TMPMID.SetActive(false);
            TMPDOWN.SetActive(true);
            TMPUP.SetActive(true);
        }
    }
    public void CAB() { Networking.SetOwner(Networking.LocalPlayer, gameObject); if (!Networking.IsOwner(gameObject)) return; ForButton(0); }
    public void O1B() { Networking.SetOwner(Networking.LocalPlayer, gameObject); if (!Networking.IsOwner(gameObject)) return; ForButton(1); }
    public void O2B() { Networking.SetOwner(Networking.LocalPlayer, gameObject); if (!Networking.IsOwner(gameObject)) return; ForButton(2); }
}
