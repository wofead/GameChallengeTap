using Assets.Scripts.Module.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class FightManager
    {
        private App app;
        private int keyFramesCount;
        private int allFramesCount;

        public FightManager(App app)
        {
            this.app = app;
            keyFramesCount = 0;
            allFramesCount = 0;
        }
        public void CreateFight()
        {
            GameObject fightStartUp = new GameObject("fightStartUp");
            app.fight = fightStartUp;
            FightStartUp fight =  fightStartUp.AddComponent<FightStartUp>();
            fight.Init(app);

        }
    }
}
