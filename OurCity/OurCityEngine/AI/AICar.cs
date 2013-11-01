using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.PhysicObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Math;
using JigLibX.Utils;
using OurCityEngine.Debug;
using OurCityEngine.Utils;

namespace OurCityEngine.AI
{

    /// <summary>
    /// AI Car
    /// </summary>
    public class AICar : CarObject
    {
        // logger enable
        const bool LOG = false;

        // max speed for car
        const float MAX_SPEED = 20f;        
        // precision for pois
        const float PRECISON = 15f;
        // precision for internal pois
        const float INTERNAL_PRECISON = 15f;

        // distance from finish to allow accelerate
        const float ACCELERATE_DISTANCE = 5f;
        // distance from finish to start break
        const float BREAK_DISTANCE = 5f;
        // distance from finish for using hand break
        const float HAND_BREAK_DISTANCE = 2f;

        // stearing factor
        const float STEER_FACTOR = 0.5f;

        // car identifier
        public int Identifier { get; internal set; }
        // distance from car to destination
        public float Distance { get; internal set; }
        // destination
        public MapPOI Destination { get; internal set; }
        // old destination
        public MapPOI OldDestination { get; internal set; }
        // current line to follow
        public MapLine line { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AICar(Game game, Model model,Model wheels, bool FWDrive,
                       bool RWDrive,
                       float maxSteerAngle,
                       float steerRate,
                       float wheelSideFriction,
                       float wheelFwdFriction,
                       float wheelTravel,
                       float wheelRadius,
                       float wheelZOffset,
                       float wheelRestingFrac,
                       float wheelDampingFrac,
                       int wheelNumRays,
                       float driveTorque,
                       float gravity)
            : base(game, model, wheels, FWDrive, RWDrive, maxSteerAngle, steerRate, wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius, wheelZOffset, wheelRestingFrac, wheelDampingFrac, wheelNumRays, driveTorque, gravity)
        {
        }


        /// <summary>
        /// Inicialize first route (line), setting start and finish pois
        /// </summary>
        public void InitRoute() {
            this.OldDestination = this.line.from;
            this.Destination = this.line.to;
        }

        /// <summary>
        /// Update car's destination
        /// </summary>
        private void UpdateDestination() {
            this.OldDestination = this.Destination;
            this.Destination = this.GetNextPOI(this.OldDestination);
            if (LOG)
                Logger.Instance.Log("car (" + this.Identifier + ") going from " + this.OldDestination.location.ToString() + "  to destination " + this.Destination.location.ToString());
        }


        /// <summary>
        /// Return next POI to follow
        /// </summary>
        /// <param name="prev">Previous POI</param>
        /// <returns>Next POI</returns>
        private MapPOI GetNextPOI(MapPOI prev) {
            MapPOI next = this.line.GetNextPOI(prev);            
            if (next == null) {
                if (LOG)
                    Logger.Instance.Log("next poi is null, finding new line");
                if (prev.lines.Count > 0)
                {
                    Random rand = new Random();
                    int randomNumber = rand.Next(0, this.OldDestination.lines.Count);
                    this.line = prev.lines[0];
                    next = this.line.GetNextPOI(prev);
                    if (LOG)
                        Logger.Instance.Log("next poi: " + next.location.ToString());
                }
                else 
                {
                    if (LOG)
                        Logger.Instance.Log("prev poi's line is empty!");
                }
            }
            return next;
        }

        /// <summary>
        /// Correction car's speed
        /// </summary>
        private void SpeedCorrection () {
            float MaxSpeed = MAX_SPEED;
            float ActualSpeed = (float)Math.Round(this.PhysicsBody.Velocity.Length());

            this.Car.HBrake = 0f;

            // accelerate if we can
            if (this.Distance > ACCELERATE_DISTANCE && ActualSpeed < MaxSpeed)
            {
                this.Car.Accelerate = 0.20f;
            }
            // break if neccesary
            else if (this.Distance <= BREAK_DISTANCE && this.Distance > HAND_BREAK_DISTANCE && ActualSpeed > 0)
            {
                this.Car.Accelerate = -1.0f;
            }
            else {
                this.Car.HBrake = 1f;
            }
        }

        /// <summary>
        /// Correction car's steering
        /// </summary>
        private void SteeringCorrection() { 

            Vector3 orientation = this.Car.Chassis.Body.Orientation.Forward;
            Vector3 toDestination = this.Destination.location - this.Car.Chassis.Body.Position;
            Vector3 path = this.Destination.location - this.OldDestination.location;

            orientation.Normalize();
            toDestination.Normalize();
            path.Normalize();

            double AngleFromDestination = Math.Round((float)Math.Acos(Vector3.Dot(orientation, toDestination)), 1);
            double AngleFromPath = Math.Round((float)Math.Acos(Vector3.Dot(orientation, path)), 1);            

            if (AngleFromDestination > 1.6f )
            {
                this.Car.Accelerate = -1.0f;
                this.Car.Steer = -STEER_FACTOR;
            }
            else if (AngleFromDestination < 1.5f )
            {
                this.Car.Accelerate = -1.0f;
                this.Car.Steer =  STEER_FACTOR;
            }
            else if (AngleFromPath > 1.6f) {
                this.Car.Steer = -STEER_FACTOR/2;
            }
            else if (AngleFromPath < 1.5f)
            {
                this.Car.Steer = STEER_FACTOR/2;
            }
            else
            {
                this.Car.Steer = 0f;
            }
            
        }

        /// <summary>
        /// Update car's move - speed, direction and destination
        /// </summary>
        public void UpdateMove(GameTime gameTime)
        {
            if (this.Destination.lineInternal)
            {
                if (Vector3.Distance(this.Destination.location, this.Car.Chassis.Body.Position) < INTERNAL_PRECISON)
                {
                    this.UpdateDestination();
                }
            }
            else
            {
                if (Vector3.Distance(this.Destination.location, this.Car.Chassis.Body.Position) < PRECISON)
                {
                    this.UpdateDestination();
                }
            }

            if (this.Destination == null) {
                // TODO: what if Destination == null
            } else {
                this.Distance = Vector3.Distance(this.Car.Chassis.Body.Position, this.Destination.location);
                this.SteeringCorrection();
            }

            
            this.SpeedCorrection();      
        }

        /// <summary>
        /// Draw car
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            this.UpdateMove(gameTime);
            base.Draw(gameTime);
        }        
    }
}
