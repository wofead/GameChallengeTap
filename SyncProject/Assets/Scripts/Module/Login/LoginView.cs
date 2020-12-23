using Login;
using UnityEngine;
using FairyGUI;

public class LoginView
{
    public UI_LoginView view;

    private void LoadPackage()
    {
        UIPackage.AddPackage("Assets/Resources/UI/Login");
    }

    public LoginView()
    {
        LoadPackage();
        LoginBinder.BindAll();
        view = UI_LoginView.CreateInstance();
        Init(view);
    }

    public void Init(GComponent com)
    {
        Test();
    }

    public void RegistEvent()
    {

    }

    public void Test()
    {
        //view.m_bgGraph.color = Color.red;
    }
}
