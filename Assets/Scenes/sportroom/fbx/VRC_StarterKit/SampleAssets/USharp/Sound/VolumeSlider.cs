using UdonSharp;
using UnityEngine;
using UnityEngine.UI;

namespace VRC_StarterKit.SampleAssets.USharp.Sound
{
    public class VolumeSlider : UdonSharpBehaviour
    {
        public AudioSource target;
        private Slider _slider;
        
        void Start()
        {
            _slider = GetComponent<Slider>();
        }

        public void OnSliderValueChanged()
        {
            if (_slider == null) return;
            target.volume = _slider.value;
        }
    }
}
