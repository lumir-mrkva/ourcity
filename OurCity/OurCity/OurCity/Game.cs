#region File Description
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace OurCity
{
    public class OurCityGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;


        // By preloading any assets used by UI rendering, we avoid framerate glitches
        // when they suddenly need to be loaded in the middle of a menu transition.
        static readonly string[] preloadAssets =
        {
            "gradient",
        };

        public GameScreen Play { get; set; }
        
        #endregion

        #region Initialization


        /// <summary>
        /// The main game constructor.
        /// </summary>
        public OurCityGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            // Create the screen manager component.
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
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
            graphics.GraphicsDevice.Clear(Color.Black);

            // The real drawing happens inside the screen manager component.
            base.Draw(gameTime);
        }

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

        #endregion
    }
}
