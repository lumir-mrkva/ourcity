using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JiggleGame.PhysicObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Math;
using JigLibX.Utils;

namespace JigLibTest.GameObjects
{
    class AICar : CarObject
    {
        const float MAX_SPEED_FW = 0.5f;
        const float MAX_SPEED_BW = 0.2f;
        const float MAX_SPEED_TURN = 0.2f;
        const float PRECISON = 10;
        const float ACCELERATE_DISTANCE = 2 * PRECISON;
        const float BREAK_DISTANCE = PRECISON;
        const float STEER_FACTOR = 1.0f;
        const int STAY = 0;
        const int UP = 1;
        const int DOWN = -1;
        const int LEFT = 2;
        const int RIGHT = 3;

        MapPOI Map;

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
            this.Map = new MapPOI();
            this.Destination = this.Car.Chassis.Body.Position;
            this.Move = STAY;
        }

        private Vector3 GetMyPosition() {
            return this.Car.Chassis.Body.Position;
        }

        public void SetDestination(Vector3 Destination) {
            this.Destination = Destination;
        }

        private void CalculateNewDestination() {
            this.Destination = this.Map.GetNextCrossRoad(this.GetMyPosition());            
        }

        private void UpdateOrientation() {
            if (this.Car.Chassis.Body.Orientation.Forward.Z.CompareTo(-1.0f) == 1)
            {
                this.ApproximateOrientation = UP;
            }
            else if (this.Car.Chassis.Body.Orientation.Backward.Z.CompareTo(-1.0f) == 1)
            {
                this.ApproximateOrientation = DOWN;
            }
            else if (this.Car.Chassis.Body.Orientation.Right.Z.CompareTo(-1.0f) == 1)
            {
                this.ApproximateOrientation = RIGHT;
            }
            else if (this.Car.Chassis.Body.Orientation.Left.Z.CompareTo(-1.0f) == 1)
            {
                this.ApproximateOrientation = LEFT;
            }
            this.ApproximateOrientation = STAY;            
        }

        private void SpeedCorrection () {
            float MaxSpeed = MAX_SPEED_FW;

            switch (this.Move)
            {
                case UP:
                    MaxSpeed = MAX_SPEED_FW;
                    break;
                case DOWN:
                    MaxSpeed = MAX_SPEED_BW;
                    break;
                case LEFT:
                case RIGHT:
                    MaxSpeed = MAX_SPEED_TURN;
                    break;
            }

            if (this.Car.Accelerate > MaxSpeed || this.Distance < ACCELERATE_DISTANCE && Distance > BREAK_DISTANCE)
            {
                this.Car.Accelerate = MathHelper.SmoothStep(0, this.Car.Accelerate, BREAK_DISTANCE);
            }
            else if (this.Distance > ACCELERATE_DISTANCE)
            {
                this.Car.Accelerate = MathHelper.Clamp(this.Car.Accelerate + 0.1f, 0, MaxSpeed);
            }
            else
            {
                this.Car.HBrake = 1.0f;
            }
        }

        private void SteeringCorrection() { 
            Vector3 MyPosition = this.GetMyPosition();
            if ((this.ApproximateOrientation == UP 
                && ((MyPosition.Z < this.Destination.Z + PRECISON) || (MyPosition.Z < this.Destination.Z - PRECISON)))
                || (this.ApproximateOrientation == RIGHT
                && ((MyPosition.Z < this.Destination.Z + PRECISON) || (MyPosition.Z < this.Destination.Z - PRECISON))))
            {
                this.Car.Steer = -STEER_FACTOR;
            }
            else if ((this.ApproximateOrientation == UP 
                && ((MyPosition.Z > this.Destination.Z + PRECISON) || (MyPosition.Z > this.Destination.Z - PRECISON)))
                || (this.ApproximateOrientation == LEFT
                && ((MyPosition.Z > this.Destination.Z + PRECISON) || (MyPosition.Z > this.Destination.Z - PRECISON))))
            {                
                this.Car.Steer = STEER_FACTOR;
            }
            else 
            {
                this.Car.Steer = 0.0f;
            }
        }

        public int ApproximateOrientation { get; internal set; }

        public int Move { get; internal set; }

        public float Distance { get; internal set; }

        public Vector3 Destination { get; internal set; }       
        
        public void UpdateMove(GameTime gameTime)
        {
            if (this.Destination == this.GetMyPosition()) {
                this.CalculateNewDestination();
            }

            this.Distance = Vector3.Distance(this.GetMyPosition(), this.Destination);

            this.UpdateOrientation();
            this.SteeringCorrection();
            this.SpeedCorrection();      
        }

        public override void Draw(GameTime gameTime)
        {
            this.UpdateMove(gameTime);

            base.Draw(gameTime);
        }
    }
}
