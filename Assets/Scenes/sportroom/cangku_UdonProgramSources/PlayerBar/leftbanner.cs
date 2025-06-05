
using UdonSharp;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class leftbanner : UdonSharpBehaviour
{
    public GameObject[] gameObjects;
    public Image[] buttons;
    private void Start()
    {
        SetObjectActive(0);
    }
    public void SetObjectActive(int set)
    {
        for (int i = 0; i < gameObjects.Length; i++)
        {
            gameObjects[i].SetActive(false);

            buttons[i].color = new Color(0.137f, 0.137f, 0.137f);
        }
        gameObjects[set].SetActive(true);
        buttons[set].color = new Color(0.380f, 0.165f, 0.165f);
    }
    public void setfor0() { SetObjectActive(0); }
    public void setfor1() { SetObjectActive(1); }
    public void setfor2() { SetObjectActive(2); }
    public void setfor3() { SetObjectActive(3); }
    public void setfor4() { SetObjectActive(4); }
    public void setfor5() { SetObjectActive(5); }
    public void setfor6() { SetObjectActive(6); }
    public void setfor7() { SetObjectActive(7); }
    public void setfor8() { SetObjectActive(8); }
    public void setfor9() { SetObjectActive(9); }
}
