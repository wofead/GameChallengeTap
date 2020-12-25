using Box2DSharp;
using Box2DSharp.Collision.Shapes;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Inspection;
using UnityEngine;
using System.Numerics;
namespace Assets.Box2DEngine.Unity
{
    public class BoxBodyComponent : MonoBehaviour
    {
        public delegate void OnSyncedCollisionDelegate(BoxCollision2D collision2D);
        public event OnSyncedCollisionDelegate OnSyncedCollisionEnterEvent = null;
        public event OnSyncedCollisionDelegate OnSyncedCollisionStayEvent = null;
        public event OnSyncedCollisionDelegate OnSyncedCollisionExitEvent = null;

        public delegate void OnTriggerCollisionDelegate(BoxCollision2D collision2D);
        public event OnTriggerCollisionDelegate OnSyncedTriggerEnterEvent = null;
        public event OnTriggerCollisionDelegate OnSyncedTriggerStayEvent = null;
        public event OnTriggerCollisionDelegate OnSyncedTriggerExitEvent = null;

        protected Body body;

        private const float DELTA_TIME_FACTOR = 10f;

        public enum InterpolateMode { None, Interpolate, Extrapolate };

        [SerializeField]
        public MassData _massData = new MassData { Mass = 1.0f };

        [SerializeField]
        public BodyDef _bodyDef = new BodyDef { BodyType = BodyType.DynamicBody };

        private UnityEngine.Transform mapTransform;
        /**
            *  @brief Mass of the body. 
            **/
        public float mass
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.Mass;
                }

