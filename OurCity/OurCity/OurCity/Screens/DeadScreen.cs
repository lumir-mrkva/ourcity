#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using OurCityEngine.Screens;
#endregion

namespace OurCity.Screens
{
   
    class DeadScreen : MenuScreen
    {
        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public DeadScreen()
            : base(Strings.Dead)
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry(Strings.Reset);
            MenuEntry quitGameMenuEntry = new MenuEntry(Strings.Quit);
            
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += Reset;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input

        void Reset(object sender, PlayerIndexEventArgs e)
        {
            PlayerManager.Instance.GameScreen.ResetCar();
            OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(Strings.AreYouSure);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
           
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            base.Draw(gameTime);
            
        }


        #endregion

    }
}
