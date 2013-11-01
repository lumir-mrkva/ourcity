using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Hud;
using OurCityEngine.PhysicObjects;

namespace OurCityEngine.Mission {
    /// <summary>
    /// Generic task class
    /// </summary>
    public abstract class Task : ITask {

        public bool Enabled { get; set; }
        public bool Completed { get; set; }
        public string UpdateText { get; set; }
        public Message DoMessage { get; set; }
        public Message DoneMessage { get; set; }
        protected Mission Mission {get;set;}
        public TimeSpan Time {get;set;}
        public List<PhysicObject> Objects = new List<PhysicObject>();

        public Task(Mission parentMission) {
            Completed = false;
            Enabled = false;
            consumeTime = false;
            Mission = parentMission;
        }
        
        public Task(Mission parentMission, TimeSpan time, bool consumeTime) 
            : this(parentMission) {
            this.consumeTime = consumeTime;
            Time = time;
        }

        public virtual void Initialize() {
            Mission.Objects.AddRange(Objects);
            if (DoesConsumeTime()) {
                Mission.AddTime(Time);
            }
            Enabled = true;
        }

        public virtual void End() {
            Kill();
            Mission.startNextTask();
        }

        public virtual void Kill() {
            foreach (PhysicObject o in Objects) {
                o.PhysicsBody.DisableBody();
                Mission.Objects.Remove(o);
            }
            Enabled = false;
        }

        public abstract void Update(Microsoft.Xna.Framework.GameTime gameTime);


        public bool DoesConsumeTime() {
            return consumeTime;
        }
        bool consumeTime;

    }
}
