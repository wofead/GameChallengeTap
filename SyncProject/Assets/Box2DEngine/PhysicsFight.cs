using System;
using System.Text;
using System.Text.RegularExpressions;
using Box2DSharp.Collision;
using Box2DSharp.Collision.Collider;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using Box2DSharp.Dynamics.Joints;
using Joint = Box2DSharp.Dynamics.Joints.Joint;
using Random = System.Random;
using Vector2 = System.Numerics.Vector2;
using Color = Box2DSharp.Common.Color;
using System.Collections.Generic;
using UnityEngine;
using Assets.Box2DEngine.Unity;
using TrueSync.Physics2D.Common;

namespace Box2DSharp
{
    public class PhysicsFight : IContactListener, IDestructionListener, IDisposable
    {
        public static readonly WorldSetting WorldSetting = new WorldSetting();

        public const int RandomLimit = 32767;

        private readonly Random _random = new Random();

        public readonly ContactPoint[] Points = new ContactPoint[2048];

        public int PointsCount;

        public Body GroundBody;

        public Profile MaxProfile = new Profile();

        public MouseJoint MouseJoint;

        public Vector2 MouseWorld;

        public Profile TotalProfile = new Profile();

        public World World;

        public readonly string TestName;

        Dictionary<Body, string> entityMap;

        Dictionary<Body, GameObject> gameObjectMap;

        Dictionary<Fixture, Dictionary<Fixture, BoxCollision2D>> collisionInfo;

        private ObjectPool<BoxCollision2D> _collisionPool;
        public ObjectPool<BoxRaycastHit2D> raycastHitPool;

        public int StepCount;

        public IDrawer Drawer => WorldSetting.Drawer;/**
         *  @brief Public access to a manager instance.
         **/
        public static PhysicsFight instance;

        public PhysicsFight()
        {
            TestName = Regex.Replace(GetType().Name, @"(\B[A-Z])", " $1");
            //新建一个物理世界，参数为gravity，为10，box2D中的计量单位为米
            World = new World(new Vector2(0, -10));
            World.SetContactListener(this);
            World.DestructionListener = this;
            World.Drawer = Drawer;
            GroundBody = World.CreateBody(new BodyDef());
            DumpLogger.Instance = new UnityLogger();
            instance = this;
        }

        public void BeginContact(Contact contact)
        { }

        public void EndContact(Contact contact)
        { }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
            ref readonly var manifold = ref contact.Manifold;

            if (manifold.PointCount == 0)
            {
                return;
            }

            var fixtureA = contact.FixtureA;
            var fixtureB = contact.FixtureB;

            var state1 = new PointState [Settings.MaxManifoldPoints];
            var state2 = new PointState [Settings.MaxManifoldPoints];
            CollisionUtils.GetPointStates(state1, state2, oldManifold, manifold);

            contact.GetWorldManifold(out var worldManifold);

            for (var i = 0; i < manifold.PointCount && PointsCount < Points.Length; ++i)
            {
                var cp = new ContactPoint
                {
                    FixtureA = fixtureA, FixtureB = fixtureB,
                    Position = worldManifold.Points[i], Normal = worldManifold.Normal,
                    State = state2[i], NormalImpulse = manifold.Points[i].NormalImpulse,
                    TangentImpulse = manifold.Points[i].TangentImpulse, Separation = worldManifold.Separations[i]
                };
                Points[PointsCount] = cp;
                ++PointsCount;
            }
        }

        /// <inheritdoc />
        public void PostSolve(Contact contact, in ContactImpulse impulse)
        { }

        /// <inheritdoc />
        public void SayGoodbye(Joint joint)
        {
            if (MouseJoint == joint)
            {
                MouseJoint = null;
            }
            else
            {
                JointDestroyed(joint);
            }
        }

        /// <inheritdoc />
        public void SayGoodbye(Fixture fixture)
        {
            /* Do nothing */
        }

        /// <summary>
        /// trigger on every game loop frame
        /// </summary>
        public void Update()
        { }

        /// <summary>
        /// draw string and draw shape in here
        /// </summary>
        public void OnRender()
        { }

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        public string Text;

        public void DrawString(string text)
        {
            _stringBuilder.AppendLine(text);
        }

        /// <summary>
        /// call GUI.Lable in here
        /// </summary>
        public void OnGUI()
        { }

        /// <summary>
        /// trigger on every fixed physic frame 
        /// </summary>
        protected void OnStep()
        { }

