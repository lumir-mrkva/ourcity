using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using System.Diagnostics;
using OurCityEngine.Screens;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Cameras;
using OurCityEngine.Utils;
using OurCityEngine.AI;
using OurCityEngine.Layers;

namespace OurCityEngine.AI
{
    public class TrafficManager : IGameComponent
    {
        // max cars on the map
        const int MAX_CARS = 1;
        // logger enable
        const bool LOG = true;

        // car counter
        int carCounter;
        // car and wheel models
        Model camaroModel, wheelModel;

        // map
        MapObject map;
        // game
        Game game;
        // content
        ContentManager content;
        // layer for cars
        Layer cars;
        // gravity constant
        private float gravity;

        // list of waypoints
        public List<Vector3> Waypoints { get { return map.Waypoints; } }
        // list of cars
        public List<int> Cars = new List<int>();
        // layer for cars
        public Layer CarsLayer { get { return cars; } set { } }

        // drawable components (cars to draw)
        List<DrawableGameComponent> components;
        public List<DrawableGameComponent> Components { get { return components; } }

        /// <summary>
        /// AI Traffic manager
        /// </summary>
        public TrafficManager(Game game, ContentManager content, float gravity)
        {
            this.game = game;
            this.content = content;
            this.gravity = gravity;
            this.Initialize();
        }

        /// <summary>
        /// Initialize traffic manager
        /// </summary>
        public void Initialize() {
            this.carCounter = 0;
            this.map = new MapObject();
            components = new List<DrawableGameComponent>();
            camaroModel = content.Load<Model>("Cars/camaro_low");
            wheelModel = content.Load<Model>("Cars/wheel");

            /// cars layer
            cars = new Layer(game);
            cars.SphereSize = 300;
            this.components.Add(cars);
            
            
            this.Populate(MAX_CARS);
        }

        /// <summary>
        /// Populate map with cars
        /// <param name="carsCount">Number of cars to populate</param>
        /// </summary>
        public void Populate(int carsCount = MAX_CARS) {
            for (int i = 0; i < MAX_CARS || i < carsCount; i++) {
                this.Cars.Add(this.AddCar());
            }
        }

        /// <summary>
        /// Adding new car to map
        /// <returns>Car identifier</returns>
        /// </summary>
        public int AddCar() {

            //TODO: retrive value from file
            AICar newCar = new AICar(game,
                camaroModel,    // model chase
                wheelModel,     // wheel model
                true,   // FW drive
                true,   // RW drive
                30.0f,  // max steer range
                5.0f,   // steer rate
                4.7f,   // wheel side friction
                5.0f,   // wheel forward friction 
                0.20f,  // wheel travel - svetla vyska
                0.4f,   // wheel radius - rozvor naprav
                0.05f,  // wheel z offset
                0.45f,  // wheel resting frac
                0.6f,   // wheel damping frac
                4,      // wheel num rays
                250.0f, // drive torque - vykon motoru
                gravity
                );
            newCar.line = this.map.GetNextStartLine();
            newCar.InitRoute();
            newCar.Car.Chassis.Body.MoveTo(newCar.line.from.location, Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
            //newCar.SetColor();
            newCar.Identifier = this.carCounter++;            
            if (LOG)
                Logger.Instance.Log("new ai car (" + newCar.Identifier + ") position: " + newCar.line.from.location.ToString());            
            newCar.Car.EnableCar();
            newCar.Car.Chassis.Body.AllowFreezing = false;
            //newCar.Visible = false;
            //this.components.Add(newCar);

            cars.Objects.Add(newCar);

            return newCar.Identifier;
        }
    }
}
