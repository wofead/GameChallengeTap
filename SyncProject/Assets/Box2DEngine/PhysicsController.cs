using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Box2DSharp.Inspection;
using Box2DSharp.Tests;
using UnityEngine;

namespace Box2DSharp
{
    //放到system里面更新
    public class PhysicsController : MonoBehaviour
    {        
        [HideInInspector]
        public PhysicsFight PhysicsFight;
        
        public FpsCounter FpsCounter = new FpsCounter();

        public FixedUpdate FixedUpdate;

        private void Awake()
        {

            PhysicsFight.WorldSetting.UnityDrawer = UnityDrawer.GetDrawer();
            PhysicsFight.WorldSetting.Drawer = new BoxDrawer {Drawer = PhysicsFight.WorldSetting.UnityDrawer};

            FixedUpdate = new FixedUpdate(TimeSpan.FromSeconds(1 / 60d), Tick);
            MainCamera = Camera.main;
            PhysicsFight.WorldSetting.Camera = MainCamera;
        }

        private void Start()
        {
            PhysicsFight = new PhysicsFight();
            FixedUpdate.Start();
        }

        private void Tick()
        {
            PhysicsFight.Step();
            FpsCounter.SetFps();
        }

        public Camera MainCamera;

        public Vector3 Diference;

        public Vector3 Origin;

        public bool Drag;

        public void Update()
        {
            PhysicsFight.Update();
            FixedUpdate.Update();
            var mousePosition = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.R))
            {
                Restart();
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Pause();
            }

            if (Input.GetKeyDown(KeyCode.O))
            {
                SingleStep();
            }

            // Launch Bomb
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PhysicsFight.LaunchBomb();
            }

            // Mouse left drag
            PhysicsFight.MouseWorld = MainCamera.ScreenToWorldPoint(Input.mousePosition).ToVector2();
            PhysicsFight.MouseJoint?.SetTarget(PhysicsFight.MouseWorld);

            if (PhysicsFight.WorldSetting.EnableMouseAction)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        PhysicsFight.ShiftMouseDown();
                    }
                    else
                    {
                        PhysicsFight.MouseDown();
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    PhysicsFight.MouseUp();
                }

                // Mouse right move camera
                if (Input.GetMouseButton(1))
                {
                    Diference = MainCamera.ScreenToWorldPoint(Input.mousePosition)
                              - MainCamera.transform.position;
                    if (Drag == false)
                    {
                        Drag = true;
                        Origin = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
                else
                {
                    Drag = false;
                }

                if (Drag)
                {
                    MainCamera.transform.position = Origin - Diference;
                }

                // Mouse wheel zoom
                //Zoom out
                if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    if (MainCamera.orthographicSize > 1)
                    {
                        MainCamera.orthographicSize += 1f;
                    }
                    else
                    {
                        MainCamera.orthographicSize += 0.1f;
                    }
                }

                //Zoom in
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    if (MainCamera.orthographicSize > 1)
                    {
                        MainCamera.orthographicSize -= 1f;
                    }
                    else if (MainCamera.orthographicSize > 0.2f)
                    {
                        MainCamera.orthographicSize -= 0.1f;
                    }
                }
            }

            if (PhysicsFight.WorldSetting.Pause)
            {
                PhysicsFight.DrawString("****PAUSED****");
            }

            // FPS
            {
                var text = $"{FpsCounter.Ms:0.0} ms ({FpsCounter.Fps:F1} fps)";
                PhysicsFight.DrawString(text);
            }

            // Step
            {
                PhysicsFight.DrawString($"{PhysicsFight.StepCount} Steps");
            }
            PhysicsFight.DrawTest();
        }

        private Rect _rect;

        private GUIStyle _style;


        private void Restart()
        {
            //CurrentTest = (Test) Activator.CreateInstance(CurrentTest.GetType());
        }

        private void Pause()
        {
            PhysicsFight.WorldSetting.Pause = !PhysicsFight.WorldSetting.Pause;
        }

        private void SingleStep()
        {
            PhysicsFight.WorldSetting.SingleStep = true;
        }

        public void ToggleControlPanel(bool isShow)
        {
            PhysicsFight.WorldSetting.ShowControlPanel = isShow;
        }
    }
}