        public void Step()
        {
            if (WorldSetting.Pause)
            {
                if (WorldSetting.SingleStep)
                {
                    WorldSetting.SingleStep = false;
                    WorldStep();
                }
            }
            else
            {
                WorldStep();
            }

            void WorldStep()
            {
                World.AllowSleep = WorldSetting.Sleep;
                World.WarmStarting = WorldSetting.WarmStarting;
                World.ContinuousPhysics = WorldSetting.TimeOfImpact;
                World.SubStepping = WorldSetting.SubStepping;

                PointsCount = 0;

                OnStep();
                //每一帧持续的时间，internal， 1/FPS秒，delta小于internal，慢放，大于就是快放了。所以我们应该保证delta=internal
                //PositionIteration，碰撞重叠之后校正的次数,单次step物理模拟计算中函数SolvePositionConstraints()执行次数。
                //VelocityIteration，碰撞重叠之后速度每帧校正次数。
                World.Step(WorldSetting.Dt, WorldSetting.VelocityIteration, WorldSetting.PositionIteration);
                PostStep();
                ++StepCount;
            }
        }

        protected void PostStep()
        { }

        public void DrawTest()
        {
            // Statistics
            if (WorldSetting.Statistics)
            {
                DrawString($"bodies/contacts/joints = {World.BodyCount}/{World.ContactCount}/{World.JointCount}");
                DrawString(
                    $"proxies:{World.ProxyCount}/ height:{World.TreeHeight}/ balance: {World.TreeBalance} /quality: {World.TreeQuality}");
            }

            // Profile
            if (WorldSetting.Profile)
            {
                var p = World.Profile;

                // Track maximum profile times
                MaxProfile.Step = Math.Max(MaxProfile.Step, p.Step);
                MaxProfile.Collide = Math.Max(MaxProfile.Collide, p.Collide);
                MaxProfile.Solve = Math.Max(MaxProfile.Solve, p.Solve);
                MaxProfile.SolveInit = Math.Max(MaxProfile.SolveInit, p.SolveInit);
                MaxProfile.SolveVelocity = Math.Max(MaxProfile.SolveVelocity, p.SolveVelocity);
                MaxProfile.SolvePosition = Math.Max(MaxProfile.SolvePosition, p.SolvePosition);
                MaxProfile.SolveTOI = Math.Max(MaxProfile.SolveTOI, p.SolveTOI);
                MaxProfile.Broadphase = Math.Max(MaxProfile.Broadphase, p.Broadphase);

                TotalProfile.Step += p.Step;
                TotalProfile.Collide += p.Collide;
                TotalProfile.Solve += p.Solve;
                TotalProfile.SolveInit += p.SolveInit;
                TotalProfile.SolveVelocity += p.SolveVelocity;
                TotalProfile.SolvePosition += p.SolvePosition;
                TotalProfile.SolveTOI += p.SolveTOI;
                TotalProfile.Broadphase += p.Broadphase;

                var aveProfile = new Profile();
                if (!WorldSetting.Pause && StepCount > 0)
                {
                    var scale = 1.0f / StepCount;
                    aveProfile.Step = scale * TotalProfile.Step;
                    aveProfile.Collide = scale * TotalProfile.Collide;
                    aveProfile.Solve = scale * TotalProfile.Solve;
                    aveProfile.SolveInit = scale * TotalProfile.SolveInit;
                    aveProfile.SolveVelocity = scale * TotalProfile.SolveVelocity;
                    aveProfile.SolvePosition = scale * TotalProfile.SolvePosition;
                    aveProfile.SolveTOI = scale * TotalProfile.SolveTOI;
                    aveProfile.Broadphase = scale * TotalProfile.Broadphase;
                }

                DrawString($"step [ave] (max) = {p.Step} [{aveProfile.Step}] ({MaxProfile.Step})");
                DrawString($"collide [ave] (max) = {p.Collide} [{aveProfile.Collide}] ({MaxProfile.Collide})");
                DrawString($"solve [ave] (max) = {p.Solve} [{aveProfile.Solve}] ({MaxProfile.Solve})");
                DrawString($"solve init [ave] (max) = {p.SolveInit} [{aveProfile.SolveInit}] ({MaxProfile.SolveInit})");
                DrawString($"solve velocity [ave] (max) = {p.SolveVelocity} [{aveProfile.SolveVelocity}] ({MaxProfile.SolveVelocity})");
                DrawString($"solve position [ave] (max) = {p.SolvePosition} [{aveProfile.SolvePosition}] ({MaxProfile.SolvePosition})");
                DrawString($"solveTOI [ave] (max) = {p.SolveTOI} [{aveProfile.SolveTOI}] ({MaxProfile.SolveTOI})");
                DrawString($"broad-phase [ave] (max) = {p.Broadphase} [{aveProfile.Broadphase}] ({MaxProfile.Broadphase})");
            }

            OnRender();
            DrawWorld();

            if (_stringBuilder.Length > 0)
            {
                Text = _stringBuilder.ToString();
                _stringBuilder.Clear();
            }
        }

