#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System.Globalization;
using Microsoft.Xna.Framework;
using OurCityEngine.Screens;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace OurCity.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D sprite;
        
        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base(Strings.MainMenu)
        {
            
            
        }

        public override void LoadContent() {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            sprite = content.Load<Texture2D>("Menus/text");

            // Create our menu entries.
            MenuEntry playGameMenuEntry = new SpriteMenuEntry(Strings.PlayGame, sprite,  new Rectangle(0, 327, 200, 50), new Rectangle(0, 147, 200, 50));
            MenuEntry optionsMenuEntry = new MenuEntry(Strings.Options);
            MenuEntry exitMenuEntry = new SpriteMenuEntry(Strings.Exit, sprite, new Rectangle(0, 448, 177, 50), new Rectangle(0, 269, 200, 50));

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            //optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            //MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);
        }

        public override void UnloadContent() {
            base.UnloadContent();
        }
        #endregion

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);
            GraphicsDevice graphics = ScreenManager.GraphicsDevice;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 position = new Vector2(graphics.Viewport.Width / 2, graphics.Viewport.Height / 2 - 200);

            spriteBatch.Begin();

            spriteBatch.Draw(sprite, position, new Rectangle(0,0,808,112), Color.White, 0, new Vector2(808/2,112/2), 1, SpriteEffects.None, 0);

            spriteBatch.End();
            
        }


        #region Handle Input


        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }


        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(Strings.AreYouSure);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }


        #endregion
    }
}
