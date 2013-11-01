using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;

namespace OurCity.Scenes {
    class CarsScene : Scene {

        public CarsScene(Game game, Model carModel, Model wheelModel, float gravity) : base(game) {
            CarObject car = new CarObject(game, carModel, wheelModel, gravity);
            car.Move(new Vector3(-161f, 0, 493.5f), Matrix.CreateFromYawPitchRoll(+1.57f, 0, 0));
            car.Car.EnableCar();
            Objects.Add(car);
            CarObject car2 = new CarObject(game, carModel, wheelModel, gravity);
            car2.Move(new Vector3(-161f, 0, 500f));
            car2.Car.EnableCar();
            Objects.Add(car2);
        }

        public void Initialize() {
            
        }

    }
}
