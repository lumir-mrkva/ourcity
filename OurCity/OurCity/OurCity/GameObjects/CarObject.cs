using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCity.GameObjects
{
    public abstract class CarObject : MovableObject
    {
        public CarObject(Game game)
            : base(game)
        {

        }
    }
}
