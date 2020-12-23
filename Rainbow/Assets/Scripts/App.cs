using Assets.Scripts.Common;
using Assets.Scripts.Module.Fight;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App
{
    public UiManager uiManager;
    public FightManager fightManager;
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
    }

    public void GameStart()
    {
        LoginView loginView = new LoginView(this);
        uiManager.ShowView(loginView.view);
    }

    public void EnterFight()
    {
        fightManager.CreateFight();
    }

    public void Update()
    {

    }
}
