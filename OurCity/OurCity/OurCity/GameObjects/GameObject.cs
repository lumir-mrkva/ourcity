using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Physics;
using Microsoft.Xna.Framework.Graphics;

namespace OurCity.GameObjects
{
    public abstract class GameObject : DrawableGameComponent
    {

        protected Body body;
        protected CollisionSkin collision;

        protected Model model;
        protected Vector3 color;

        protected Vector3 scale = Vector3.One;

        public Body PhysicsBody { get { return body; } }
        public CollisionSkin PhysicsSkin { get { return collision; } }

        protected static Random random = new Random();
        
        public GameObject(Game game)
            : base(game) 
        {
            
        }
    }
}
