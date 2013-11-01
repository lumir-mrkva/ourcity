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
    /// Third mission -  take care of nasty waste
    /// </summary>
    class Mission3 : Mission {

        private ContentManager content;
        private CarObject car;
        private Model waypointModel;
        private Model cylinderModel;
        List<Vector3> locations = new List<Vector3>();

        public Mission3(Game game, CarObject car, IHud hud)
            : base(game, hud) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
            this.SphereSize = 500;
        }

        protected override void LoadContent() {
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
            cylinderModel = content.Load<Model>("Objects/cylinder");
        }

        public override void Initialize() {
            base.Initialize(); // first

            remainingTime = new TimeSpan(0, 0, 7);

            Locations();
            
            List<PhysicObject> waste = new List<PhysicObject>();
            foreach (Vector3 l in locations) {
                waste.Add(new CylinderObject(Game, 1, 3, l, cylinderModel));
            }



            /// first checkpoint

            Waypoint entry = new Waypoint(Game, waypointModel,  new Vector3(-303, 0, -872));
            Task t1 = new GoToWaypointTask(car, entry, new TimeSpan(), false, this);
            t1.DoneMessage = new Message("City is in trouble!");
            tasks.AddLast(t1);
            
            tasks.AddLast(new DisposeTask(waste,true,new TimeSpan(0,4,10), true, this));
            tasks.Last.Value.DoMessage = new Message("Dispose of all nasty waste and fast!","Dispose waste.");
            tasks.Last.Value.DoneMessage = new Message("Congratulations, you made it!",5,Color.White,false);
            
        }

        public override void Reset() {
            base.Reset();
            Objects.Clear();
            locations.Clear();
            Initialize();
            startNextTask();
        }

        protected override void UnloadContent() {
            content.Unload();
        }

        private void Locations() {
            locations.Add(new Vector3(66.08356f, 0.2204068f, -5.476781f));
            locations.Add(new Vector3(-296.2548f, 0.2205567f, -189.387f));
            locations.Add(new Vector3(-292.2125f, 0.2204286f, -182.4046f));
            locations.Add(new Vector3(-179.9064f, 0.226613f, -789.7442f));
            locations.Add(new Vector3(-179.4271f, 0.2203986f, -794.4497f));
            locations.Add(new Vector3(239.3474f, 0.2213009f, -427.5146f));
            locations.Add(new Vector3(239.486f, 0.2204351f, -435.7713f));
            locations.Add(new Vector3(239.2423f, 0.2209904f, -421.2549f));
            locations.Add(new Vector3(180.8194f, 0.2230355f, 243.4099f));
            locations.Add(new Vector3(179.8457f, 0.2362397f, 249.2806f));
            locations.Add(new Vector3(-61.40409f, 0.2214435f, 968.6444f));
            locations.Add(new Vector3(-71.99778f, 0.2244644f, 966.8336f));
        }
    }
}
