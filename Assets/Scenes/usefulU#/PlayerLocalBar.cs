
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PlayerLocalBar : UdonSharpBehaviour
{
    public GameObject UIUI;
    public GameObject GetWordUI;
    private void Start()
    {
        GetUIUI();
    }
    public void GetUIUI()
    {
        UIUI.SetActive(true);
        GetWordUI.SetActive(false);
    }
    public void GetGetWordUI() 
    {
        UIUI.SetActive(false);
        GetWordUI.SetActive(true);
    }
}
