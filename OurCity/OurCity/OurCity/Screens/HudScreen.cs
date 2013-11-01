using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OurCity.Screens
{
        public class HudScreen : GameScreen
        {
            #region Fields


            #endregion

            #region Properties




            #endregion

            #region Initialization


            /// <summary>
            /// Constructor.
            /// </summary>
            public HudScreen()
            {
            }


            #endregion

            #region Handle Input


            /// <summary>
            /// Responds to user input, changing the selected entry and accepting
            /// or cancelling the menu.
            /// </summary>
//             public override void HandleInput(InputState input)
//             {
//                 
//             }


            #endregion

            #region Update and Draw



            /// <summary>
            /// Updates.
            /// </summary>
            public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                           bool coveredByOtherScreen)
            {
                base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                
            }


            /// <summary>
            /// Draws.
            /// </summary>
            public override void Draw(GameTime gameTime)
            {
                GraphicsDevice graphics = ScreenManager.GraphicsDevice;
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
                SpriteFont font = ScreenManager.Font;

                spriteBatch.Begin();

                String s = "test";

                // Draw the menu title centered on the screen
                Vector2 titlePosition = new Vector2(graphics.Viewport.Width / 2, 80);
                Vector2 titleOrigin = font.MeasureString(s) / 2;
                Color titleColor = Color.Black;
                float titleScale = 1.25f;


                spriteBatch.DrawString(font, s, titlePosition, titleColor, 0,
                                       titleOrigin, titleScale, SpriteEffects.None, 0);

                spriteBatch.End();
            }


            #endregion
        }
    }

