using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework;
using OurCityEngine.Utils;
using OurCityEngine.Waypoints;
using OurCityEngine.Hud;

namespace OurCityEngine.Mission
{
    /// <summary>
    /// Generic mission
    /// </summary>
    public class Mission : Layer, IMission
    {
        protected LinkedList<Task> tasks = new LinkedList<Task>();
        protected Task currentTask;
        protected TimeSpan remainingTime;

        public Message DoMessage { get; set; }
        public Message DoneMessage { get; set; }

        public bool Finished { get; set; }
        public bool Failed { get; set; }

        public IHud hud;

        public Mission(Game game, IHud hud) :
            base(game)
        {
            this.hud = hud;
            this.Finished = false;
            this.Failed = false;
            DoneMessage = new Message("Congrats! You just finished mission.");
        }


        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            if (currentTask != null)
            {
                currentTask.Update(gameTime);

                //write message from curent task
                hud.AddMainMessage(currentTask.UpdateText);

                if (currentTask.DoesConsumeTime())
                {
                    if (remainingTime.TotalMilliseconds < 0) {
                        hud.Notify(new Message("Time over! Mission failed.", 3, Color.Red, false));
                        this.Failed = true;
                        this.Enabled = false;
                        currentTask.Kill();
                        hud.RemoveObjective(currentTask.DoMessage);
                    } else { 
                        remainingTime -= gameTime.ElapsedGameTime;
                        hud.AddMainMessage("Time: " + String.Format("{0:0.00}", remainingTime.TotalSeconds));
                    }
                }

            }


            
        }

        /// <summary>
        /// Add time to mission timespan
        /// </summary>
        /// <param name="timeSpan"></param>
        public void AddTime(TimeSpan timeSpan) {
            remainingTime += timeSpan;
        }

        public virtual void Reset() {
            Enabled = true;
            Failed = false;
            Finished = false;
            tasks.Clear();
            currentTask = null;
        }

        #region IMission Members

        public void startNextTask()
        {
            if (tasks.Count == 0)
            {
                /// MISSION FINISHED
                hud.RemoveObjective(currentTask.DoMessage);
                hud.Notify(this.DoneMessage);
                this.Finished = true;
                this.Enabled = false;
            }
            else
            {
                /// OLD TASK cleanup
                if (currentTask != null) {
                    hud.RemoveObjective(currentTask.DoMessage);
                    
                    if (currentTask.DoneMessage != null)
                        hud.Notify(currentTask.DoneMessage);
                }

                /// NEW TASK init
                currentTask = tasks.First.Value;
                currentTask.Initialize();
                
                // notify and show task in objectives
                if (currentTask.DoMessage != null)
                    hud.Notify(currentTask.DoMessage);

                tasks.RemoveFirst();
            }
        }

        #endregion
    }
}
