#region Using
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using JigLibX.Physics;
using Microsoft.Xna.Framework.Input;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Cameras;
#endregion

namespace OurCity
{
    /// <summary>
    /// Used for adding debugging information to the screen
    /// </summary>
    public class Debug : DrawableGameComponent
    {
        #region Private fields
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SpriteFont spriteFont2;
        Model sphereModel;
        OurCityGame game;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        int bbWidth, bbHeight;
        #endregion

        #region Initialization
        public Debug(OurCityGame game)
            : base(game)
        {
            this.game = game;
            content = new ContentManager(game.Services, "Content");
            DrawCarPosition = true;
            
        }
        
        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        protected override void LoadContent()
        {
            spriteBatch = game.ScreenManager.SpriteBatch;
            spriteFont = content.Load<SpriteFont>("Fonts/FpsFont");
            spriteFont2 = content.Load<SpriteFont>("Fonts/debug");
            sphereModel = content.Load<Model>("Objects/sphere");

            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

        }

        protected override void UnloadContent()
        {
                content.Unload();
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            KeyboardState state = Keyboard.GetState();
            if (!DrawHelp && state.IsKeyDown(Keys.H)) DrawHelp = true;
            if (DrawHelp && state.IsKeyUp(Keys.H)) DrawHelp = false;

        }
        #endregion

        #region Public fields
        public bool DrawHelp { get; internal set; }
        public Vector3 CarVelocity { get; set; }
        public bool DrawCarPosition { get; internal set; }   
        public Vector3 CarPosition { get; internal set; }
        public Vector3 CarFWOrientation { get; internal set; }
        public Vector3 CarBWOrientation { get; internal set; }
        public int CarOrientation { get; internal set; }
        public float SkyTime { get; internal set; }
        public string Culling;
        public string Physics { get; set; }
        public Vector2 Resolution { get; set; }
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            if (!Enabled)
            {
                game.RestoreRenderState();
                return;
            };

            frameCounter++;

            string fps = frameRate.ToString();

            //TODO it should be set only if viewport change - make work GraphicsDevice_DeviceReset
            bbWidth = GraphicsDevice.Viewport.Width;
            bbHeight = GraphicsDevice.Viewport.Height;

            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, fps, new Vector2(11, 6), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(12, 7), Color.Yellow);
            
            if (!DrawHelp)
            {
                spriteBatch.DrawString(spriteFont2, "hold 'h' for help, 'c' for debug drawer and toggle debug with 'x'", new Vector2(0, bbHeight - 15*2), Color.White);
            }
            else
            {
                spriteBatch.DrawString(spriteFont2, "reset car with R, toggle HUD with J, toggle fullscreen by F4", new Vector2(0, bbHeight - 15 * 4), Color.YellowGreen);
                spriteBatch.DrawString(spriteFont2, "change camera modes F5-F8, cycle time of day by holding O and P", new Vector2(0, bbHeight - 15 * 3), Color.YellowGreen);
                spriteBatch.DrawString(spriteFont2, "use the arrow keys to drive the car and B to handBrake, log car position by L", new Vector2(0, bbHeight - 15 *2), Color.YellowGreen);
            }

            if (DrawCarPosition)
            {

                spriteBatch.DrawString(spriteFont2, ""+Physics, new Vector2(0f, 29f), Color.Black);
                spriteBatch.DrawString(spriteFont2, "" + Physics, new Vector2(1, 30f), Color.White);

                string StringPosition = "Resolution: " + Resolution.X + "x" + Resolution.Y;
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 12), Color.White);

                StringPosition = "" + Culling;
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 11), Color.White);

                StringPosition = "Sky time: "
                    + SkyTime;
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 9), Color.White);

                StringPosition = "Car FW orientation: X: "
                    + Math.Round(CarFWOrientation.X,3)
                    + " Y:" + Math.Round(CarFWOrientation.Y,3)
                    + " Z:" + Math.Round(CarFWOrientation.Z,3);
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 8), Color.White);

                StringPosition = "Car position: X: " 
                    + (int)Math.Round (CarPosition.X)
                    + " Y:" + (int)Math.Round(CarPosition.Y)
                    + " Z:" + (int)Math.Round(CarPosition.Z);
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 7), Color.White);
                Vector3 camPos = CameraManager.Instance.Default.Position;
                StringPosition = "Cam position: X: "
                    + (int)Math.Round(camPos.X)
                    + " Y:" + (int)Math.Round(camPos.Y)
                    + " Z:" + (int)Math.Round(camPos.Z);
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 6), Color.White);
                string StringVel = "Car Velocity: " + Math.Round(CarVelocity.Length());
                spriteBatch.DrawString(spriteFont2, StringVel, new Vector2(0, bbHeight - 15 * 5), Color.Yellow);
            }

            spriteBatch.End();

            game.RestoreRenderState();

            
        }
        #endregion

        #region Show positions
        public List<PhysicObject> debugObjects = new List<PhysicObject>();
        

        /// <summary>
        /// Shows given positions as DebugObjects (small spheres)
        /// </summary>
        /// <param name="positions"></param>
        public void ShowPositions(List<Vector3> positions)
        {
            foreach (Vector3 pos in positions)
            {
                SpawnDebugObject(pos);
            }
        }
        
        /// <summary>
        /// Creates DebugObject
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="ori">Orientation</param>
        public void SpawnDebugObject(Vector3 pos)
        {
            PhysicObject physicObj;
            physicObj = new DebugObject(Game, sphereModel, 0.5f, Matrix.Identity, pos);
            debugObjects.Add(physicObj);
        }
        #endregion

        
    }
}
