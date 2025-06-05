
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class whiteboardshowbutton : UdonSharpBehaviour
{
    public GameObject TMPMID;
    public GameObject TMPDOWN;
    public GameObject TMPUP;
    [UdonSynced] private int show = 0;

    private void ForButton(int setint) {
        show= setint;
        RequestSerialization();
        showthat();
    }
    public override void OnDeserialization()
    {
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
