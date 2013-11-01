#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace OurCityEngine.Screens
{
    /// <summary>
    /// Single entry in menu screen from sprite
    /// </summary>
    public class SpriteMenuEntry : MenuEntry
    {
        #region Fields

        /// <summary>
        /// Used sprite
        /// </summary>
        Texture2D sprite;
        Rectangle selectedRect;
        Rectangle notSelectedRect;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs menu entry with selected sprite and rectangles
        /// </summary>
        /// <param name="sprite">Sprite texture</param>
        /// <param name="selectedRect">Rectangle shown if selected</param>
        /// <param name="notSelectedRect">Rectangle shown if not selected</param>
        public SpriteMenuEntry(string text, Texture2D sprite, Rectangle selectedRect, Rectangle notSelectedRect) : base(text)
        {
            this.sprite = sprite;
            this.selectedRect = selectedRect;
            this.notSelectedRect = notSelectedRect;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public override void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public override void Draw(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in yellow, otherwise white.
            Rectangle rect = isSelected ? selectedRect : notSelectedRect;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            
            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            //color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            Vector2 origin = new Vector2(0, 0);

            spriteBatch.Draw(sprite, position, rect, Color.White, 0, origin, 1, SpriteEffects.None, 0);
        }

        public override int GetWidth(MenuScreen screen) {
            return selectedRect.Width;
        }

        public override int GetHeight(MenuScreen screen) {
            return selectedRect.Height;
        }

        #endregion
    }
}
