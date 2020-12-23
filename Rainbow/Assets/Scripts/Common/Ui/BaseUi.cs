using System;
using UnityEngine;
using FairyGUI;
using FairyGUI.Utils;

public class BaseUi : BaseComponent
{
    GComponent gComponent;

    public void Awake()
    {
    }


    public virtual void Init(GComponent view, params object[] ps)
    {
        gComponent = view;
    }

    public virtual void Init(GComponent view)
    {
        gComponent = view;
    }

    public virtual void OnInitialize()
    {
        gComponent.Center();
    }

    public virtual void Dispose()
    {
        gComponent.Dispose();
    }

}

