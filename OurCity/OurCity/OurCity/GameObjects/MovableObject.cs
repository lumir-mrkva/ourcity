using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCity.PhysicObjects;
using Microsoft.Xna.Framework;

namespace OurCity.GameObjects
{
    public abstract class MovableObject : GameObject
    {
        public MovableObject(Game game)
            : base(game)
        {

        }
    }
}
