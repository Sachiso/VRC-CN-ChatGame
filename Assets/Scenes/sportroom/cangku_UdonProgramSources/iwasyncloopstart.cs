
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace HoshinoLabs.IwaSync3.Udon
{
    public class iwasyncloopstart : UdonSharpBehaviour
    {
        public VideoController videoController;
        void Start()
        {
            if (Networking.GetOwner(gameObject) == Networking.LocalPlayer)
            {
                RequestSerialization();
                videoController.LoopOn();
            }
        }
        public override void OnDeserialization()
        {
            videoController.LoopOn();
        }
    }
}
