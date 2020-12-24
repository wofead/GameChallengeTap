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
        public Test CurrentTest;
        
        public string StartTest;

        public FpsCounter FpsCounter = new FpsCounter();

        public FixedUpdate FixedUpdate;

        private void Awake()
        {

            Test.TestSettings.UnityDrawer = UnityDrawer.GetDrawer();
            Test.TestSettings.Drawer = new BoxDrawer {Drawer = Test.TestSettings.UnityDrawer};

            FixedUpdate = new FixedUpdate(TimeSpan.FromSeconds(1 / 60d), Tick);
            MainCamera = Camera.main;
            Test.TestSettings.Camera = MainCamera;
        }

        private void Start()
        {
            CurrentTest = new HelloWorld();
            FixedUpdate.Start();
        }

        private void Tick()
        {
            CurrentTest.Step();
            FpsCounter.SetFps();
        }

        public Camera MainCamera;

        public Vector3 Diference;

        public Vector3 Origin;

        public bool Drag;

        public void Update()
        {
            CurrentTest.Update();
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
                CurrentTest.LaunchBomb();
            }

            // Mouse left drag
            CurrentTest.MouseWorld = MainCamera.ScreenToWorldPoint(Input.mousePosition).ToVector2();
            CurrentTest.MouseJoint?.SetTarget(CurrentTest.MouseWorld);

            if (Test.TestSettings.EnableMouseAction)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        CurrentTest.ShiftMouseDown();
                    }
                    else
                    {
                        CurrentTest.MouseDown();
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    CurrentTest.MouseUp();
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

            CurrentTest.DrawString(CurrentTest.TestName);
            if (Test.TestSettings.Pause)
            {
                CurrentTest.DrawString("****PAUSED****");
            }

            // FPS
            {
                var text = $"{FpsCounter.Ms:0.0} ms ({FpsCounter.Fps:F1} fps)";
                CurrentTest.DrawString(text);
            }

            // Step
            {
                CurrentTest.DrawString($"{CurrentTest.StepCount} Steps");
            }
            CurrentTest.DrawTest();
        }

        private Rect _rect;

        private GUIStyle _style;


        private string GetTestName(Type type)
        {
            return Regex.Replace(type.Name, @"(\B[A-Z])", " $1");
        }

        private void Restart()
        {
            CurrentTest = (Test) Activator.CreateInstance(CurrentTest.GetType());
        }

        private void Pause()
        {
            Test.TestSettings.Pause = !Test.TestSettings.Pause;
        }

        private void SingleStep()
        {
            Test.TestSettings.SingleStep = true;
        }

        public void ToggleControlPanel(bool isShow)
        {
            Test.TestSettings.ShowControlPanel = isShow;
        }

        private string GetSliderText(string text, int value)
        {
            return Regex.Replace(text, "([0-9]+)", $"{value}");
        }
    }
}