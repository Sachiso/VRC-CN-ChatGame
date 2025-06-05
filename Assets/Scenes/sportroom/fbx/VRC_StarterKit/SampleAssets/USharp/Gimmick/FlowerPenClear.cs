using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp.Gimmick
{
    public class FlowerPenClear : UdonSharpBehaviour
    {
        [SerializeField] private FlowerPen pen;
        
        public override void Interact()
        {
            pen.SendCustomEvent(nameof(FlowerPen.TriggerPenDrawClear));
        }
    }
}
