using Login;
using UnityEngine;
using FairyGUI;

public class LoginView
{
    public UI_LoginView view;
    App app;

    private void LoadPackage()
    {
        UIPackage.AddPackage("Assets/Resources/UI/Login");
    }

    public LoginView(App app)
    {
        this.app = app;
        LoadPackage();
        LoginBinder.BindAll();
        view = UI_LoginView.CreateInstance();
        Init(view);
    }

    public void Init(UI_LoginView com)
    {
        this.view = com;
        RegistEvent();
    }

    public void RegistEvent()
    {
        view.m_loginBtn.onClick.Add(() => { app.EnterFight(); });
    }
}
