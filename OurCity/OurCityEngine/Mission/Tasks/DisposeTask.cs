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
    public class DisposeTask : Task
    {
        List<PhysicObject> stuff = new List<PhysicObject>();
        bool onMinimap;
        CarObject playerCar;

        public DisposeTask(List<PhysicObject> stuff, bool onMinimap, TimeSpan time, bool consumeTime, Mission parentMission) 
            : base(parentMission,time,consumeTime)
        {
            this.stuff = stuff;
            this.Time = time;
            this.onMinimap = onMinimap;
            Objects.AddRange(stuff);            
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

        #region ITask Members


        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // check conditions
            if (!Enabled) return;
            if (Completed) End();

            // check if disposed + show on minimap
            List<PhysicObject> st = new List<PhysicObject>();
            st.AddRange(stuff);
            foreach (PhysicObject o in stuff) {
                Vector3 loc = o.PhysicsBody.Position;
                if (loc.Y < -40) st.Remove(o);
                else {
                    Mission.hud.AddMinimapPoi(o.PhysicsBody.Position, Color.GreenYellow);
                }
            }
            stuff.Clear();
            stuff.AddRange(st);

            // check if completed
            if (stuff.Count < 1) {
                Completed = true;
            }

        }

        #endregion


        
    }
}
