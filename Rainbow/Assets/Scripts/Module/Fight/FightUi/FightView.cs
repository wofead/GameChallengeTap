using Fight;
using UnityEngine;
using FairyGUI;

public class FightView
{
    public UI_FightView view;
    App app;

    private void LoadPackage()
    {
        UIPackage.AddPackage("Assets/Resources/UI/Fight");
    }

    public FightView(App app)
    {
        this.app = app;
        LoadPackage();
        FightBinder.BindAll();
        view = UI_FightView.CreateInstance();
        Init(view);
    }

    public void Init(UI_FightView com)
    {
        this.view = com;
        RegistEvent();
    }

    public void RegistEvent()
    {
        //view.m_loginBtn.onClick.Add(() => { app.EnterFight(); });
    }
}
