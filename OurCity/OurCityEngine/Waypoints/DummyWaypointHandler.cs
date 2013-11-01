using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Utils;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Waypoints
{
    public class DummyWaypointHandler : DrawableGameComponent
    {
        WaypointManager wpm;
        CarObject playerCar;
        public Waypoint NextWaypoint
        {
            get;
            internal set;
        }
        public Waypoint PrevWaypoint
        {
            get;
            internal set;
        }

        TimeSpan time;
        bool timerActive;

        public DummyWaypointHandler(Game game, WaypointManager wpm, CarObject playerCar)
            : base(game)
        {
            this.wpm = wpm;
            this.playerCar = playerCar;

            Initialize();
        }

        public override void Initialize()
        {
            // Make all waypoints invisible
            foreach (Waypoint w in wpm.GetAllWaypoints())
            {
                w.Visible = false;
                w.PhysicsBody.DisableBody();
            }

            // Set next waypoint and make it visible
            NextWaypoint = wpm.GetAllWaypoints()[0];
            NextWaypoint.Visible = true;
            NextWaypoint.PhysicsBody.EnableBody();

            PrevWaypoint = null;
            time = TimeSpan.FromSeconds(10);

            timerActive = false;
        }

        public void HandleWaypointCollision(Waypoint w, CarObject c)
        {
            if (w.Equals(NextWaypoint) && c.Equals(playerCar))
            {
                NextWaypoint.Visible = false;
                NextWaypoint.PhysicsBody.DisableBody();
                Waypoint nextCandidate = wpm.GetAllWaypoints()[(wpm.GetAllWaypoints().IndexOf(NextWaypoint) + 1) % wpm.GetAllWaypoints().Count];

                float distance = Vector3.Distance(NextWaypoint.PhysicsBody.Position, nextCandidate.PhysicsBody.Position);
                Logger.Instance.Log("Distance: " + distance + ", time to accomplish: " + ((distance * 60 * 60 * 1000) / 70000) + ", new timespan: " + TimeSpan.FromHours(distance / 70000).ToString());
                time += TimeSpan.FromHours(distance / 70000);
                Logger.Instance.Log("New time from checkpoint "+time.ToString());
                timerActive = true;

                PrevWaypoint = NextWaypoint;
                NextWaypoint = nextCandidate;
                NextWaypoint.PhysicsBody.EnableBody();
                NextWaypoint.Visible = true;
            }
        }

        public float GetDistanceToNextWaypoint()
        {
            return Vector3.Distance(playerCar.PhysicsBody.Position, NextWaypoint.PhysicsBody.Position);
        }

        public TimeSpan GetRemainingTime()
        {
            return time;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;
            base.Update(gameTime);
            if (timerActive)
                time -= gameTime.ElapsedGameTime;

            if (time.TotalMilliseconds <= 0)
            {
                Initialize();
            }
        }
    }
}
