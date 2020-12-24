using System;
using Box2DSharp.Inspection;
using UnityEngine;

namespace Box2DSharp
{
    public class WorldSetting
    {
        public Camera Camera;
        
        public UnityDrawer UnityDrawer;

        public BoxDrawer Drawer;

        public bool Sleep = true;

        public bool WarmStarting = true;

        public bool TimeOfImpact = true;

        public bool SubStepping = false;

        public bool Shape = true;

        public bool Joint = true;

        public bool AABB = false;

        public bool Pair = false;

        public bool CenterOfMass = false;

        public bool ContactPoint = false;

        public bool ContactNormals = false;

        public bool ContactImpulse = false;

        public bool FrictionImpulse = false;

        public bool Statistics = false;

        public bool Profile = false;

        public bool Pause;

        public bool SingleStep;

        public float Dt = 1 / 60f;

        public int PositionIteration = 3;

        public int VelocityIteration = 8;

        public bool ShowControlPanel;

        public bool EnableMouseAction;

        public void Awake()
        { }
    }
}