using Box2DSharp.Collision.Collider;
using Box2DSharp.Common;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System.Numerics;
using UnityEngine;

namespace Assets.Box2DEngine.Unity
{
    public class ContactPoint2D
    {

        /**
        *  @brief Contact point between two bodies
        **/
        public System.Numerics.Vector2 point;

        /**
        *  @brief Normal vector from the contact point
        **/
        public System.Numerics.Vector2 normal;

    }

    /**
    *  @brief Represents information about a contact between two 2D bodies
    **/
    public class BoxCollision2D
    {

        /**
        *  @brief An array of {@link TSContactPoint}
        **/
        public ContactPoint2D[] contacts = new ContactPoint2D[1];

        /**
        *  @brief {@link TSCollider} of the body hit
        **/
        //public TSCollider2D collider;

        /**
        *  @brief GameObject of the body hit
        **/
        public GameObject gameObject;

        public string entityAId;
        public string entityBId;

        public Body bodyA;
        public Body bodyB;

        public Fixture fixtureA;
        public Fixture fixtureB;

        /**
        *  @brief {@link TSRigidBody} of the body hit, if there is one attached
        **/
        public BoxBodyComponent rigidbody;

        /**
        *  @brief {@link TSTransform} of the body hit
        **/
        //public TSTransform2D transform;

        /**
        *  @brief The {@link TSTransform} of the body hit
        **/
        public System.Numerics.Vector2 relativeVelocity;

        internal void Reset()
        {
            gameObject = null;
            rigidbody = null;
            entityAId = null;
            entityBId = null;
            bodyA = null;
            bodyB = null;
            fixtureA = null;
            fixtureB = null;
            relativeVelocity = default;
        }

        internal void UpdateFixture(Fixture a, Fixture b, Contact c)
        {
            this.bodyA = a.Body;
            this.bodyB = b.Body;

            if (a == c.FixtureA)
            {
                fixtureA = c.FixtureA;
                fixtureB = c.FixtureB;
            }
            else
            {
                fixtureA = c.FixtureB;
                fixtureB = c.FixtureA;
            }
        }

        internal void UpdateRelativeVelocity()
        {
            System.Numerics.Vector2 result = System.Numerics.Vector2.Zero;
            if (fixtureA != null && fixtureB != null && fixtureA.Body != null && fixtureB.Body != null)
            {
                result.X = fixtureA.Body.LinearVelocity.X - fixtureB.Body.LinearVelocity.X;
                result.Y = fixtureA.Body.LinearVelocity.Y - fixtureB.Body.LinearVelocity.Y;
            }

            this.relativeVelocity = result;
        }

        internal void UpdateEntityId(string entityAId, string entityBId)
        {
            this.entityAId = entityAId;
            this.entityBId = entityBId;
        }

        internal void UpdateGameObject(GameObject otherGO)
        {
            if (this.gameObject == null && otherGO != null)
            {
                this.gameObject = otherGO;
                this.rigidbody = this.gameObject.GetComponent<BoxBodyComponent>();
            }
        }


        internal void Update(Contact c)
        {
            if (c != null)
            {
                if (contacts[0] == null)
                {
                    contacts[0] = new ContactPoint2D();
                }

                WorldManifold worldManifold;
                c.GetWorldManifold(out worldManifold);
                System.Numerics.Vector2 normal = worldManifold.Normal;
                FixedArray2<System.Numerics.Vector2> points = worldManifold.Points;

                if (this.bodyA == c.FixtureA.Body)
                {
                    contacts[0].normal = -normal;
                }
                else
                {
                    contacts[0].normal = normal;
                }

                contacts[0].point = points[0];
            }
        }

        public void GetRelativeVelocity(ref System.Numerics.Vector2 velc)
        {
            velc.Set(this.relativeVelocity.X, this.relativeVelocity.Y);
        }

        public void GetContactNormal(int index, ref System.Numerics.Vector2 normal)
        {
            normal.Set(this.contacts[index].normal.X, this.contacts[index].normal.Y);
        }

        public void GetContactPoint(int index, ref System.Numerics.Vector2 point)
        {
            point.Set(this.contacts[index].point.X, this.contacts[index].point.Y);
        }

    }
}
