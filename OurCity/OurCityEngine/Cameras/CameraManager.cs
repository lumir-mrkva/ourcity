using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Cameras
{
    /// <summary>
    /// Camera Manager
    /// </summary>
    public class CameraManager
    {

        /// <summary>
        /// Camera collection, default camera is on index 0
        /// 
        /// TODO remove default form camea collection
        /// </summary>
        public Dictionary<string, Camera> Cameras { get { return cameras; } set { } }
        private Dictionary<String, Camera> cameras = new Dictionary<string,Camera>();

        /// <summary>
        /// Default camera
        /// </summary>
        public Camera Default { get {return def;} }
        Camera def;

        /// <summary>
        /// Update all cameras in colection?
        /// </summary>
        public bool MultiCameraEnabled { get; set; }

        /// <summary>
        /// Bounding flustrum of default camera
        /// </summary>
        public BoundingFrustum Flustrum { get; set; }

        /// <summary>
        /// Set default camera
        /// </summary>
        /// <param name="p">Identifier</param>
        public void SetDefault(string p)
        {
            if (cameras.ContainsKey(p))
            {
                cameras["Default"] = cameras[p];
                def = cameras[p];
            }
        }

        /// <summary>
        /// Set / get Aspect ratio
        /// </summary>
        public float AspectRatio { 
            get {
                return Default.AspectRatio;
            }
            set {
                if (Default == null) return;
                foreach (Camera c in cameras.Values)
                {
                    c.AspectRatio = value;
                }
            }
        }

        /// <summary>
        /// Singleton
        /// </summary>
        #region Constructor
        public static CameraManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new CameraManager();

                return instance;
            }
        }
        private static CameraManager instance;
    
        private CameraManager()
        {
            cameras.Add("Default", null);
            MultiCameraEnabled = false;
        }
        #endregion

        public void Update(GameTime gameTime)
        {
            if (MultiCameraEnabled)
            {
                // update all cameras
                foreach (Camera c in cameras.Values)
                {
                    if (Default == c) continue;
                    c.Update(gameTime);
                }
            }
            else
            {
                // update only default camera
                Default.Update(gameTime);
            }

            // update bounding flustrum
            Flustrum = new BoundingFrustum(Default.View * Default.Projection);
        }
        
    }
}
