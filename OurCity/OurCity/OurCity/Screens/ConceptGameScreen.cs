#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

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
#endregion

namespace OurCity.Screens
{
    /// <summary>
    /// This screen implements the actual game logic.
    /// 
    /// </summary>
    public class ConceptGameScreen : GameScreen, IGameComponent
    {
        #region Fields
        protected PhysicsSystem physicSystem;

        Model islandModel, camaroModel, wheelModel, waypointModel;

        TriangleMeshObject island;
        CarObject carObject;

        ChaseCamera chaseCamera;
        Layer buildings;

        ContentManager content;

        HudOverlay hud;

        Debug debug;

        List<DrawableGameComponent> components;

        TrafficManager trafficManager;

        Water water;
        SkyDome sky;

        public Camera Camera { get { return CameraManager.Instance.Cameras["Default"]; } }
        public List<DrawableGameComponent> Components { get { return components; } }
        OurCityEngine.Debug.DebugDrawer DebugDrawer { get; set; }

        WaypointManager wpManager;
        DummyWaypointHandler wpHandler;
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public ConceptGameScreen()
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
            chaseCamera.DesiredPositionOffset = new Vector3(0.0f, 5f, 12f);
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
            sky.Theta = 2.4f;
            
            sky.Parameters.NumSamples = 10;
            sky.Initialize();
            sky.Theta = 1.8f;
            Components.Add(sky);
            
            // water
            water = new Water(ScreenManager.Game, "Map/Textury/SkyBox/skybox02", new Vector3(-1310,-25,-1900),new Vector3(20,5,31));
            water.Initialize();
            water.BumpSpeed = new Vector2(0, .01f);
            water.BumpHeight = 0.5f;
            water.WaveFrequency = 0.5f;
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
            buildings = new Buildings(ScreenManager.Game);
            buildings.Initialize();
            buildings.CullingEnabled = true;
            Components.Add(buildings);

            // traffic manager
            trafficManager = new TrafficManager(ScreenManager.Game, content, physicSystem.Gravity.Length());
            trafficManager.AddCar();
            trafficManager.AddCar();
            trafficManager.AddCar();
            trafficManager.AddCar();

            // show traffic manager waypoints for debuging purposes
            debug.ShowPositions(trafficManager.Waypoints);

            foreach (DrawableGameComponent c in trafficManager.Components)
                this.Components.Add(c);

            // car
            carObject = new CarObject(ScreenManager.Game,
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
                physicSystem.Gravity.Length()
                );