                return _massData.Mass;
            }

            set
            {
                _massData.Mass = value;

                if (this._useAutoMass)
                {
                    return;
                }

                if (this.body != null)
                {
                    this.body.SetMassData(_massData);
                }
            }
        }

        [SerializeField]
        private bool _useGravity = true;

        /**
            *  @brief If true it uses gravity force. 
            **/
        public bool useGravity
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.IgnoreGravity;
                }

                return _useGravity;
            }

            set
            {
                _useGravity = value;

                if (this.body != null)
                {
                    this.body.IgnoreGravity = _useGravity;
                }
            }
        }

        public float GravityScale { get; private set; }

        public BodyType bodyType
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.BodyType;
                }

                return _bodyDef.BodyType;
            }

            set
            {
                _bodyDef.BodyType = value;

                if (this.body != null)
                {
                    this.body.BodyType = value;
                }
            }
        }

        private bool useAutoMass;

        /**
            *  @brief inertia of the body. 
            **/
        public float inertia
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.Inertia;
                }

                return _massData.RotationInertia;
            }

            set
            {
                _massData.RotationInertia = value;

                if (this.body != null)
                {
                    this.body.SetMassData(_massData);
                }
            }
        }

        [SerializeField]
        private bool _useAutoMass = false;
        [SerializeField]
        private Category _collisionCategories = Settings.DefaultFixtureCollisionCategories;
        public Category CollisionCategories
        {
            get
            {
                return _collisionCategories;
            }
            set
            {
                _collisionCategories = value;
                if (this.body != null)
                {
                    this.body.CollisionCategories = value;
                }
            }
        }

        [SerializeField]
        private Category _collidesWith = Settings.DefaultFixtureCollidesWith;
        public Category CollidesWith
        {
            get
            {
                return _collidesWith;
            }
            set
            {
                _collidesWith = value;
                if (this.body != null)
                {
                    this.body.CollidesWith = value;
                }
            }
        }

        /**
            *  @brief Linear drag coeficient.
            **/
        public float drag
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.LinearDamping;
                }

                return _bodyDef.LinearDamping;
            }

            set
            {
                _bodyDef.LinearDamping = value;

                if (this.body != null)
                {
                    this.body.LinearDamping = value;
                }
            }
        }

        /**
            *  @brief Angular drag coeficient.
            **/
        public float angularDrag
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.AngularDamping;
                }

                return _bodyDef.AngularDamping;
            }

            set
            {
                _bodyDef.AngularDamping = value;

                if (this.body != null)
                {
                    this.body.AngularDamping = value;
                }
            }
        }
        public void SetMapTransform(UnityEngine.Transform transform)
        {
            mapTransform = transform;
        }

        public bool FixedRotation
        {
            get
            {
                if (this.body != null)
                {
                    return this.body.HasFlag(BodyFlags.FixedRotation);
                }

                return _bodyDef.FixedRotation;
            }

            set
            {
                _bodyDef.FixedRotation = value;

                if (this.body != null)
                {
                    if (value)
                    {
                        this.body.SetFlag(BodyFlags.FixedRotation);
                    }
                    else
                    {
                        this.body.UnsetFlag(BodyFlags.FixedRotation);
                    }
                }
            }
        }


        /**
            *  @brief Interpolation mode that should be used. 
            **/
        public BoxBodyComponent.InterpolateMode interpolation;


        public void Start()
        {
            if (this.body != null)
                return;

            var rigid = this.GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                if (rigid.bodyType == RigidbodyType2D.Static)
                {
                    this._bodyDef.BodyType = BodyType.StaticBody;
                }
                if (rigid.bodyType == RigidbodyType2D.Kinematic)
                {
                    this._bodyDef.BodyType = BodyType.KinematicBody;
                }
                if (rigid.bodyType == RigidbodyType2D.Dynamic)
                {
                    this._bodyDef.BodyType = BodyType.DynamicBody;
                }
                this.useAutoMass = rigid.useAutoMass;
                this.inertia = rigid.inertia;
                this.mass = rigid.mass;
                this.GravityScale = rigid.gravityScale;
                this.drag = rigid.drag;
                this.angularDrag = rigid.angularDrag;
                this._massData.Center = rigid.centerOfMass.ToVector2();



                Object.Destroy(rigid);
            }

            World world = PhysicsFight.instance.World;
            //body = BodyFactory.CreateRectangle(FSWorldComponent.PhysicsWorld, 1f, 1f, Density);
            body = world.CreateBody(this._bodyDef);

            var colliders = this.GetComponents<UnityEngine.Collider2D>();
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] is BoxCollider2D)
                {
                    var shape = FSHelper.GetBoxShape(colliders[i] as BoxCollider2D);
                    Fixture f = body.CreateFixture(shape, colliders[i].density);
                    this.UpdateFixture(f, colliders[i]);
                    f.Friction = colliders[i].friction;
                    f.Restitution = colliders[i].bounciness;
                    f.IsSensor = colliders[i].isTrigger;
                }
                else if (colliders[i] is CircleCollider2D)
                {
                    var shape = FSHelper.GetCircleShape(colliders[i] as CircleCollider2D);
                    Fixture f = body.CreateFixture(shape, colliders[i].density);
                    this.UpdateFixture(f, colliders[i]);
                    f.Friction = colliders[i].friction;
                    f.Restitution = colliders[i].bounciness;
                    f.IsSensor = colliders[i].isTrigger;
                }
                else if (colliders[i] is PolygonCollider2D)
                {
                    var shapes = FSHelper.GetPolygonShape(colliders[i] as PolygonCollider2D);
                    for (int j = 0; j < shapes.Length; j++)
                    {
                        Fixture f = body.CreateFixture(shapes[j], colliders[i].density);
                        this.UpdateFixture(f, colliders[i]);
                        f.Friction = colliders[i].friction;
                        f.Restitution = colliders[i].bounciness;
                        f.IsSensor = colliders[i].isTrigger;
                    }
                }
            }



            body.IgnoreGravity = !this._useGravity;

            body.CollisionCategories = this._collisionCategories;
            body.CollidesWith = this._collidesWith;
            //body.IsBullet = true;

            body.SetTransform(new System.Numerics.Vector2(transform.position.x, transform.position.y), transform.rotation.eulerAngles.z * Mathf.Deg2Rad);
            body.Scale = transform.lossyScale.ToVector2();

            body.UseAutoMass = this._useAutoMass;
            body.SetMassData(_massData);
            body.LinearVelocity = this._velocity;
            body.AngularVelocity = this._angularVelocity;
            body.IsEnabled = true;
            body.IsBullet = false;


            PhysicsFight.instance.AddBody(body, this.gameObject);

        }

        void UpdateFixture(Fixture fixture, Collider2D collider)
        {
            var pos = collider.transform.position - this.transform.position;
            var scale1 = collider.transform.lossyScale;
            var scale2 = this.transform.lossyScale;
            var scale = new System.Numerics.Vector3(scale1.x / scale2.x, scale1.y / scale2.y, 1);
            var angle = collider.transform.rotation.eulerAngles.z - this.transform.eulerAngles.z;
            fixture.LocalTransform(pos.ToVector2(), scale.ToVector2(), angle);
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isPlaying)
            {
                if (this.body != null)
                {
                    if (this.body.IsDisposed || this.body.World != PhysicsFight.instance.World)
                    {
                        this.Dispose();
                        return;
                    }
                    if (this.body.IsEnabled)
                        UpdatePlayMode();
                }
            }
        }

        public void UpdatePlayMode()
        {
            body.Scale = transform.lossyScale.ToVector2();

            var pos = position.ToVector();

            if (this.interpolation == BoxBodyComponent.InterpolateMode.Interpolate)
            {
                transform.position = System.Numerics.Vector3.Lerp(transform.position, position.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, UnityEngine.Quaternion.Euler(0, 0, rotation.AsFloat()), Time.deltaTime * DELTA_TIME_FACTOR);
                //transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                return;
            }
            else if (this.interpolation == BoxBodyComponent.InterpolateMode.Extrapolate)
            {
                transform.position = (position + this.body.LinearVelocity * Time.deltaTime * DELTA_TIME_FACTOR).ToVector();
                transform.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, UnityEngine.Quaternion.Euler(0, 0, rotation.AsFloat()), Time.deltaTime * DELTA_TIME_FACTOR);
                //transform.localScale = Vector3.Lerp(transform.localScale, scale.ToVector(), Time.deltaTime * DELTA_TIME_FACTOR);
                return;
            }
            //this.enabled
            if (mapTransform)
            {
                pos = pos + mapTransform.position;
            }
            transform.position = pos;

            UnityEngine.Quaternion rot = transform.rotation;
            rot.eulerAngles = new System.Numerics.Vector3(0, 0, rotation.AsFloat());
            transform.rotation = rot;

            //transform.localScale = scale.ToVector();
        }

        protected virtual void OnDestroy()
        {
            // destroy this body on Farseer Physics
            Dispose();
        }

        public void Dispose()
        {
            if (this.body != null && this.body.World != null)
            {
                this.body.World.DestroyBody(this.body);
            }
            this.body = null;
        }

        /**
        *  @brief Applies the provided force in the body. 
        *  
        *  @param force A {@link TSVector2} representing the force to be applied.
        **/
        public void AddForce(System.Numerics.Vector2 force)
        {
            AddForce(force, ForceMode.Force);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param mode Indicates how the force should be applied.
         **/
        public void AddForce(System.Numerics.Vector2 force, ForceMode mode)
        {
            if (mode == ForceMode.Force)
            {
                this.body.ApplyForceToCenter(force, true);
            }
            else if (mode == ForceMode.Impulse)
            {
                this.body.ApplyLinearImpulseToCenter(force, true);
            }
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(System.Numerics.Vector2 force, System.Numerics.Vector2 position)
        {
            AddForceAtPosition(force, position, ForceMode.Impulse);
        }

        /**
         *  @brief Applies the provided force in the body. 
         *  
         *  @param force A {@link TSVector2} representing the force to be applied.
         *  @param position Indicates the location where the force should hit.
         **/
        public void AddForceAtPosition(System.Numerics.Vector2 force, System.Numerics.Vector2 position, ForceMode mode)
        {
            if (mode == ForceMode.Force)
            {
                this.body.ApplyForce(force, position, true);
            }
            else if (mode == ForceMode.Impulse)
            {
                this.body.ApplyLinearImpulse(force, position, true);
            }
        }

        /**
         *  @brief Returns the velocity of the body at some position in world space. 
         **/
        public System.Numerics.Vector2 GetPointVelocity(System.Numerics.Vector2 worldPoint)
        {
            System.Numerics.Vector3 directionPoint = new System.Numerics.Vector3(position - this.body.Position, 0);
            return System.Numerics.Vector3.Cross(new System.Numerics.Vector3(0, 0, body.AngularVelocity), directionPoint).ToVector2() + this.body.LinearVelocity;
        }

        /**
         *  @brief Simulates the provided tourque in the body. 
         *  
         *  @param torque A {@link TSVector2} representing the torque to be applied.
         **/
        public void AddTorque(float torque)
        {
            this.body.ApplyTorque(torque, true);
        }

        /**
         *  @brief Moves the body to a new position. 
         **/
        public void MovePosition(System.Numerics.Vector2 position)
        {
            this.position = position;
        }

        /**
         *  @brief Rotates the body to a provided rotation. 
         **/
        public void MoveRotation(float rot)
        {
            this.rotation = rot;
        }

        /**
        *  @brief Position of the body. 
        **/
        public System.Numerics.Vector2 position
        {
            get
            {
                if (this.body != null)
                    return this.body.Position;

                return this.transform.position.ToVector2();
            }

            set
            {
                if (this.body != null)
                    this.body.Position = value;

                this.transform.position = value.ToVector();
            }
        }

        /**
        *  @brief Orientation of the body. 
        **/
        public float rotation
        {
            get
            {
                if (this.body != null)
                    return this.body.Rotation * FP.Rad2Deg;

                return this.transform.rotation.eulerAngles.z;
            }

            set
            {
                if (this.body != null)
                    this.body.Rotation = value * Mathf.Deg2Rad;

                this.transform.rotation = UnityEngine.Quaternion.Euler(0, 0, value);
            }
        }

        private System.Numerics.Vector2 _velocity;
        /**
        *  @brief LinearVelocity of the body. 
        **/
        public System.Numerics.Vector2 velocity
        {
            get
            {
                if (this.body != null)
                    return this.body.LinearVelocity;

                return _velocity;
            }

            set
            {
                _velocity = value;

                if (this.body != null)
                    this.body.LinearVelocity = value;
            }
        }

        private float _angularVelocity;
        /**
        *  @brief AngularVelocity of the body (radians/s). 
        **/
        public float angularVelocity
        {
            get
            {
                if (this.body != null)
                    return this.body.AngularVelocity;

                return _angularVelocity;
            }

            set
            {
                if (this.body != null)
                    this.body.AngularVelocity = value;

                _angularVelocity = value;

            }
        }

        public Body PhysicsBody
        {
            get
            {
                if (this.body == null || this.body.IsDisposed || this.body.World != PhysicsFight.instance.World)
                {
                    this.body = null;
                    Start();
                }

                return body;
            }
            set
            {
                this.body = value;
                PhysicsFight.instance.AddBody(this.body, this.gameObject);
            }
        }

        #region OnSyncedCollision

        void OnSyncedCollisionEnter(BoxCollision2D collision)
        {
            if (OnSyncedCollisionEnterEvent != null)
            {
                OnSyncedCollisionEnterEvent(collision);
            }
        }

        void OnSyncedCollisionStay(BoxCollision2D collision)
        {
            if (OnSyncedCollisionStayEvent != null)
            {
                OnSyncedCollisionStayEvent(collision);
            }
        }

        void OnSyncedCollisionExit(BoxCollision2D collision)
        {
            if (OnSyncedCollisionExitEvent != null)
            {
                OnSyncedCollisionExitEvent(collision);
            }
        }

        void OnSyncedTriggerEnter(BoxCollision2D collision)
        {
            if (OnSyncedTriggerEnterEvent != null)
            {
                OnSyncedTriggerEnterEvent(collision);
            }
        }

        void OnSyncedTriggerStay(BoxCollision2D collision)
        {
            if (OnSyncedTriggerStayEvent != null)
            {
                OnSyncedTriggerStayEvent(collision);
            }
        }

        void OnSyncedTriggerExit(BoxCollision2D collision)
        {
            if (OnSyncedTriggerExitEvent != null)
            {
                OnSyncedTriggerExitEvent(collision);
            }
        }

        #endregion

    }
}
