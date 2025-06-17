
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using static usualuseclass;

namespace HoshinoLabs.IwaSync3.Udon
{
    public class iwasyncloopstart : UdonSharpBehaviour
    {
        public VideoController videoController;
        void Start()
        {
            if(Networking.IsOwner(gameObject))
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
