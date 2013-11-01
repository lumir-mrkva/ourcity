using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace OurCity.Screens
{
    class DebugScreen : GameScreen
    {
        ContentManager content;
        SpriteFont spriteFont;
        SpriteFont spriteFont2;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        int bbWidth, bbHeight;

        public DebugScreen()
        {
            
        }

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            bbWidth = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            ScreenManager.GraphicsDevice.DeviceReset += new EventHandler<EventArgs>(GraphicsDevice_DeviceReset);
            GraphicsDevice_DeviceReset(null, null);
            spriteFont = content.Load<SpriteFont>("Fonts/FpsFont");
            spriteFont2 = content.Load<SpriteFont>("Fonts/debug");

            bbWidth = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = ScreenManager.GraphicsDevice.PresentationParameters.BackBufferHeight;
        }

        public override void UnloadContent()
        {
                content.Unload();
        }


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

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

            if (!DrawCarPosition && state.IsKeyDown(Keys.P)) DrawCarPosition = true;
            if (DrawCarPosition && state.IsKeyUp(Keys.P)) DrawCarPosition = false;

        }

        public bool DrawHelp { get; internal set; }        
        
        public bool DrawCarPosition { get; internal set; }   
        public Vector3 CarPosition { get; internal set; }
        public Vector3 CarFWOrientation { get; internal set; }
        public Vector3 CarBWOrientation { get; internal set; }
        public int CarOrientation { get; internal set; }   

        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = frameRate.ToString();
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteFont = ScreenManager.Font;
            spriteFont2 = ScreenManager.Font;
            spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, fps, new Vector2(11, 6), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(12, 7), Color.Yellow);
            
            if (!DrawHelp)
            {
                spriteBatch.DrawString(spriteFont2, "press 'h' for help, 'p' for car position, 'c' for debug view", new Vector2(0, bbHeight - 15), Color.White);
            }
            else
            {
                spriteBatch.DrawString(spriteFont2, "use the arrow keys to drive the car", new Vector2(0, bbHeight - 15 * 3), Color.White);
                spriteBatch.DrawString(spriteFont2, "left mouse click to shoot a random body", new Vector2(0, bbHeight - 15 * 2), Color.White);
                spriteBatch.DrawString(spriteFont2, "use mousewheel to pick and drag objects", new Vector2(0, bbHeight - 15 * 1), Color.White);
                spriteBatch.DrawString(spriteFont2, "toggle fullscreen mode by F10", new Vector2(0, bbHeight - 15 * 4), Color.White);
                spriteBatch.DrawString(spriteFont2, "respawn scenes by 1 .. 0 number keys", new Vector2(0, bbHeight - 15 * 5), Color.White);
                spriteBatch.DrawString(spriteFont2, "press F1 for camera toggle, you can use wasd and rmb to navigate", new Vector2(0, bbHeight - 15 * 6), Color.White);
            }

            if (DrawCarPosition)
            {
                string StringPosition = "Car position: X: " 
                    + (int)Math.Round (CarPosition.X)
                    + " Y:" + (int)Math.Round(CarPosition.Y)
                    + " Z:" + (int)Math.Round(CarPosition.Z);
                spriteBatch.DrawString(spriteFont2, StringPosition, new Vector2(0, bbHeight - 15 * 7), Color.White);

                string StringFW = "Car FW orientation: X: "
                                    + (double)Math.Round(CarFWOrientation.X)
                                    + " Y:" + (double)Math.Round(CarFWOrientation.Y)
                                    + " Z:" + (double)Math.Round(CarFWOrientation.Z);
                string StringBW = "Car BW orientation: X: "
                                                    + (double)Math.Round(CarBWOrientation.X)
                                                    + " Y:" + (double)Math.Round(CarBWOrientation.Y)
                                                    + " Z:" + (double)Math.Round(CarBWOrientation.Z);
                spriteBatch.DrawString(spriteFont2, StringFW, new Vector2(0, bbHeight - 15 * 6), Color.White);
                spriteBatch.DrawString(spriteFont2, StringBW, new Vector2(0, bbHeight - 15 * 5), Color.White);

                string AppOrientation = "";
                switch (CarOrientation) { 
                    case -1:
                        AppOrientation = "DOWN";
                        break;
                    case 0:
                        AppOrientation = "STAY";
                        break;
                    case 1:
                        AppOrientation = "UP";
                        break;
                    case 2:
                        AppOrientation = "LEFT";
                        break;
                    case 3:
                        AppOrientation = "RIGHT";
                        break;
                    default:
                        AppOrientation  = "" + CarOrientation;
                        break;
                    
                }
                spriteBatch.DrawString(spriteFont2, "Car orientation: " + AppOrientation, new Vector2(0, bbHeight - 15 * 4), Color.White);
            }

            spriteBatch.End();

            
            ((OurCityGame)ScreenManager.Game).RestoreRenderState();
        }
    }
    }

