using System;
using UdonSharp;
using UnityEngine;

namespace VRC_StarterKit.SampleAssets.USharp.Time
{
    public class TimeApiMock : UdonSharpBehaviour
    {
        [SerializeField] private Texture2D mockApiTex;
        [SerializeField] private Camera timeApiCamera;

        private const float TimeApiCameraDisableSec = 5f;
        
        void Start()
        {
            CreateMockApiTex();
            SendCustomEventDelayedSeconds(nameof(DisableTimeApiCamera), TimeApiCameraDisableSec);
        }

        public void DisableTimeApiCamera()
        {
            if (timeApiCamera) timeApiCamera.enabled = false;
        }
        
        private void CreateMockApiTex()
        {
            var now = DateTime.Now;

            SetValueBelongX(mockApiTex, 0, 0, 2, now.Hour);
            SetValueBelongX(mockApiTex, 2, 0, 2, now.Minute);
            SetValueBelongX(mockApiTex, 4, 0, 2, now.Second);
            SetValueBelongX(mockApiTex, 6, 0, 2, now.Millisecond);

            SetValueBelongX(mockApiTex, 0, 1, 3, now.Year-1900);
            SetValueBelongX(mockApiTex, 3, 1, 2, now.Month-1);
            SetValueBelongX(mockApiTex, 5, 1, 2, now.Day);
            SetValueBelongX(mockApiTex, 7, 1, 1, (int)now.DayOfWeek);
            
            var moonAge = (((now.Year - 2009) % 19) * 11 + (now.Month + 1) + (now.Day + 1)) % 30;
            SetValueBelongX(mockApiTex, 0, 2, 2, moonAge);
            
            mockApiTex.Apply();
        }

        private void SetValueBelongX(Texture2D o, int x, int y, int xlen, int value)
        {
            for (int i = 0; i < xlen; i++)
            {
                SetValue(o, x+i, y, value);
                value >>= 3;
            }
        }
        
        private void SetValue(Texture2D o, int x, int y, int value)
        {
            int r = 0 < (value & (1 << 0)) ? 1 : 0;
            int g = 0 < (value & (1 << 1)) ? 1 : 0;
            int b = 0 < (value & (1 << 2)) ? 1 : 0;
            o.SetPixel(x, o.height - 1 - y, new Color(r, g, b));
        }
    }
}
