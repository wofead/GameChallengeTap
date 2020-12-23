using FairyGUI;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
    public class UiManager
    {
        GComponent uiLayer;
        Camera stageCamera;
        public UiManager()
        {
            uiLayer = new GComponent();
            uiLayer.width = 1920;
            uiLayer.height = 1080;
            uiLayer.Center(true);
            GRoot.inst.AddChild(uiLayer);
            stageCamera = GameObject.Find("Stage Camera").GetComponent<Camera>();
        }

        public void ShowView(GComponent view)
        {
            uiLayer.AddChild(view);
        }
    }
}
