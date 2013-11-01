using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Waypoints;
using OurCityEngine.PhysicObjects;
using JigLibX.Collision;
using Microsoft.Xna.Framework;
using OurCityEngine.Hud;

namespace OurCityEngine.Mission.Tasks
{
    /// <summary>
    /// Simple go to waypoint task
    /// </summary>
    public class GoToWaypointTask : Task
    {
        Waypoint w;
        
        CarObject playerCar;
        
        public GoToWaypointTask(CarObject playerCar, Waypoint w, TimeSpan time, bool consumeTime, Mission parentMission) 
            : base(parentMission,time,consumeTime)
        {
            this.playerCar = playerCar;
            w.PhysicsSkin.callbackFn += new CollisionCallbackFn(WaypointCollisionHandler);
            this.w = w;
            this.Time = time;
            w.Visible = false;
            w.PhysicsBody.DisableBody();

            Objects.Add(w);
            
        }

        #region ITask Members
        /// <summary>
        /// Task start
        /// </summary>
        public override void Initialize()
        {

            base.Initialize(); // at last
        }

        /// <summary>
        /// Task is done
        /// </summary>
        public override void End() {

            base.End(); // at last
        }
        #endregion

        // colission handler
        public bool WaypointCollisionHandler(CollisionSkin o, CollisionSkin c) {
            if (c.Equals(playerCar.PhysicsSkin)) {
                Completed = true;
            } 
            return false;
        }

        
        
        #region ITask Members


        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // check conditions
            if (!Enabled) return;
            if (Completed) End();

            // show waypont on minimap
            Mission.hud.AddMinimapPoi(w.PhysicsBody.Position, Color.Yellow);
            
        }

        #endregion


        
    }
}
