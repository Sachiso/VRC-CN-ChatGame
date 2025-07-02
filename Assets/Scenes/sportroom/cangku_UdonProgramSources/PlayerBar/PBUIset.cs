using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

public class PBUIset : UdonSharpBehaviour
{
    public Slider PPVEVSlider;
    public Animator PPV;
    public Slider PPVBLSlider;

    private void Start()
    {
        OnPPVEVSliderChanged();
        OnPPVBLSliderChanged();
    }
    public void OnPPVEVSliderChanged()
    {
        PPV.SetFloat("VolumeEV", PPVEVSlider.value);
    }
    public void OnPPVBLSliderChanged()
    {
        PPV.SetFloat("VolumeBL", PPVBLSlider.value);
    }
}
