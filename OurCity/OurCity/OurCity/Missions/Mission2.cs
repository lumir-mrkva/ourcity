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
    /// Second mission - waypoints with time limit
    /// </summary>
    class Mission2 : Mission {

        private ContentManager content;
        private CarObject car;
        private Model waypointModel;
        
        public Mission2(Game game, CarObject car, IHud hud) : base(game, hud) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
        }

        protected override void LoadContent() {
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
        }

        public override void Initialize() {
            base.Initialize(); // first

            remainingTime = new TimeSpan(0, 0, 7);
            

            Waypoint entry = new Waypoint(Game, waypointModel,  new Vector3(-203, 0, 731));
            Task t1 = new GoToWaypointTask(car, entry, new TimeSpan(), false, this);
            t1.DoneMessage = new Message("Welcome to another mission!");
            tasks.AddLast(t1);

            Waypoint w2 = new Waypoint(Game, waypointModel, new Vector3(-107, 0, 498));
            tasks.AddLast(new GoToWaypointTask(car, w2, new TimeSpan(0,0,5), true, this));
            tasks.Last.Value.DoMessage = new Message("Guess what you have to do.","Get the waypoints in time!");
            
            List<Vector3> locations = new List<Vector3>();
            locations.Add(new Vector3(-107, 0, 498));
            locations.Add(new Vector3(-91.0041f, -19.76195f, 257.3348f));
            locations.Add(new Vector3(102.3996f, 20.27918f, 247.5118f));
            locations.Add(new Vector3(318.0446f, 0.2403644f, 305.4154f));
            locations.Add(new Vector3(229.0668f, 0.2408435f, 494.5981f));
            locations.Add(new Vector3(92.94297f, 0.2391669f, 360.0591f));
            locations.Add(new Vector3(92.42701f, -19.75922f, 24.43032f));
            locations.Add(new Vector3(218.9947f, 0.240454f, -312.432f));
            locations.Add(new Vector3(30.15414f, 20.2967f, -368.9315f));
            locations.Add(new Vector3(-285.254f, 20.24482f, -616.9044f));
            locations.Add(new Vector3(-106.7512f, 0.2540664f, -864.9634f));
            locations.Add(new Vector3(-93.57486f, 20.24141f, -469.8819f));

            Waypoint w;
            for (int i = 1; i < locations.Count; i++) {
                w = new Waypoint(Game, waypointModel, locations[i]);
                tasks.AddLast(new GoToWaypointTask(car, w, GetTimeSpan(locations[i-1],locations[i],i), true, this));
                tasks.Last.Value.DoMessage = new Message("Get the waypoints in time!",0,Color.White,true);
            }

            DoneMessage = new Message("Wow, you got some serious skill.");


        }

        public TimeSpan GetTimeSpan(Vector3 v1, Vector3 v2, int i) {
            float avgSpd = 22f+i;
            float distance = Vector3.Distance(v1, v2);
            return new TimeSpan(0, 0, (int)(distance / avgSpd));
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
