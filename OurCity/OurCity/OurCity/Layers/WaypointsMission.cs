using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework.Graphics;
using OurCityEngine.Waypoints;
using OurCityEngine.Screens;
using OurCity.Screens;

namespace OurCity.Layers {
    /// <summary>
    /// Simple waypoint mission
    /// </summary>
    public class WaypointsMission : Layer {

        ContentManager content;
        Model waypointModel;
        CarObject car;
        HudOverlay hud;


        WaypointManager wpManager;

        public DummyWaypointHandler Handler { get { return wpHandler; } internal set { } }
        DummyWaypointHandler wpHandler;


        List<Vector3> positions = new List<Vector3>();



        /// <summary>
        /// Constructor with default waypoint locations
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="car">Watched car</param>
        public WaypointsMission(Game game, CarObject car, HudOverlay hud)
            : base(game) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
            this.hud = hud;

            // default positions
            positions.Add(new Vector3(102.1698f, -20, 65.49726f));
            positions.Add(new Vector3(102.3883f, 0.22563f, 193.8246f));
            positions.Add(new Vector3(107.5732f, 0.2405609f, 938.9814f));
            positions.Add(new Vector3(88.17076f, 0.2415653f, 958.3015f));
            positions.Add(new Vector3(-92.6205f, 0.2404301f, 960.243f));
            positions.Add(new Vector3(-109.479f, 0.3087659f, 945.4579f));
            positions.Add(new Vector3(-104.5664f, 0.2465245f, 830.2363f));
            positions.Add(new Vector3(-109.1518f, 0.2462023f, 328.932f));
            positions.Add(new Vector3(-110.2776f, -20, 223.4564f));
            positions.Add(new Vector3(-60.85434f, -20, 171.6528f));
            positions.Add(new Vector3(-15.97892f, -20, 106.8634f));
            positions.Add(new Vector3(25.59191f, -20, 5.352768f));
            positions.Add(new Vector3(53.71033f, -20, -35.43206f));
            positions.Add(new Vector3(91.76981f, -20, -32.53667f));
        }

        /// <summary>
        /// Constructor with specified waypoint positions
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="car">Watched car</param>
        /// <param name="positions">Waypoint positions</param>
        public WaypointsMission(Game game, CarObject car, HudOverlay hud, List<Vector3> positions)
            : base(game) {
            content = new ContentManager(game.Services, "Content");
            this.car = car;
            this.hud = hud;
            this.positions.AddRange(positions);
        }
        
        protected override void LoadContent()
        {
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
            wpManager = new WaypointManager();


            List<Waypoint> wps = new List<Waypoint>();
            foreach (Vector3 v in positions) {
                wps.Add(new Waypoint(Game, waypointModel, new Vector3(10, 20, 10), Matrix.Identity, v));
            }

            foreach (Waypoint w in wps) {
                Objects.Add(w);
                wpManager.AddWatchedWaypoint(w);
            }

            wpManager.AddWatchedCar(car);


            // Register handler to be notified
            wpHandler = new DummyWaypointHandler(Game, wpManager, car);
            wpManager.WaypointCollision += wpHandler.HandleWaypointCollision;            
        }

        public override void Update(GameTime gameTime) {
            
            Handler.Update(gameTime);

            float distance = Vector3.Distance(car.PhysicsBody.Position, new Vector3(0, 0, 0)) - 80;
            float distanceToWP = wpHandler.GetDistanceToNextWaypoint();

            #region HUD
            if (hud == null) return;

            // show waypoint on minimap
            hud.AddMinimapPoi(wpHandler.NextWaypoint.PhysicsBody.Position, wpHandler.NextWaypoint.Color);

            // print message in hud
            if (wpHandler.GetRemainingTime().TotalSeconds == 10) {
            } else {
                hud.AddMainMessage("Next WP " + Math.Round(distanceToWP) + " m");

                if (wpHandler.GetRemainingTime().TotalMilliseconds > 0)
                    hud.AddMainMessage("Time: " + String.Format("{0:0.00}",wpHandler.GetRemainingTime().TotalSeconds, 2));
                else
                    hud.AddMainMessage("You lost!");
            }
            #endregion
        }

        public void ResetCar(Vector3 defaultPos) {
            if (wpHandler.GetRemainingTime().TotalMilliseconds > 0)
            { // We still have time
                // Respawn on last waypoint
                Vector3 newPos;
                if (wpHandler.PrevWaypoint == null)
                {
                    newPos = defaultPos;
                }
                else
                {
                    newPos = wpHandler.PrevWaypoint.PhysicsBody.Position;
                }

                car.Move(newPos);
            }
            else
            { // We lost or havent even started
                car.Move(defaultPos);
                wpHandler.Initialize();
            }
        }

        protected override void UnloadContent() 
        {
            content.Unload();
        }
    }
    
}
