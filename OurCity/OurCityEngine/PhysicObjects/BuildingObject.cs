#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JigLibX.Collision;
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Math;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace OurCityEngine.PhysicObjects
{
    /// <summary>
    /// Building object - slightly modified Box object
    /// </summary>
    public class BuildingObject : PhysicObject
    {
        
        public BuildingObject(Game game, Model model, Vector3 sideLengths, Matrix orientation, Vector3 position)
            : base(game, model)
        {
            body = new Body();
            collision = new CollisionSkin(body);

            collision.AddPrimitive(new Box(sideLengths, orientation, sideLengths), new MaterialProperties(0.8f, 0.8f, 0.7f));
            body.CollisionSkin = this.collision;
            Vector3 com = SetMass(1.0f);
            body.MoveTo(position, Matrix.Identity);
            collision.ApplyLocalTransform(new Transform(-com, Matrix.Identity));
            body.EnableBody();
            this.scale = sideLengths;
            body.Immovable = true;
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            //effect.DiffuseColor = color;
        }
    }
}


