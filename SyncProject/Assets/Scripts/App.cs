using Assets.Scripts.Common;
using Assets.Scripts.Module.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App
{
    public UiManager uiManager;
    public FightManager fightManager;
    public TemplateManager templateManager;

    public GameObject fight;
    public App()
    {
        Application.targetFrameRate = 60;
    }

    public void Init()
    {
        InitMananger();
    }

    private void InitMananger()
    {
        uiManager = new UiManager();
        fightManager = new FightManager(this);
        templateManager = new TemplateManager();
    }

    public void GameStart()
    {
        LoginView loginView = new LoginView(this);
        uiManager.ShowView(loginView.view);
    }

    public void EnterFight()
    {
        uiManager.CloseAllView();
        fightManager.CreateFight();
    }

    public void Update()
    {

    }
}
