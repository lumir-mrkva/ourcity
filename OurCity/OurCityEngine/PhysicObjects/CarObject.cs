using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Vehicles;
using JigLibX.Collision;
using OurCityEngine.Cameras;
using OurCityEngine.Waypoints;
using OurCityEngine.Utils;

namespace OurCityEngine.PhysicObjects
{
    /// <summary>
    /// Car object
    /// </summary>
    public class CarObject : PhysicObject
    {

        private Car car;
        private Model wheel;

        /// <summary>
        /// Constructor of CarObject
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="model">Car model</param>
        /// <param name="wheels">Wheel model</param>
        /// <param name="FWDrive">Is front axel powered</param>
        /// <param name="RWDrive">Rear axel power</param>
        /// <param name="maxSteerAngle">Max steer angle</param>
        /// <param name="steerRate">Steer rate</param>
        /// <param name="wheelSideFriction">Wheel side friction</param>
        /// <param name="wheelFwdFriction">Wheel forward froction</param>
        /// <param name="wheelTravel">Curb height</param>
        /// <param name="wheelRadius"></param>
        /// <param name="wheelZOffset"></param>
        /// <param name="wheelRestingFrac"></param>
        /// <param name="wheelDampingFrac"></param>
        /// <param name="wheelNumRays"></param>
        /// <param name="driveTorque">Engine power</param>
        /// <param name="gravity">Gravity (for wheel dampers)</param>
        public CarObject(Game game, Model model, Model wheels, bool FWDrive,
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
            : base(game, model)
        {
            car = new Car(FWDrive, RWDrive, maxSteerAngle, steerRate,
                wheelSideFriction, wheelFwdFriction, wheelTravel, wheelRadius,
                wheelZOffset, wheelRestingFrac, wheelDampingFrac,
                wheelNumRays, driveTorque, gravity);

            this.body = car.Chassis.Body;
            this.collision = car.Chassis.Skin;
            this.wheel = wheels;

            SetCarMass(100.0f);
            color = RandomHelper.Instance.StandardColor.ToVector3();
        }

        /// <summary>
        /// Constructor with minimal params
        /// </summary>
        /// <param name="game"></param>
        /// <param name="model"></param>
        /// <param name="wheels"></param>
        /// <param name="gravity"></param>
        public CarObject (Game game, Model model, Model wheels, float gravity) : 
            this(game, model, wheels, true, true, 30f, 5f, 4.7f, 5f, 0.2f, 0.4f, 0.05f, 0.45f, 0.6f, 4, 250f, gravity) {

        }

        /// <summary>
        /// Draw car wheels according to steering
        /// </summary>
        /// <param name="wh">Wheel</param>
        /// <param name="rotated">Rotate this wheel</param>
        private void DrawWheel(Wheel wh, bool rotated)
        {
            Camera camera = CameraManager.Instance.Default;

            foreach (ModelMesh mesh in wheel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    float steer = wh.SteerAngle;

                    Matrix rot;
                    if (rotated) rot = Matrix.CreateRotationY(MathHelper.ToRadians(180.0f));
                    else rot = Matrix.Identity;

                    effect.World = rot * Matrix.CreateRotationZ(MathHelper.ToRadians(-wh.AxisAngle)) * // rotate the wheels
                        Matrix.CreateRotationY(MathHelper.ToRadians(steer)) *
                        Matrix.CreateTranslation(wh.Pos + wh.Displacement * wh.LocalAxisUp) * car.Chassis.Body.Orientation * // oritentation of wheels
                        Matrix.CreateTranslation(car.Chassis.Body.Position); // translation

                    effect.View = camera.View;
                    effect.Projection = camera.Projection;
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;
                }
                mesh.Draw();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            DrawWheel(car.Wheels[0], true);
            DrawWheel(car.Wheels[1], true);
            DrawWheel(car.Wheels[2], false);
            DrawWheel(car.Wheels[3], false);

            DrawColor();

            base.Draw(gameTime);
        }

        public Car Car
        {
            get { return this.car; }
        }

        private void SetCarMass(float mass)
        {
            body.Mass = mass;
            Vector3 min, max;
            car.Chassis.GetDims(out min, out max);
            Vector3 sides = max - min;

            float Ixx = (1.0f / 12.0f) * mass * (sides.Y * sides.Y + sides.Z * sides.Z);
            float Iyy = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Z * sides.Z);
            float Izz = (1.0f / 12.0f) * mass * (sides.X * sides.X + sides.Y * sides.Y);

            Matrix inertia = Matrix.Identity;
            inertia.M11 = Ixx; inertia.M22 = Iyy; inertia.M33 = Izz;
            car.Chassis.Body.BodyInertia = inertia;
            car.SetupDefaultWheels();
        }

        

        /// <summary>
        /// Paints Car
        /// </summary>
        /// <param name="color">Color</param>
        public void SetColor(Color color) {
            this.color = color.ToVector3();
        }

        private void DrawColor() {
            foreach (ModelMesh mesh in model.Meshes) {
                if (mesh.Name.Contains("Car_Body") || mesh.Name.Contains("Door") || mesh.Name.Contains("Mirror")) {
                    foreach (BasicEffect eff in mesh.Effects) {
                        eff.DiffuseColor = color;
                    }
                }
            }
        }

        /// <summary>
        /// Move car to position
        /// </summary>
        /// <param name="position">Position</param>
        public void Move(Vector3 position) {
            Move(position, Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
        }

        public void Move(Vector3 position, Matrix orientation) {
            this.Car.Chassis.Body.MoveTo(position, orientation);
        }

        public override void ApplyEffects(BasicEffect effect)
        {
            // 
        }

        
    }
}
