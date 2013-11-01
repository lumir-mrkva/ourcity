#region File Description
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurCityEngine.Screens;
using OurCity.Screens;
using OurCityEngine.Debug;
using OurCityEngine.Utils;
#endregion

namespace OurCity
{
    /// <summary>
    /// Game lives here
    /// </summary>
    public class OurCityGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;

        ScreenManager screenManager;

        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "Menus/gradient",
        };

        #endregion

        #region Initialization

        public Debug Debug { get; set; }

        public ScreenManager ScreenManager { get { return screenManager; } }

        /// <summary>
        /// The main game constructor.
        /// </summary>
        public OurCityGame()
        {

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true; 
            this.Window.Title = "OurCity beta";

            Content.RootDirectory = "Content";

            //logging
            Logger.Instance.Log("game starting");

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.SynchronizeWithVerticalRetrace = false;

            // Create the screen manager component.
            screenManager = new ScreenManager(this, graphics);

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            //DEBUG
            Debug = new Debug(this);
            Debug.UpdateOrder = 1000;
            Debug.Enabled = false;
            Components.Add(Debug);

            DebugManager.Instance.Drawer = new OurCityEngine.Debug.DebugDrawer(this);
            DebugManager.Instance.Drawer.Enabled = false;
            Components.Add(DebugManager.Instance.Drawer);

        }


        /// <summary>
        /// Loads graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            foreach (string asset in preloadAssets)
            {
                Content.Load<object>(asset);
            }
        }


        #endregion

        #region Draw


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }


        #endregion

        public void RestoreRenderState()
        {
            //this.graphics.GraphicsDevice.RenderState.DepthBufferEnable = true;
            this.graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //this.graphics.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            //this.graphics.GraphicsDevice.RenderState.AlphaTestEnable = true;
            this.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            //this.graphics.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
            //this.graphics.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            this.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
        }


    }
}
