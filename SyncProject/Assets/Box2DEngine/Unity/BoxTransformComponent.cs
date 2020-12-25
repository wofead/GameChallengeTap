using UnityEngine;
using Box2DSharp.Dynamics;
using System.Numerics;
using Box2DSharp;
using Box2DSharp.Common;
using Box2DSharp.Inspection;

namespace Assets.Box2DEngine.Unity
{
    public class BoxTransformComponent: MonoBehaviour
    {
        private const float DELTA_TIME_FACTOR = 10f;

        protected Body body;
        private UnityEngine.Transform mapTransform;


        public UnityEngine.Transform RotationNode;

        public Body PhysicsBody
        {
            get
            {
                return body;
            }
            set
            {
                body = value;
                PhysicsFight.instance.AddBody(value, this.gameObject);
                //FixedUpdate();
                //LateUpdate();
                Update();
            }
        }

        public BoxBodyComponent.InterpolateMode interpolation;

        //private void FixedUpdate()
        //{
        //    if (Application.isPlaying)
        //    {
        //        if (this.body != null && this.body.IsActive && !this.body.IsDisposed)
        //        {
        //            UpdatePlayMode();
        //        }
        //    }
        //}

        //private void LateUpdate()
        //{
        //    if (Application.isPlaying)
        //    {
        //        if (this.body != null && this.body.IsActive && !this.body.IsDisposed)
        //        {
        //            UpdatePlayMode();
        //        }
        //    }
        //}
        void Update()
        {
            if (Application.isPlaying)
            {
                if (this.body != null && this.body.IsEnabled && !this.body.IsDisposed)
                {
                    UpdatePlayMode();
                }
            }
        }

        public void ForceUpdate()
        {
            if (this.body != null && this.body.IsEnabled && !this.body.IsDisposed)
            {
                UpdatePlayMode();
            }
        }

        public void SetMapTransform(UnityEngine.Transform transform)
        {
            mapTransform = transform;
        }

        public System.Numerics.Vector2 position
        {
            get
            {
                return this.body.GetPosition();
            }

            set
            {
                this.body.Transform.Position = value;
            }
        }

        /**
        *  @brief Orientation of the body. 
        **/
        public float rotation
        {
            get
            {
                return this.body.Rotation * UnityEngine.Mathf.Rad2Deg;
            }

            set
            {
                this.body.Rotation = value * UnityEngine.Mathf.Deg2Rad;
            }
        }

        public void UpdatePlayMode()
        {
            UnityEngine.Transform rotationNode = this.transform;
            if (RotationNode != null)
            {
                rotationNode = RotationNode;
            }

            var pos = position.ToUnityVector3();

            if (this.interpolation == BoxBodyComponent.InterpolateMode.Interpolate)
            {
                transform.position = UnityEngine.Vector3.Lerp(transform.position, position.ToUnityVector2(), Time.deltaTime * DELTA_TIME_FACTOR);
                rotationNode.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, UnityEngine.Quaternion.Euler(0, 0, rotation), Time.deltaTime * DELTA_TIME_FACTOR);
            }
            else if (this.interpolation == BoxBodyComponent.InterpolateMode.Extrapolate)
            {
                transform.position = (position + this.body.LinearVelocity * Time.deltaTime * DELTA_TIME_FACTOR).ToUnityVector2();
                rotationNode.rotation = UnityEngine.Quaternion.Lerp(transform.rotation, UnityEngine.Quaternion.Euler(0, 0, rotation), Time.deltaTime * DELTA_TIME_FACTOR);
            }
            //this.enabled
            var bodyR = UnityEngine.Quaternion.Euler(0, 0, this.PhysicsBody.Angle);
            if (mapTransform)
            {
                pos = pos + mapTransform.position;
                bodyR = UnityEngine.Quaternion.Euler(0, 0, this.transform.rotation.eulerAngles.z + this.PhysicsBody.Angle);
            }
            transform.position = pos;
            rotationNode.localRotation = bodyR;
            var s = PhysicsBody.Scale.ToUnityVector3();
            s.z = 1.0f;
            rotationNode.localScale = s;

        }

        protected virtual void OnDestroy()
        {

            // destroy this body on Farseer Physics
            //World world = (World)Physics2DWorldManager.instance.GetWorld();
            if (PhysicsBody != null)
            {
                PhysicsFight.instance.RemoveObjectByBody(PhysicsBody);
            }
        }
    }
}
