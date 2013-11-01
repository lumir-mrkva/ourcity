using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.PhysicObjects;
using JigLibX.Collision;
using OurCityEngine.Utils;
using JigLibX.Vehicles;

namespace OurCityEngine.Waypoints
{
    public delegate void WaypointCollisionHandler(Waypoint waypoint, CarObject car);

    /// <summary>
    /// Waypoint manager is watching for collisions
    /// </summary>
    public class WaypointManager
    {
        List<CarObject> cars = new List<CarObject>();
        List<Waypoint> waypoints = new List<Waypoint>();
        
        public WaypointManager() { }

        public event WaypointCollisionHandler WaypointCollision;

        private void OnWaypointCollision(Waypoint waypoint, CarObject car)
        {
            //Logger.Instance.Log("Checkpoint hit!");
            if (WaypointCollision != null)
                WaypointCollision(waypoint, car);
        }

        public void AddWatchedCar(CarObject car) 
        {
            //car.PhysicsSkin.callbackFn += this.WaypointCollisionHandler;
            cars.Add(car); 
        }
        public void RemovaWatchedCar(CarObject car) { cars.Remove(car); }

        public void AddWatchedWaypoint(Waypoint w) 
        {
            w.PhysicsSkin.callbackFn += this.WaypointCollisionHandler;
            waypoints.Add(w); 
        }
        public void RemoveWatchedWaypoint(Waypoint w) { waypoints.Remove(w); }

        public void ClearWatchedCars() { cars.Clear(); }
        public void ClearWatchedWaypoints() { waypoints.Clear(); }

        public void ClearAll()
        {
            this.ClearWatchedCars();
            this.ClearWatchedWaypoints();
        }

        public List<Waypoint> GetAllWaypoints()
        {
            return waypoints;
        }

        /// <summary>
        /// We register this callback to our collision system so we can assure, that physics won't apply for waypoints.
        /// They become "Ghost"-like.
        /// Also when watched car runs through (collides) waypoint, we fire up an event.
        /// </summary>
        /// <param name="waypointSkin"></param>
        /// <param name="colidee"></param>
        /// <returns></returns>
        public bool WaypointCollisionHandler(CollisionSkin waypointSkin, CollisionSkin colidee)
        {
            CarObject car = null;
            Waypoint waypoint = null;

            foreach (CarObject c in cars)
            {

                if (colidee.Equals(c.Car.Chassis.Skin))
                {
                    car = c;
                    break;
                }
            }

            if (car == null) return false; // No wp to collide with -> just apply physics

            // Find correct wp to return


            foreach (Waypoint w in waypoints)
            {
                if (waypointSkin.Equals(w.PhysicsSkin))
                {
                    waypoint = w;
                }

            }

            if (waypoint == null)
            {
                Logger.Instance.Log("Coliding car isn't managed!");
                //applyPhysics = false; // Redundant
            }

            OnWaypointCollision(waypoint, car); // Notify all listeners, that we have a hit


            return false; // Never mess with waypoints
        }

    }
}
