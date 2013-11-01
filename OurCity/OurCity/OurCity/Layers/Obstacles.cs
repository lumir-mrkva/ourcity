using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;

namespace OurCity.Layers {
    /// <summary>
    /// Layer full of obstacles and small things
    /// </summary>
    class Obstacles : Layer {
        ContentManager content;
        Model boxModel;

        public Obstacles(Game game)
            : base(game) {
            content = new ContentManager(game.Services, "Content");
            boxModel = content.Load<Model>("Waypoints/waypoint_box");
        }


        protected override void LoadContent() {
            Vector3 loc = new Vector3(-13, 0, 422);
            for (int i = 0; i < 10; i += 2) {
                Objects.Add(new BoxObject(Game, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(0, i * 1f, 1)+loc));
                Objects.Add(new BoxObject(Game, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(1, i * 1f, 1) + loc));
                Objects.Add(new BoxObject(Game, boxModel, new Vector3(1, 1f, 3), Matrix.Identity, new Vector3(2, i * 1f, 1) + loc));


                Objects.Add(new BoxObject(Game, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f, 0) + loc));
                Objects.Add(new BoxObject(Game, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f, 1) + loc));
                Objects.Add(new BoxObject(Game, boxModel, new Vector3(3, 1f, 1), Matrix.Identity, new Vector3(1, i * 1f + 1f, 2) + loc));
            }
        }


        protected override void UnloadContent() {
            content.Unload();
        }
    }
}
