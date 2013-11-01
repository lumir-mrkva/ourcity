using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCity.GameObjects
{
    public abstract class ObstacleObject : MovableObject
    {
        public ObstacleObject(Game game)
            : base(game)
        {

        }
    }
}
