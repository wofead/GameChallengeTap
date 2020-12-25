using UnityEngine;
using Box2DSharp.Dynamics;
using System.Numerics;
using Box2DSharp.Common;

namespace Assets.Box2DEngine.Unity
{
    public class BoxRaycastHit2D
    {
        public GameObject gameObject;

        public string entityId;

        public System.Numerics.Vector2 point;

        public System.Numerics.Vector2 normal;

        public Fixture fixture;

        public BoxBodyComponent rigidbody;

        public BoxTransformComponent transform;

        public void Reset()
        {
            gameObject = default;
            entityId = default;
            point = default;
            normal = default;
            fixture = default;
            rigidbody = default;
            transform = default;
        }

        public BoxRaycastHit2D Init(GameObject go, Fixture f, string entityId, System.Numerics.Vector2 point = default, System.Numerics.Vector2 normal = default)
        {
            this.gameObject = go;
            this.fixture = f;
            this.entityId = entityId;
            this.point = point;
            this.normal = normal;
            if (go != null)
            {
                this.rigidbody = go.GetComponent<BoxBodyComponent>();
                this.transform = go.GetComponent<BoxTransformComponent>();
            }
            return this;
        }

        public void Cross(in float s, ref System.Numerics.Vector2 result)
        {
            result.X = s * normal.Y;
            result.Y = -s * normal.X;
        }

        public void GetPoint(ref System.Numerics.Vector2 result)
        {
            result.X = this.point.X;
            result.Y = this.point.Y;
        }

        public void GetNormal(ref System.Numerics.Vector2 result)
        {
            result.X = this.normal.X;
            result.Y = this.normal.Y;
        }
    }
}
