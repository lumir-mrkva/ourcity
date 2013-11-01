#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using JigLibX.Physics;
using JigLibX.Collision;
using JigLibX.Geometry;
using JigLibX.Math;
using JigLibX.Utils;
using JigLibX.Vehicles;
using System.Diagnostics;
using OurCityEngine.Screens;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Cameras;
using OurCityEngine.Debug;
using OurCityEngine.Utils;
using OurCityEngine.AI;
using OurCityEngine.Waypoints;
using OurCity.Effects;
using OurCityEngine.Layers;
using OurCityEngine.Hud;
using OurCity.Layers;
using OurCity.Missions;
using OurCityEngine.Mission;
using OurCity.Scenes;
#endregion

namespace OurCity.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// 
    /// </summary>
    public class GameplayScreen : GameScreen, IGameComponent
    {
        #region Fields
        protected PhysicsSystem physicSystem;

        Model islandModel, camaroModel, wheelModel;

        TriangleMeshObject island;
        CarObject carObject;
        Tuple<Vector3, Matrix> carResetLocation;

        ChaseCamera chaseCamera;
        Layer buildings, sceneLayer;

        ContentManager content;

        HudOverlay hud;

        Debug debug;

        List<DrawableGameComponent> components;

        List<Mission> missions = new List<Mission>();
        Mission activeMission;

        TrafficManager trafficManager;

        Water water;
        SkyDome sky;

        public Camera Camera { get { return CameraManager.Instance.Cameras["Default"]; } }
        public List<DrawableGameComponent> Components { get { return components; } }
        OurCityEngine.Debug.DebugDrawer DebugDrawer { get; set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            components = new List<DrawableGameComponent>();
        }

        public void Initialize()
        {
            physicSystem = new PhysicsSystem();
            physicSystem.CollisionSystem = new CollisionSystemSAP();

            #region Camera
            //camera
            chaseCamera = new ChaseCamera(ScreenManager.Game);
            if (!CameraManager.Instance.Cameras.ContainsKey("Chase"))
                CameraManager.Instance.Cameras.Add("Chase", chaseCamera);
            else
                CameraManager.Instance.Cameras["Chase"] = chaseCamera;

            CameraManager.Instance.SetDefault("Chase");

            // Set the camera offsets
            chaseCamera.DesiredPositionOffset = new Vector3(0.0f, 5f, 11f);
            chaseCamera.LookAtOffset = new Vector3(0.0f, 3f, 0.0f);

            // Set camera perspective
            chaseCamera.NearPlaneDistance = 1.0f;
            chaseCamera.FarPlaneDistance = 10000.0f;

            chaseCamera.DontFollow = new Vector3(1000, -17, 1000);

            // set default looking direction
            LookInDirection = Vector3.Forward;
            #endregion

            // sky dome
            sky = new SkyDome(ScreenManager.Game);       
            sky.Initialize();
            sky.Theta = 1.828064f;
            Components.Add(sky);
            
            // water
            water = new Water(ScreenManager.Game, "Map/Textury/SkyBox/skybox02", new Vector3(-1310,-25,-1900),new Vector3(20,5,31));
            water.Initialize();
            water.BumpSpeed = new Vector2(0, .01f);
            water.BumpHeight = 0.4f;
            water.WaveFrequency = 0.5f;
            water.ReflectionAmount = 0.4f;
            water.WaveAmplitude = 0.3f;
            Components.Add(water);

            // debug
            debug = ((OurCityGame)ScreenManager.Game).Debug;
            debug.Enabled = false;

            DebugDrawer = DebugManager.Instance.Drawer;

            // player
            PlayerManager.Instance.Initialize(this);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {

            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            Initialize();

            islandModel = content.Load<Model>("Map/ourCity");
            camaroModel = content.Load<Model>("Cars/camaro_n");
            wheelModel = content.Load<Model>("Cars/wheel");

            island = new TriangleMeshObject(ScreenManager.Game, islandModel, Matrix.Identity, Vector3.Zero);
            //island.ApplyAdvancedEffects(terrainEffect);
            this.Components.Add(island);

            // buildings
            buildings = new Buildings(ScreenManager.Game, island);
            buildings.Initialize();
            buildings.FlustrumCulling = true;
            Components.Add(buildings);


            // traffic manager
            trafficManager = new TrafficManager(ScreenManager.Game, content, physicSystem.Gravity.Length());
            AICarsInMinimap = false;

            // show traffic manager waypoints for debuging purposes
            debug.ShowPositions(trafficManager.Waypoints);

            foreach (DrawableGameComponent c in trafficManager.Components)
                this.Components.Add(c);

            // car
            carObject = new CarObject(ScreenManager.Game,
                camaroModel,    // model chase
                wheelModel,     // wheel model
                false,   // FW drive
                true,   // RW drive
                25.0f,  // max steer range
                6.0f,   // steer rate
                4.7f,   // wheel side friction
                5.0f,   // wheel forward friction 
                0.20f,  // wheel travel - svetla vyska
                0.4f,   // wheel radius - rozvor naprav
                0.05f,  // wheel z offset
                0.45f,  // wheel resting frac
                0.8f,   // wheel damping frac
                4,      // wheel num rays
                250.0f, // drive torque - vykon motoru
                physicSystem.Gravity.Length()
                );

            carResetLocation = new Tuple<Vector3, Matrix>(new Vector3(0, 2, 40), Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
            //carObject.Car.Chassis.Body.MoveTo(new Vector3(0, 2, 40), Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
            ResetCar();
            carObject.Car.EnableCar();
            carObject.SetColor(new Color(0.698f,0,0));
            carObject.Car.Chassis.Body.AllowFreezing = false;

            this.Components.Add(carObject);

            //HUD
            hud = new HudOverlay(ScreenManager);
            hud.Initialize(); // je treba zavolat jinak neprobehne LoadContent();

            // Scenes
            sceneLayer = new Layer(ScreenManager.Game);
            sceneLayer.Initialize();
            Components.Add(sceneLayer);

            // Missions
            missions.Add(new Mission1(ScreenManager.Game, carObject, hud));
            missions.Add(new Mission4(ScreenManager.Game, carObject, hud));
            missions.Add(new Mission2(ScreenManager.Game, carObject, hud));
            missions.Add(new Mission3(ScreenManager.Game, carObject, hud));
                        
            missions[0].Initialize();
            missions[0].startNextTask();
            activeMission = missions[0];
            Components.Add(activeMission);

            base.LoadContent();
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            debug.Enabled = false;

            content.Unload();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            #region Update to debug
            if (debug.Enabled)
            {
                UpdateDebug();
            }
            #endregion

            #region Update camera
            // Update the camera to chase the new target
            UpdateCameraChaseTarget();
            CameraManager.Instance.Update(gameTime);
            #endregion

            if (IsActive) {
                #region Update to hud
                if (hud != null && hud.Enabled) {
                    UpdateHud(gameTime);
                }
                #endregion

                #region Update missions (simply start one mission after another)
                if (activeMission.Failed) {
                    activeMission.Reset();
                    hud.ClearObjectives(); // zichr TODO: opravit
                }
                if (activeMission.Finished) {
                    hud.ClearObjectives(); // zichr TODO: opravit
                    activeMission.Enabled = false;
                    int i = missions.IndexOf(activeMission) + 1;
                    if (missions.Count > i) {
                        missions[i].Initialize();
                        missions[i].startNextTask();
                        activeMission = missions[i];
                        Components.Add(activeMission);
                    } else if (missions.Count +1 > i) {
                        hud.Notify(new Message("Thats all we have for you today."));
                        hud.Notify(new Message("For updates visit http://mrq.cz/ourcity",5,Color.White,false));
                        hud.Notify(new Message("btw. developer console is x key", 2,Color.White,false));
                        activeMission = missions[0];
                        activeMission.Finished = false;
                        activeMission.Enabled = false;
                    } 
                }
                #endregion

                CheckDeath();
                physicSystem.Integrate((float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
            }

            List<DrawableGameComponent> toUpdate = new List<DrawableGameComponent>();
            foreach (DrawableGameComponent c in Components)
                if (c.Enabled)
                    toUpdate.Add(c);

            foreach (DrawableGameComponent c in toUpdate)
                c.Update(gameTime);
        }

        #region Check death
        private void CheckDeath()
        {
            if (carObject.PhysicsBody.Position.Y < -40)
            {
                //chaseCamera.ChasePosition = chaseCamera.ChasePosition + new Vector3(0, 10, 0);
                hud.Enabled = false;
                ScreenManager.AddScreen(new DeadScreen(), ControllingPlayer);
            }
        }
        #endregion

        #region Update HUD and debug
        /// <summary>
        /// Update to debug
        /// </summary>
        private void UpdateDebug() {
            debug.CarPosition = carObject.PhysicsBody.Position;
            debug.CarFWOrientation = carObject.PhysicsBody.Orientation.Forward;
            debug.CarVelocity = carObject.PhysicsBody.Velocity;
            debug.SkyTime = sky.Theta;
            debug.Culling = "Buildings cull: "+buildings.DisabledObjects + " / " + buildings.CulledObjects + " / " + buildings.Objects.Count;
            debug.Culling+="\nAIcars culling:"+this.trafficManager.CarsLayer.DisabledObjects + " / " + this.trafficManager.CarsLayer.CulledObjects + " / " + this.trafficManager.CarsLayer.Objects.Count;
            debug.Physics = " Physics \n islands: " + physicSystem.islands.Count + "\n colisions: " + physicSystem.Collisions.Count
                + "\n bodies: " + physicSystem.Bodies.Count
                + "\n skins: " + physicSystem.CollisionSystem.CollisionSkins.Count
                + "\n time: " + Math.Round(physicSystem.TargetTime - physicSystem.OldTime,3);
            debug.Resolution = ScreenManager.GetResolution();
        }

        private void UpdateHud(GameTime gameTime)
        {
            // assuming velocity is in m/s, converting to km/h
            hud.CarVelocity = (float)Math.Round(carObject.PhysicsBody.Velocity.Length() * 3.6f);

            // show AI cars on minimap
            if (AICarsInMinimap) {
                foreach (AICar car in this.trafficManager.CarsLayer.Objects)
                {
                    hud.AddMinimapPoi(car.PhysicsBody.Position, Color.Orange, PoiShape.Square);
                }
            }
            
            // send player location (for minimap)
            hud.PlayerLocation = carObject.PhysicsBody.Position;
            hud.PlayerRotation = Util.Vector3ToRadians(-carObject.PhysicsBody.Orientation.Forward);

            hud.Update(gameTime);
        }
        public bool AICarsInMinimap {get;set;}
        #endregion

        #region Update Chase camera
        /// <summary>
        /// Update the values to be chased by the camera
        /// </summary>
        /// 
        private Vector3 LookInDirection { get; set; }

        private void UpdateCameraChaseTarget()
        {
            chaseCamera.ChasePosition = carObject.Car.Chassis.Body.Position;

            //chaseCamera.Up = carObject.Car.Chassis.Body.Orientation.Up;
            chaseCamera.Up = Vector3.Lerp(Vector3.Up, carObject.Car.Chassis.Body.Orientation.Up, 0.2f);


            chaseCamera.SpringEnabled = true;

            // where to look
            if (LookInDirection == Vector3.Forward)
            {
                chaseCamera.ChaseDirection = carObject.Car.Chassis.Body.Orientation.Right;
                return;
            }
            if (LookInDirection == Vector3.Backward)
            {
                // no spring in look-back
                chaseCamera.SpringEnabled = false;
                chaseCamera.ChaseDirection = carObject.Car.Chassis.Body.Orientation.Left;
                return;
            }
            if (LookInDirection == Vector3.Right)
            {
                chaseCamera.ChaseDirection = carObject.Car.Chassis.Body.Orientation.Backward;
                return;
            }
            if (LookInDirection == Vector3.Left)
            {
                chaseCamera.ChaseDirection = carObject.Car.Chassis.Body.Orientation.Forward;
                return;
            }

        }
        #endregion

        #endregion

        #region Scene management
        /// <summary>
        /// Load scene number X
        /// </summary>
        /// <param name="scene">X</param>
        public void LoadScene(int scene) {
            sceneLayer.Clear();
            switch (scene) {
                case 1: {
                    sceneLayer.Add(new CarsScene(ScreenManager.Game, camaroModel, wheelModel, physicSystem.Gravity.Length()));
                    break;
                    }
                default: return;
            }

        }
        #endregion

        #region Handle input
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            #region Handle pause
            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                return;
            }
            #endregion

            #region Car Control
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.Down))
            {
                if (keyState.IsKeyDown(Keys.Up))
                    carObject.Car.Accelerate = 1.0f;
                else
                    carObject.Car.Accelerate = -1.0f;
            }
            else
                carObject.Car.Accelerate = 0.0f;

            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.Right))
            {
                if (keyState.IsKeyDown(Keys.Left))
                    carObject.Car.Steer = 1.0f;
                else
                    carObject.Car.Steer = -1.0f;
            }
            else
                carObject.Car.Steer = 0.0f;

            if (keyState.IsKeyDown(Keys.B))
                carObject.Car.HBrake = 1.0f;
            else
                carObject.Car.HBrake = 0.0f;

            #region Gamepad
            if (gamePadState.IsConnected) {
                carObject.Car.Accelerate = gamePadState.Triggers.Right - gamePadState.Triggers.Left;
                carObject.Car.Steer = gamePadState.ThumbSticks.Left.X * (-1);

                if (gamePadState.IsButtonDown(Buttons.A))
                    carObject.Car.HBrake = 1.0f;
                else
                    carObject.Car.HBrake = 0.0f;
            }
            #endregion

            // speedlimiter
            if (carObject.Car.Chassis.Body.Velocity.Length() > 56 && carObject.Car.Accelerate > 0) {
                carObject.Car.Accelerate = 0f;
            }
            #endregion

            #region Debug toggle
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.X) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.X))
            {
                debug.Enabled = !debug.Enabled;
            }
            #endregion

            #region Handle C key (Debug drawer)
            if (debug.Enabled)
            {

                if (!DebugDrawer.Enabled && keyState.IsKeyDown(Keys.C)) DebugDrawer.Enabled = true;
                if (DebugDrawer.Enabled && keyState.IsKeyUp(Keys.C)) DebugDrawer.Enabled = false;

            }
            #endregion

            #region Camera modes
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F5) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F5))
            {
                CameraManager.Instance.SetDefault("Chase");
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F6) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F6))
            {
                if (CameraManager.Instance.Cameras.ContainsKey("Free"))
                    CameraManager.Instance.SetDefault("Free");
                else
                {
                    CameraManager.Instance.Cameras.Add("Free", new Camera(ScreenManager.Game));
                    CameraManager.Instance.SetDefault("Free");
                }
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F7) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F7))
            {
                if (CameraManager.Instance.Cameras.ContainsKey("Free2"))
                    CameraManager.Instance.SetDefault("Free2");
                else
                {
                    CameraManager.Instance.Cameras.Add("Free2", new Camera(ScreenManager.Game));
                    CameraManager.Instance.SetDefault("Free2");
                }
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F8) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F8))
            {
                if (CameraManager.Instance.Cameras.ContainsKey("Free3"))
                    CameraManager.Instance.SetDefault("Free3");
                else
                {
                    CameraManager.Instance.Cameras.Add("Free3", new Camera(ScreenManager.Game));
                    CameraManager.Instance.SetDefault("Free3");
                }
            }
            #endregion

            #region Reset car
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.R) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.R))
            {
                ResetCar();
            }

            #endregion

            #region Log car position
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.L) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.L))
            {
                Vector3 pos = carObject.PhysicsBody.Position;
                Logger.Instance.Log("car position: Vector3(" + pos.X + "f, " + pos.Y + "f, " + pos.Z + "f)");

                //add debug sphere to logged location
                debug.SpawnDebugObject(pos);
            }
            #endregion

            #region Toggle hud
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.J) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.J))
            {
                hud.Enabled = !hud.Enabled;
            }
            #endregion

            #region Look Back ...
            LookInDirection = Vector3.Forward;
            // back
            if (keyState.IsKeyDown(Keys.NumPad2) || keyState.IsKeyDown(Keys.N)) LookInDirection = Vector3.Backward;
            if (keyState.IsKeyDown(Keys.NumPad4)) LookInDirection = Vector3.Left;
            if (keyState.IsKeyDown(Keys.NumPad6)) LookInDirection = Vector3.Right;

            #endregion

            #region Customise water 
            /*
            if (debug.Enabled)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                    water.WaterAmount = 0;

                if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    water.WaterAmount = 1;
                    water.ShallowWaterColor = Color.DarkSeaGreen;
                    water.DeepWaterColor = Color.Navy;
                    water.ReflectionColor = Color.DarkGray;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    water.WaterAmount = 1;
                    water.ShallowWaterColor = Color.Gold;
                    water.DeepWaterColor = Color.Red;
                    water.ReflectionColor = Color.White;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D4))
                {
                    water.WaveFrequency = .5f;
                    water.WaveAmplitude = .3f;
                    water.BumpHeight = 1f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D5))
                {
                    water.WaveFrequency = .1f;
                    water.WaveAmplitude = .1f;
                    water.BumpHeight = .1f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D6))
                {
                    water.WaveFrequency = .1f;
                    water.WaveAmplitude = .5f;
                    water.BumpHeight = .1f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D7))
                {
                    water.WaveFrequency = .1f;
                    water.WaveAmplitude = 2f;
                    water.BumpHeight = 1f;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D8))
                {
                    water.WaveFrequency = 0;
                    water.WaveAmplitude = 0;
                    water.BumpHeight = 0;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.D9))
                {
                    water.WaveFrequency = 0;
                    water.WaveAmplitude = 0;
                    water.BumpHeight = 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D0))
                    water.SetDefault();
            }
             */
            #endregion

            #region Test notify
            
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.U) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.U)) {
                RandomHelper r = RandomHelper.Instance;
                //hud.Notify(new Message("test notify " + r.Random.Next(20).ToString(), 3, r.Color, true));
                hud.Notify(new Message("For updates visit http://mrq.cz/ourcity",5,Color.White,false));
            }

            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.I) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.I)) {
                hud.NotifyPsycho = !hud.NotifyPsycho;
            }
            #endregion

            #region Toggle AI cars on minimap
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.T) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.T)) {
                AICarsInMinimap = !AICarsInMinimap;
            }
            #endregion

            #region Skip mission
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F10) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F10)) {
                hud.ClearObjectives();
                activeMission.Objects.Clear();
                activeMission.Enabled = false;
                activeMission.Finished = true;
            }
            #endregion

            #region Scene control
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.D1) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.D1)) {
                LoadScene(1);
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.D2) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.D2)) {
                LoadScene(2);
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.D3) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.D3)) {
                LoadScene(3);
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.D4) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.D4)) {
                LoadScene(4);
            }
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.D5) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.D5)) {
                LoadScene(5);
            }
            #endregion

            #region Set reset location
            if (input.CurrentKeyboardStates[playerIndex].IsKeyUp(Keys.F) && input.LastKeyboardStates[playerIndex].IsKeyDown(Keys.F)) {
                SetCarResetLocation();
            }
            #endregion

        }

        #endregion

        #region Public functions
        /// <summary>
        /// Reset car to default position or on last waypoint
        /// </summary>
        public void ResetCar()
        {
            carObject.Car.Chassis.Body.MoveTo(carResetLocation.Item1, carResetLocation.Item2);
            if (hud != null)
                hud.Enabled = true;

        }

        /// <summary>
        /// Set car default reset position to actual car position
        /// </summary>
        public void SetCarResetLocation() {
            carResetLocation = new Tuple<Vector3,Matrix>(carObject.PhysicsBody.Position, carObject.PhysicsBody.Orientation);
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            List<DrawableGameComponent> toDraw = new List<DrawableGameComponent>();
            foreach (DrawableGameComponent c in Components)
                if (c.Visible)
                    toDraw.Add(c);

            // only draw debug objects (waypoints) if debug enabled
            if (debug.Enabled)
            {
                toDraw.AddRange(debug.debugObjects);
            }

            foreach (DrawableGameComponent c in toDraw)
                c.Draw(gameTime);

            hud.Draw(gameTime);
        }

        #endregion

        #region Misc

        #region IDrawable Members

        public int DrawOrder
        {
            get;
            set;
        }

        public event EventHandler<EventArgs> DrawOrderChanged;

        public bool Visible
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<EventArgs> VisibleChanged;
        

        #endregion

        #region ImmovableSkinPredicate
        class ImmovableSkinPredicate : CollisionSkinPredicate1
        {
            public override bool ConsiderSkin(CollisionSkin skin0)
            {
                if (skin0.Owner != null && !skin0.Owner.Immovable)
                    return true;
                else
                    return false;
            }
        }
        #endregion

        #endregion
    }
}
