using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;
using Microsoft.Xna.Framework.Graphics;

namespace OurCityEngine.Waypoints
{
    /// <summary>
    /// Waypoint 
    /// </summary>
    public class Waypoint : BoxObject
    {
        public Waypoint(Game game, Model model, Vector3 sideLengths, Matrix orientation, Vector3 position)
            : base(game, model, sideLengths, orientation, position)
        {
            this.body.Immovable = true;
            this.SetMass(0);
            this.body.SetInactive();
        }

        public Waypoint(Game game, Model model, Vector3 position)
            : base(game, model, new Vector3(10, 10, 10), Matrix.Identity, position + new Vector3(0,5,0)) {
            this.body.Immovable = true;
            this.SetMass(0);
            this.body.SetInactive();
        }

        public override void ApplyEffects(BasicEffect effect) {
            effect.Alpha = 0.5f;
            effect.DiffuseColor = color;
        }

        public Color Color { get { return new Color(color); } internal set {} }

    }
}