        private void DrawWorld()
        {
            DrawFlag flags = 0;
            if (WorldSetting.Shape)
            {
                flags |= DrawFlag.DrawShape;
            }

            if (WorldSetting.Joint)
            {
                flags |= DrawFlag.DrawJoint;
            }

            if (WorldSetting.AABB)
            {
                flags |= DrawFlag.DrawAABB;
            }

            if (WorldSetting.Pair)
            {
                flags |= DrawFlag.DrawPair;
            }

            if (WorldSetting.CenterOfMass)
            {
                flags |= DrawFlag.DrawCenterOfMass;
            }

            if (WorldSetting.ContactPoint)
            {
                flags |= DrawFlag.DrawContactPoint;
            }

            WorldSetting.Drawer.Flags = flags;
            World.DebugDraw();
            if (WorldSetting.ContactPoint)
            {
                const float impulseScale = 0.1f;
                const float axisScale = 0.3f;
                for (var i = 0; i < PointsCount; ++i)
                {
                    var point = Points[i];
                    if (point.State == PointState.AddState)
                    {
                        // Add
                        WorldSetting.Drawer.DrawPoint(point.Position, 10f, Color.FromArgb(77, 242, 77));
                    }
                    else if (point.State == PointState.PersistState)
                    {
                        // Persist
                        WorldSetting.Drawer.DrawPoint(point.Position, 5f, Color.FromArgb(77, 77, 242));
                    }

                    if (WorldSetting.ContactNormals)
                    {
                        var p1 = point.Position;
                        var p2 = p1 + axisScale * point.Normal;
                        WorldSetting.Drawer.DrawSegment(p1, p2, Color.FromArgb(230, 230, 230));
                    }
                    else if (WorldSetting.ContactImpulse)
                    {
                        var p1 = point.Position;
                        var p2 = p1 + impulseScale * point.NormalImpulse * point.Normal;
                        WorldSetting.Drawer.DrawSegment(p1, p2, Color.FromArgb(230, 230, 77));
                    }

                    if (WorldSetting.FrictionImpulse)
                    {
                        var tangent = MathUtils.Cross(point.Normal, 1.0f);
                        var p1 = point.Position;
                        var p2 = p1 + impulseScale * point.TangentImpulse * tangent;
                        WorldSetting.Drawer.DrawSegment(p1, p2, Box2DSharp.Common.Color.FromArgb(230, 230, 77));
                    }
                }
            }

            if (BombSpawning)
            {
                WorldSetting.Drawer.DrawPoint(BombSpawnPoint, 4.0f, Box2DSharp.Common.Color.Blue);
                WorldSetting.Drawer.DrawSegment(MouseWorld, BombSpawnPoint, Box2DSharp.Common.Color.FromArgb(203, 203, 203));
            }
        }

        class MouseQueryCallback : IQueryCallback
        {
            public Fixture QueryFixture;

            public Vector2 Point;

            public void Reset(in Vector2 point)
            {
                QueryFixture = null;
                Point = point;
            }

            /// <inheritdoc />
            public bool QueryCallback(Fixture fixture1)
            {
                var body = fixture1.Body;
                if (body.BodyType == BodyType.DynamicBody)
                {
                    var inside = fixture1.TestPoint(Point);
                    if (inside)
                    {
                        QueryFixture = fixture1;

                        // We are done, terminate the query.
                        return false;
                    }
                }

                // Continue the query.
                return true;
            }
        }

        private readonly MouseQueryCallback _callback = new MouseQueryCallback();

        public void MouseDown()
        {
            if (MouseJoint != null)
            {
                return;
            }

            var p = MouseWorld;

            // Make a small box.
            var aabb = new AABB();
            var d = new Vector2(0.001f, 0.001f);
            aabb.LowerBound = p - d;
            aabb.UpperBound = p + d;

            // Query the world for overlapping shapes.
            _callback.Reset(p);
            World.QueryAABB(_callback, aabb);
            if (_callback.QueryFixture != null)
            {
                var body = _callback.QueryFixture.Body;
                var md = new MouseJointDef
                {
                    BodyA = GroundBody, BodyB = body,
                    Target = p, MaxForce = 1000.0f * body.Mass
                };
                MouseJoint = (MouseJoint) World.CreateJoint(md);
                body.IsAwake = true;
            }
        }

        public void MouseUp()
        {
            if (MouseJoint != null)
            {
                World.DestroyJoint(MouseJoint);
                MouseJoint = null;
            }

            if (BombSpawning)
            {
                CompleteBombSpawn(MouseWorld);
            }
        }

        public void ShiftMouseDown()
        {
            if (MouseJoint != null)
            {
                return;
            }

            SpawnBomb(MouseWorld);
        }

        public void JointDestroyed(Joint joint)
        { }
        
