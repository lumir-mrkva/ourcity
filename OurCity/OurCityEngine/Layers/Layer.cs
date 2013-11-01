using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Cameras;
using Microsoft.Xna.Framework.Graphics;

namespace OurCityEngine.Layers {
    /// <summary>
    /// Layer of PhysicsObjects supporting Flustrum culling, Physics Disabling
    /// 
    /// </summary>
    public class Layer : DrawableGameComponent {
        #region Private fields
        List<PhysicObject> objects = new List<PhysicObject>();
        Camera camera;
        #endregion

        #region Public fields
        /// <summary>
        /// Collection of objects in layer
        /// </summary>
        public List<PhysicObject> Objects {
            get { return objects; }
            internal set { }
        }
        /// <summary>
        /// Count of culled objects
        /// </summary>
        public int CulledObjects {
            get;
            set;
        }
        /// <summary>
        /// Flustrum culling enabled
        /// </summary>
        public bool FlustrumCulling {
            get;
            set;
        }
        /// <summary>
        /// Physics disabling enabled
        /// </summary>
        public bool PhysicsDisabling { get; set; }
        /// <summary>
        /// Count of objects with disabled physics
        /// </summary>
        public int DisabledObjects { get; set; }
        /// <summary>
        /// Physics disabling sphere size
        /// </summary>
        public int SphereSize { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Default
        /// </summary>
        /// <param name="game"></param>
        public Layer(Game game)
            : base(game) {
                camera = CameraManager.Instance.Default;
                FlustrumCulling = true;
                PhysicsDisabling = true;
                SphereSize = 100;
                CulledObjects = 0;
                DisabledObjects = 0;
        }

        /// <summary>
        /// Custom constructor
        /// </summary>
        /// <param name="game"></param>
        /// <param name="flustrumCulling">Flustrum culling enabled</param>
        /// <param name="physiscsDisabling">Physics disabling enabled</param>
        /// <param name="sphereSize">Physics disabling sphere size</param>
        public Layer(Game game, bool flustrumCulling, bool physiscsDisabling, int sphereSize)
            : base(game) {
            camera = CameraManager.Instance.Default;
            FlustrumCulling = flustrumCulling;
            PhysicsDisabling = physiscsDisabling;
            SphereSize = sphereSize;
            CulledObjects = 0;
            DisabledObjects = 0;
        }
        #endregion

        /// <summary>
        /// Adds all layers objects to the this layer
        /// </summary>
        /// <param name="layer">Add objects from this layer</param>
        public void Add(Layer layer) {
            Objects.AddRange(layer.Objects);
        }

        /// <summary>
        /// Remove objects used in Layer
        /// </summary>
        /// <param name="layer">Layer</param>
        public void Remove(Layer layer) {
            foreach (PhysicObject o in layer.Objects)
            {
                Objects.Remove(o);
            }
        }

        /// <summary>
        /// Clear layer
        /// </summary>
        public void Clear() {
            foreach (PhysicObject o in Objects) {
                o.Enabled = false;
                o.PhysicsBody.DisableBody();
            }
            Objects.Clear();
        }


        #region Update
        public override void Update(GameTime gameTime) {
            
            if (FlustrumCulling) {
                // standard flustrum culling
                BoundingFrustum frustum = CameraManager.Instance.Flustrum;
                CulledObjects = 0;
                foreach (PhysicObject o in objects) {
                    if (o.PhysicsBody.CollisionSkin.WorldBoundingBox.Intersects(frustum)) {
                        o.Visible = true;
                    } else {
                        o.Visible = false;
                        CulledObjects++;
                    }
                }
            }

            if (PhysicsDisabling) {
                // physics disabled if object is far away of camera
                DisabledObjects = 0;
                BoundingSphere sphere = new BoundingSphere(CameraManager.Instance.Default.Position, SphereSize);
                foreach (PhysicObject o in objects) {
                    if (o.PhysicsBody.CollisionSkin.WorldBoundingBox.Intersects(sphere)) {
                        if (!o.PhysicsBody.IsBodyEnabled)
                            o.PhysicsBody.EnableBody();
                    } else {
                        DisabledObjects++;
                        if (o.PhysicsBody.IsBodyEnabled)
                            o.PhysicsBody.DisableBody();
                    }
                }
            }
        }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime) {
            if (!Enabled) {
                return;
            };

            // draw visible objects
            foreach (PhysicObject o in objects) {
                if (o.Visible) {
                    o.Draw(gameTime);
                }
            }


        }
        #endregion

        private class CompareByDistance : IComparer<Vector2> {
            public int Compare(Vector2 v1, Vector2 v2) {
                if (v1.X >= v2.X) {
                    return 1;
                } else {
                    return -1;
                }

            }
        }
    }
}
