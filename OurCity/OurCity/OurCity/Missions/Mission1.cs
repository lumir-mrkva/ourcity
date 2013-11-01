using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Mission;
using OurCityEngine.Mission.Tasks;
using Microsoft.Xna.Framework;
using OurCityEngine.Hud;
using OurCityEngine.Waypoints;
using OurCityEngine.PhysicObjects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace OurCity.Missions {
    /// <summary>
    /// First mission, simple 2 waypoints
    /// </summary>
    class Mission1 : Mission {

        private ContentManager content;
        private CarObject car;
        private Model waypointModel;
        
        public Mission1(Game game, CarObject car, IHud hud) : base(game, hud) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
        }

        protected override void LoadContent() {
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
        }

        public override void Initialize() {
            base.Initialize();

            /// mission start
            hud.Notify(new Message("Welcome to Our City."));
            DoneMessage = new Message("You just finished your first mission!");
            hud.Notify(DoMessage);
            

            Waypoint w1 = new Waypoint(Game, waypointModel,  new Vector3(10, 0, 30));
            Task t1 = new GoToWaypointTask(car, w1, new TimeSpan(), false, this);
            t1.DoMessage = new Message("To start mission, go to the waypoint on the map.", "Go to the waypoint.");
            tasks.AddLast(t1);

            Waypoint w2 = new Waypoint(Game, waypointModel, new Vector3(10, 0, 400));

            tasks.AddLast(new GoToWaypointTask(car, w2, new TimeSpan(), false, this));
            tasks.Last.Value.DoMessage = new Message("Go to the next waypoint.", 0, Color.White, true);
        }

        public override void Reset() {
            base.Reset();
            Initialize();
            startNextTask();
        }

        protected override void UnloadContent() {
            content.Unload();
        }
    }
}
