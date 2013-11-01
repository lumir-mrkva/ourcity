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
    /// mission 4 - waypoints with time limit (flat course)
    /// </summary>
    class Mission4 : Mission {

        private ContentManager content;
        private CarObject car;
        private Model waypointModel;

        public Mission4(Game game, CarObject car, IHud hud)
            : base(game, hud) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
        }

        protected override void LoadContent() {
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
        }

        public override void Initialize() {
            base.Initialize(); // first

            remainingTime = new TimeSpan(0, 0, 7);


            Waypoint entry = new Waypoint(Game, waypointModel, new Vector3(94.21614f, 0.2407234f, 363.18f));
            Task t1 = new GoToWaypointTask(car, entry, new TimeSpan(), false, this);
            t1.DoneMessage = new Message("Drive trought waypoints as fast as you can!", "Get the waypoints in time!");
            tasks.AddLast(t1);

            Waypoint w2 = new Waypoint(Game, waypointModel, new Vector3(-1.901785f, 0.2475788f, 612.1538f));
            tasks.AddLast(new GoToWaypointTask(car, w2, new TimeSpan(0, 0, 5), true, this));

            List<Vector3> locations = new List<Vector3>();
            locations.Add(new Vector3(-1.901785f, 0.2475788f, 612.1538f));
            locations.Add(new Vector3(-102.4215f, 0.2415622f, 678.15f));
            locations.Add(new Vector3(-104.0758f, 0.2454486f, 848.0155f));
            locations.Add(new Vector3(-4.996365f, 0.2395227f, 994.6099f));
            locations.Add(new Vector3(106.7987f, 0.2438538f, 875.3759f));
            locations.Add(new Vector3(150.7078f, 0.2412485f, 618.4578f));
            locations.Add(new Vector3(224.205f, 0.2408228f, 499.1378f));
            locations.Add(new Vector3(231.8389f, 0.2431216f, 153.2321f));
            locations.Add(new Vector3(268.4265f, 0.3003913f, -120.9193f));
            locations.Add(new Vector3(325.0365f, 0.2439411f, -282.2174f));
            locations.Add(new Vector3(257.9585f, 0.2491833f, -494.4867f));
            locations.Add(new Vector3(-1.867459f, 0.2536268f, -539.759f));
            locations.Add(new Vector3(50.8216f, 0.3083113f, -736.6711f));
            locations.Add(new Vector3(152.5396f, 0.2704311f, -677.6798f));
            locations.Add(new Vector3(95.40494f, 0.2429266f, -367.8463f));
            locations.Add(new Vector3(-0.6022212f, 0.3096106f, -177.7116f));
            locations.Add(new Vector3(-100.1592f, 0.2324649f, -63.4423f));
            locations.Add(new Vector3(-296.8296f, 0.2385921f, 56.908f));
            locations.Add(new Vector3(-139.7207f, 0.2890184f, 246.8282f));
            locations.Add(new Vector3(2.049286f, 0.2498577f, 311.3592f));
            locations.Add(new Vector3(88.52901f, 0.2427682f, 376.1607f));

            Waypoint w;
            for (int i = 1; i < locations.Count; i++) {
                w = new Waypoint(Game, waypointModel, locations[i]);
                tasks.AddLast(new GoToWaypointTask(car, w, GetTimeSpan(locations[i - 1], locations[i], i), true, this));
                tasks.Last.Value.DoMessage = new Message("Get the waypoints in time!", 0, Color.White, true);
            }

            DoneMessage = new Message("Well done, you are the fastest driver in town.");


        }

        public TimeSpan GetTimeSpan(Vector3 v1, Vector3 v2, int i) {
            float avgSpd = 30f;
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
