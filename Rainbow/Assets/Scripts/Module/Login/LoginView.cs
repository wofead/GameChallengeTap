using Login;
using UnityEngine;
using FairyGUI;

public class LoginView:BaseUi
{
    UI_LoginView view;

    public LoginView()
    {
        view = UI_LoginView.CreateInstance();
        Init(view);

    }

    override public void Init(GComponent com)
    {
        base.Init(com);
        Test();
    }

    public void RegistEvent()
    {
        
    }

    public void Test()
    {
        view.m_bgGraph.color = Color.red;
    }
}
