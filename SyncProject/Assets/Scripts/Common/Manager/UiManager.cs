using FairyGUI;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Common
{
    public class UiManager
    {
        GComponent uiLayer;
        Camera stageCamera;
        Dictionary<string, GComponent> viewDic;
        public UiManager()
        {
            uiLayer = new GComponent();
            uiLayer.displayObject.gameObject.name = "WindowLayer";
            uiLayer.width = 1920;
            uiLayer.height = 1080;
            uiLayer.Center(true);
            GRoot.inst.AddChild(uiLayer);
            stageCamera = GameObject.Find("Stage Camera").GetComponent<Camera>();
            viewDic = new Dictionary<string, GComponent>();
        }

        public void ShowView(GComponent view)
        {
            if (viewDic.ContainsKey(view.name))
            {
                view.visible = true;
            }
            else
            {
                uiLayer.AddChild(view);
                viewDic[view.name] = view;
            }
        }

        public void CloseView(GComponent view)
        {
            if (viewDic.ContainsKey(view.name))
            {
                view.visible = false;
            }
        }

        public void CloseAllView()
        {
            foreach (var kv in viewDic)
            {
                kv.Value.visible = false;
            }
        }
    }
}