            carObject.Car.Chassis.Body.MoveTo(new Vector3(0, 2, 10), Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
            carObject.Car.EnableCar();
            carObject.Car.Chassis.Body.AllowFreezing = false;
            this.Components.Add(carObject);

            #region Waypoints
            waypointModel = content.Load<Model>("Waypoints/waypoint_box");
            wpManager = new WaypointManager();

            List<Vector3> positions = new List<Vector3>();
            positions.Add(new Vector3(102.1698f, -19.75958f, 65.49726f));
            positions.Add(new Vector3(102.3883f, 0.22563f, 193.8246f));
            positions.Add(new Vector3(107.5732f, 0.2405609f, 938.9814f));
            positions.Add(new Vector3(88.17076f, 0.2415653f, 958.3015f));
            positions.Add(new Vector3(-92.6205f, 0.2404301f, 960.243f));
            positions.Add(new Vector3(-109.479f, 0.3087659f, 945.4579f));
            positions.Add(new Vector3(-104.5664f, 0.2465245f, 830.2363f));
            positions.Add(new Vector3(-109.1518f, 0.2462023f, 328.932f));
            positions.Add(new Vector3(-110.2776f, -19.757f, 223.4564f));
            positions.Add(new Vector3(-60.85434f, -19.86155f, 171.6528f));
            positions.Add(new Vector3(-15.97892f, -19.85956f, 106.8634f));
            positions.Add(new Vector3(25.59191f, -19.85958f, 5.352768f));
            positions.Add(new Vector3(53.71033f, -19.85888f, -35.43206f));
            positions.Add(new Vector3(91.76981f, -19.75591f, -32.53667f));


            List<Waypoint> wps = new List<Waypoint>();
            foreach (Vector3 v in positions)
            {
                wps.Add(new Waypoint(ScreenManager.Game, waypointModel, new Vector3(10, 10, 0.01f), Matrix.Identity, v));
            }

            foreach (Waypoint w in wps)
            {
                Components.Add(w);
                wpManager.AddWatchedWaypoint(w);
            }

            wpManager.AddWatchedCar(carObject);


            // Register handler to be notified
            wpHandler = new DummyWaypointHandler(ScreenManager.Game, wpManager, carObject);
            wpManager.WaypointCollision += wpHandler.HandleWaypointCollision;
            Components.Add(wpHandler);
            #endregion

            //HUD
            hud = new HudOverlay(ScreenManager);
            hud.Initialize(); // je treba zavolat jinak neprobehne LoadContent();

            base.LoadContent();
            ScreenManager.Game.ResetElapsedTime();


            Vector3 pos = island.PhysicsBody.Position;
            
            Logger.Instance.Log("Island position: (" + pos.X + ", " + pos.Y + ", " + pos.Z + ")");
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

            #region Update to hud
            if (hud != null && hud.Enabled)
            {
                UpdateHud();
            }
            #endregion

            #region Update camera
            // Update the camera to chase the new target
            UpdateCameraChaseTarget();
            CameraManager.Instance.Update(gameTime);
            #endregion

            if (IsActive)
            {
                CheckDeath();
                physicSystem.Integrate((float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond);
                wpHandler.Enabled = true;
            }
            else
            {
                wpHandler.Enabled = false;
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
        private void UpdateDebug()
        {
            debug.CarPosition = carObject.PhysicsBody.Position;
            debug.CarFWOrientation = carObject.PhysicsBody.Orientation.Forward;
            debug.CarVelocity = carObject.PhysicsBody.Velocity;
            debug.SkyTime = sky.Theta;
            debug.Culling = buildings.CulledObjects + " / " + buildings.Objects.Count; 
        }

        private void UpdateHud()
        {
            // assuming velocity is in m/s, converting to km/h
            hud.CarVelocity = (float)Math.Round(carObject.PhysicsBody.Velocity.Length() * 3.6f);

            // set main message shown in hud
            // for example distance to the waypoint
            //TODO make it update only once a while (this could save like 40 fps)
            float distance = Vector3.Distance(carObject.PhysicsBody.Position, new Vector3(0, 0, 0)) - 80;
            float distanceToWP = wpHandler.GetDistanceToNextWaypoint();

            //clear hud message
            hud.MainMessage = "";

            if (wpHandler.GetRemainingTime().TotalSeconds == 10) {
            } else { 
            hud.MainMessage = "Next WP " + Math.Round(distanceToWP) + " m";

            if (wpHandler.GetRemainingTime().TotalMilliseconds > 0)
                hud.MainMessage += "\nTime:" + Math.Round(wpHandler.GetRemainingTime().TotalSeconds,2).ToString();
            else
                hud.MainMessage += "\n You lost!";
            }
            // send player location (for minimap)
            hud.PlayerLocation = carObject.PhysicsBody.Position;
        }
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
            chaseCamera.Up = carObject.Car.Chassis.Body.Orientation.Up;

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
                Logger.Instance.Log("car position: (" + pos.X + ", " + pos.Y + ", " + pos.Z + ")");

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
            #endregion

            
        }

        #endregion

        #region Public functions
        public void ResetCar()
        {
            hud.Enabled = true;
            if (wpHandler.GetRemainingTime().TotalMilliseconds > 0)
            { // We still have time
                // Respawn on last waypoint
                Vector3 newPos;
                if (wpHandler.PrevWaypoint == null)
                {
                    newPos = new Vector3(0, 2, 10);
                }
                else
                {
                    newPos = wpHandler.PrevWaypoint.PhysicsBody.Position;
                }

                carObject.Car.Chassis.Body.MoveTo(newPos, Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
            }
            else
            { // We lost or havent even started
                carObject.Car.Chassis.Body.MoveTo(new Vector3(0, 2, 10), Matrix.CreateFromYawPitchRoll(-1.57f, 0, 0));
                wpHandler.Initialize();
            }
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
