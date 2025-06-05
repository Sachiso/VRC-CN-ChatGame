using UdonSharp;
using VRC.SDKBase;

namespace VRC_StarterKit.SampleAssets.USharp.Chair
{
    public class Chair : UdonSharpBehaviour
    {
        public override void Interact()
        {
            Networking.LocalPlayer.UseAttachedStation();
        }
    }
}