        /// Random number in range [-1,1]
        public float RandomFloat()
        {
            float r = _random.Next() & RandomLimit;
            r /= RandomLimit;
            r = 2.0f * r - 1.0f;
            return r;
        }

        /// Random floating point number in range [lo, hi]
        public float RandomFloat(float lo, float hi)
        {
            float r = _random.Next() & RandomLimit;
            r /= RandomLimit;
            r = (hi - lo) * r + lo;
            return r;
        }

        public void AddBody(Body body, string entityId)
        {
            if (body == null)
            {
                return;
            }
            if (entityMap.ContainsKey(body))
            {
                return;
            }
            entityMap[body] = entityId;
        }

        public void AddBody(Body body, GameObject go)
        {
            if (body == null)
            {
                return;
            }
            if (gameObjectMap.ContainsKey(body))
            {
                return;
            }

            gameObjectMap[body] = go;
        }
        public void RemoveBody(Body body)
        {
            if (!body.IsDisposed)
            {
                World.DestroyBody(body);
            }
        }

        public bool HasBody(Body body)
        {
            return World.HasBody(body);
        }

        public void OnRemoveBody(Action<Body> OnRemoveBody)
        {
            World.BodyRemoved += delegate (Body body)
            {
                OnRemoveBody(body);
            };
        }

        private void OnRemovedRigidBody(Body body)
        {
            this.RemoveObjectByBody(body);
            this.RemoveEntityByBody(body);

            for (int i = 0; i < body.FixtureList.Count; i++)
            {
                if (collisionInfo.ContainsKey(body.FixtureList[i]))
                {
                    var dict = collisionInfo[body.FixtureList[i]];
                    var values = dict.Values;
                    foreach (var item in values)
                    {
                        _collisionPool.Return(item);
                    }
                    dict.Clear();
                    collisionInfo.Remove(body.FixtureList[i]);
                }
            }
        }

        public void RemoveObjectByBody(Body body)
        {
            if (gameObjectMap.ContainsKey(body))
            {
                gameObjectMap.Remove(body);
            }
        }
        public void RemoveEntityByBody(Body body)
        {
            if (entityMap.ContainsKey(body))
            {
                entityMap.Remove(body);
            }
        }

        public GameObject GetGameObject(Body body)
        {
            if (!gameObjectMap.ContainsKey(body))
            {
                return null;
            }

            return gameObjectMap[body];
        }

        public string GetEntityId(Body body)
        {
            if (!entityMap.ContainsKey(body))
            {
                return null;
            }

            return entityMap[body];
        }

        public Body GetEntity(string entityId)
        {
            if (!entityMap.ContainsValue(entityId))
            {
                return null;
            }
            foreach (KeyValuePair<Body, string> kvp in entityMap)
            {
                if (kvp.Value.Equals(entityId))
                {
                    return kvp.Key;
                }
            }
            return null;
        }


        public int GetBodyLayer(Body body)
        {
            GameObject go = GetGameObject(body);
            if (go == null)
            {
                return -1;
            }

            return go.layer;
        }
        public struct ContactPoint
        {
            public Fixture FixtureA;

            public Fixture FixtureB;

            public Vector2 Normal;

            public Vector2 Position;

            public PointState State;

            public float NormalImpulse;

            public float TangentImpulse;

            public float Separation;
        }

        #region Bomb

        protected Vector2 BombSpawnPoint;

        protected bool BombSpawning;

        protected Body Bomb;

        public void SpawnBomb(Vector2 worldPt)
        {
            BombSpawnPoint = worldPt;
            BombSpawning = true;
        }

        public void CompleteBombSpawn(Vector2 p)
        {
            if (BombSpawning == false)
            {
                return;
            }

            const float multiplier = 30.0f;
            var vel = BombSpawnPoint - p;
            vel *= multiplier;
            LaunchBomb(BombSpawnPoint, vel);
            BombSpawning = false;
        }

        public void LaunchBomb()
        {
            var p = new Vector2(UnityEngine.Random.Range(-15.0f, 15.0f), 30.0f);
            var v = -5.0f * p;
            LaunchBomb(p, v);
        }

        public void LaunchBomb(Vector2 position, Vector2 velocity)
        {
            if (Bomb != default)
            {
                World.DestroyBody(Bomb);
                Bomb = default;
            }

            var bd = new BodyDef
            {
                BodyType = BodyType.DynamicBody, Position = position,
                Bullet = true
            };
            Bomb = World.CreateBody(bd);
            Bomb.SetLinearVelocity(velocity);

            var circle = new CircleShape {Radius = 0.3f};

            var fd = new FixtureDef
            {
                Shape = circle, Density = 20.0f,
                Restitution = 0.0f
            };

            Bomb.CreateFixture(fd);
        }

        #endregion

        public void Dispose()
        {
            World?.Dispose();
            World = null;
        }
    }
}