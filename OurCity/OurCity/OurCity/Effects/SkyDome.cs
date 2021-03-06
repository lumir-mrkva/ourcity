/*
 * Skydome Component
 * 
 * Alex Urbano �lvarez
 * XNA Community Coordinator
 * 
 * goefuika@gmail.com
 * 
 * http://elgoe.blogspot.com
 * http://www.codeplex.com/XNACommunity
 */
// ported to XNA4 by team 10 - Lumir Mrkva
#region Using Statements

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;
using OurCityEngine.Cameras;

#endregion

namespace OurCity.Effects
{
	/// <summary>
	/// This is a game component that implements IUpdateable.
	/// </summary>
	public class SkyDome : Microsoft.Xna.Framework.DrawableGameComponent
	{

		#region Properties

		private float fTheta;
		private float fPhi;

		private float previousTheta, previousPhi;

		private bool realTime;
		
		private Camera camera {
            get { return CameraManager.Instance.Default; }
        }

		Game game;

		Texture2D mieTex, rayleighTex;
		RenderTarget2D mieRT, rayleighRT;

		Texture2D moonTex, glowTex, starsTex;

        Texture2D permTex;

		Effect scatterEffect, texturedEffect, noiseEffect;

		QuadRenderComponent quad;

		SkyDomeParameters parameters;

		VertexPositionTexture[] domeVerts, quadVerts;
		short[] ib, quadIb;

		int DomeN;
		int DVSize;
		int DISize;

		Vector4 sunColor;

		private float inverseCloudVelocity;
		private float cloudCover;
		private float cloudSharpness;
		private float numTiles;
        private int previousdWidth;
        private int dwidth;
		#endregion

		#region Gets/Sets
		/// <summary>
		/// Gets/Sets Theta value
		/// </summary>
		public float Theta { get { return fTheta; } set { fTheta = value; } }

		/// <summary>
		/// Gets/Sets Phi value
		/// </summary>
		public float Phi { get { return fPhi; } set { fPhi = value; } }
			
		/// <summary>
		/// Gets/Sets actual time computation
		/// </summary>
		public bool RealTime 
		{ 
			get { return realTime; } 
			set { realTime = value; } 
		}

		/// <summary>
		/// Gets/Sets the SkyDome parameters
		/// </summary>
		public SkyDomeParameters Parameters { get { return parameters; } set { parameters = value; } }

		/// <summary>
		/// Gets the Sun color
		/// </summary>
		public Vector4 SunColor { get { return sunColor; } }

		/// <summary>
		/// Gets/Sets InverseCloudVelocity value
		/// </summary>
		public float InverseCloudVelocity { get { return inverseCloudVelocity; } set { inverseCloudVelocity = value; } }

		/// <summary>
		/// Gets/Sets CloudCover value
		/// </summary>
		public float CloudCover { get { return cloudCover; } set { cloudCover = value; } }

		/// <summary>
		/// Gets/Sets CloudSharpness value
		/// </summary>
		public float CloudSharpness { get { return cloudSharpness; } set { cloudSharpness = value; } }

		/// <summary>
		/// Gets/Sets CloudSharpness value
		/// </summary>
		public float NumTiles { get { return numTiles; } set { numTiles = value; } }

		#endregion

		#region Contructor

		public SkyDome(Game game)
			: base(game)
		{
			this.game = game;

			realTime = false;
			
			parameters = new SkyDomeParameters();

			quad = new QuadRenderComponent(game);
			game.Components.Add(quad);

			fTheta = 0.0f;
			fPhi = 0.0f;

			DomeN = 32;

			GeneratePermTex();
		}

		#endregion

		#region Initialize
		/// <summary>
		/// Allows the game component to perform any initialization it needs to before starting
		/// to run.  This is where it can query for any required services and load content.
		/// </summary>
		public override void Initialize()
		{
            // You can use SurfaceFormat.Color to increase performance / reduce quality
            PresentationParameters pp = game.GraphicsDevice.PresentationParameters;

            this.mieRT = new RenderTarget2D(game.GraphicsDevice, 128, 64, true,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            this.rayleighRT = new RenderTarget2D(game.GraphicsDevice, 128, 64, true,
                SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

			// Clouds constantes
			inverseCloudVelocity = 16f;
			CloudCover = -0.1f;
			CloudSharpness = 0.5f;
			numTiles = 16.0f;

			base.Initialize();
		}
		#endregion

		#region Load

		protected override void LoadContent()
		{
			
			scatterEffect = game.Content.Load<Effect>("Map/Shaders/scatter");
            texturedEffect = game.Content.Load<Effect>("Map/Shaders/Textured");
            noiseEffect = game.Content.Load<Effect>("Map/Shaders/SNoise");

//             //oldDepthBuffer = this.GraphicsDevice.DepthStencilBuffer;
// 
//             //this.newDepthBuffer = new DepthStencilBuffer(game.GraphicsDevice,
//                 game.GraphicsDevice.PresentationParameters.BackBufferWidth,
//                 game.GraphicsDevice.PresentationParameters.BackBufferHeight,
//                 game.GraphicsDevice.DepthStencilBuffer.Format, MultiSampleType.None, 0);

            moonTex = game.Content.Load<Texture2D>("Map/Textury/SkyDome/moon");
            glowTex = game.Content.Load<Texture2D>("Map/Textury/SkyDome/moonglow");
            starsTex = game.Content.Load<Texture2D>("Map/Textury/SkyDome/starfield");

			GenerateDome();
			GenerateMoon();

			base.LoadContent();

		}

		#endregion

		#region Update
		/// <summary>
		/// Allows the game component to update itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Update(GameTime gameTime)
		{
			KeyboardState keyState = Keyboard.GetState();

            #region Control skydome
            float step = (float)gameTime.ElapsedGameTime.TotalSeconds;
            RealTime = false;
            if (keyState.IsKeyDown(Keys.O) ||
                GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed)
                Theta -= 0.2f * step;
            if (keyState.IsKeyDown(Keys.P) ||
                GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed)
                Theta += 0.2f * step;
            if (Theta > (float)Math.PI * 2.0f)
                Theta = Theta - (float)Math.PI * 2.0f;
            if (Theta < 0.0f)
               Theta = (float)Math.PI * 2.0f + Theta;


            #endregion

			if (realTime)
			{
				int minutos = DateTime.Now.Hour*60 + DateTime.Now.Minute ;
				this.fTheta = (float)minutos * (float)(Math.PI) / 12.0f / 60.0f;
            }

			parameters.LightDirection = this.GetDirection();
			parameters.LightDirection.Normalize();
		   
			//base.Update(gameTime);
		}
		#endregion

		#region Draw
		/// <summary>
		/// Draws the component.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		public override void Draw(GameTime gameTime)
		{

			Matrix View = camera.View;
			Matrix Projection = camera.Projection;
			Matrix World = Matrix.CreateTranslation(camera.Position.X,
				camera.Position.Y,
				camera.Position.Z);


            int dwidth = game.GraphicsDevice.Viewport.Width;

            
			if (previousTheta != fTheta || previousPhi != fPhi || previousdWidth != dwidth)
				UpdateMieRayleighTextures();

            previousdWidth = dwidth;

			this.sunColor = this.GetSunColor(-this.fTheta, 2);

			game.GraphicsDevice.Clear(Color.CornflowerBlue);
            


//             game.GraphicsDevice.RenderState.DepthBufferEnable = false;
//             game.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
            DepthStencilState ds = GraphicsDevice.DepthStencilState;

            //set render states
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

			//EDIT
			//game.GraphicsDevice.RenderState.CullMode = CullMode.None;
			GraphicsDevice.RasterizerState = RasterizerState.CullNone;

			scatterEffect.CurrentTechnique = scatterEffect.Techniques["Render"];
			//scatterEffect.Begin();
			scatterEffect.Parameters["txMie"].SetValue(this.mieTex);
			scatterEffect.Parameters["txRayleigh"].SetValue(this.rayleighTex);
			scatterEffect.Parameters["WorldViewProjection"].SetValue(World * View * Projection);
			scatterEffect.Parameters["v3SunDir"].SetValue(new Vector3(-parameters.LightDirection.X,
				-parameters.LightDirection.Y, -parameters.LightDirection.Z));
			scatterEffect.Parameters["NumSamples"].SetValue(parameters.NumSamples);
			scatterEffect.Parameters["fExposure"].SetValue(parameters.Exposure);
			scatterEffect.Parameters["StarsTex"].SetValue(starsTex);
			if (fTheta < Math.PI / 2.0f || fTheta > 3.0f * Math.PI / 2.0f)
				scatterEffect.Parameters["starIntensity"].SetValue((float)Math.Abs(
					Math.Sin(Theta + (float)Math.PI / 2.0f)));
			else
				scatterEffect.Parameters["starIntensity"].SetValue(0.0f);

			foreach (EffectPass pass in scatterEffect.CurrentTechnique.Passes)
			{
				pass.Apply();

				//game.GraphicsDevice.VertexDeclaration = vertexDecl;
				game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
					(PrimitiveType.TriangleList, domeVerts, 0, this.DVSize, ib, 0, this.DISize);

				//pass.End();
			}

			//scatterEffect.End();

			DrawGlow();
			DrawMoon();
			DrawClouds(gameTime);

//             game.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
//             game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = ds;

			previousTheta = this.fTheta;
			previousPhi = this.fPhi;

		}

		#region DrawMoon

		private void DrawMoon()
		{
			BlendState bs = GraphicsDevice.BlendState;

			//set render states
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
//             game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
//             game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
//             game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

			texturedEffect.CurrentTechnique = texturedEffect.Techniques["Textured"];
			//texturedEffect.Begin();
			texturedEffect.Parameters["World"].SetValue(
				Matrix.CreateRotationX(this.Theta + (float)Math.PI / 2.0f) *
				Matrix.CreateRotationY(-this.Phi + (float)Math.PI / 2.0f) *
				Matrix.CreateTranslation(parameters.LightDirection.X * 15,
				parameters.LightDirection.Y * 15,
				parameters.LightDirection.Z * 15) *
				Matrix.CreateTranslation(camera.Position.X,
				camera.Position.Y,
				camera.Position.Z));
			texturedEffect.Parameters["View"].SetValue(camera.View);
			texturedEffect.Parameters["Projection"].SetValue(camera.Projection);
			texturedEffect.Parameters["Texture"].SetValue(this.moonTex);
			if (fTheta < Math.PI / 2.0f || fTheta > 3.0f * Math.PI / 2.0f)
				texturedEffect.Parameters["alpha"].SetValue((float)Math.Abs(
					Math.Sin(Theta + (float)Math.PI / 2.0f)));
			else
				texturedEffect.Parameters["alpha"].SetValue(0.0f);
			foreach (EffectPass pass in texturedEffect.CurrentTechnique.Passes)
			{
				pass.Apply();

				game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
					(PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);

				//pass.End();
			}
			//texturedEffect.End();

			//game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			GraphicsDevice.BlendState = bs;

		}

		#endregion

		#region DrawGlow

		private void DrawGlow()
		{
//             game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
//             game.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
//             game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			

			BlendState bs = GraphicsDevice.BlendState;

			//set render states
			GraphicsDevice.BlendState = BlendState.AlphaBlend;
					 

			texturedEffect.CurrentTechnique = texturedEffect.Techniques["Textured"];
			//texturedEffect.Begin();
			texturedEffect.Parameters["World"].SetValue(
				Matrix.CreateRotationX(this.Theta + (float)Math.PI / 2.0f) *
				Matrix.CreateRotationY(-this.Phi + (float)Math.PI / 2.0f) *
				Matrix.CreateTranslation(parameters.LightDirection.X * 5,
				parameters.LightDirection.Y * 5,
				parameters.LightDirection.Z * 5) *
				Matrix.CreateTranslation(camera.Position.X,
				camera.Position.Y,
				camera.Position.Z));//*
			texturedEffect.Parameters["View"].SetValue(camera.View);
			texturedEffect.Parameters["Projection"].SetValue(camera.Projection);
			texturedEffect.Parameters["Texture"].SetValue(this.glowTex);
			if (fTheta < Math.PI / 2.0f || fTheta > 3.0f * Math.PI / 2.0f)
				texturedEffect.Parameters["alpha"].SetValue((float)Math.Abs(
					Math.Sin(Theta + (float)Math.PI / 2.0f)));
			else
				texturedEffect.Parameters["alpha"].SetValue(0.0f);
			foreach (EffectPass pass in texturedEffect.CurrentTechnique.Passes)
			{
				pass.Apply();

				game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
					(PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);

				//pass.End();
			}
			//texturedEffect.End();

			//game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			// restore render states
			GraphicsDevice.BlendState = bs;
		}

		#endregion

		#region DrawClouds

		private void DrawClouds(GameTime gameTime)
		{
			//game.GraphicsDevice.RenderState.AlphaBlendEnable = true;
			
//             game.GraphicsDevice.RenderState.SourceBlend = Blend.One;
//             game.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

			BlendState bs = GraphicsDevice.BlendState;

			//set render states
			GraphicsDevice.BlendState = BlendState.AlphaBlend;

			noiseEffect.CurrentTechnique = noiseEffect.Techniques["Noise"];
		   // noiseEffect.Begin();
			noiseEffect.Parameters["World"].SetValue(Matrix.CreateScale(10000.0f)*
				Matrix.CreateTranslation(new Vector3(0,0,-900)) *
				Matrix.CreateRotationX((float)Math.PI/2.0f) *
				Matrix.CreateTranslation(camera.Position.X,
				camera.Position.Y,
				camera.Position.Z)
				);
			noiseEffect.Parameters["View"].SetValue(camera.View);
			noiseEffect.Parameters["Projection"].SetValue(camera.Projection);
			noiseEffect.Parameters["permTexture"].SetValue(this.permTex);
			noiseEffect.Parameters["time"].SetValue((float)gameTime.TotalGameTime.TotalSeconds / inverseCloudVelocity);
			noiseEffect.Parameters["SunColor"].SetValue(this.sunColor);
			noiseEffect.Parameters["numTiles"].SetValue(numTiles);
			noiseEffect.Parameters["CloudCover"].SetValue(cloudCover);
			noiseEffect.Parameters["CloudSharpness"].SetValue(cloudSharpness);
			
			foreach (EffectPass pass in noiseEffect.CurrentTechnique.Passes)
			{
				//pass.Begin();
				pass.Apply();
				game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTexture>
					(PrimitiveType.TriangleList, quadVerts, 0, 4, quadIb, 0, 2);

				
			}
			//noiseEffect.End();
			//game.GraphicsDevice.BlendState = BlendState.Opaque;
		   // game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			GraphicsDevice.BlendState = bs;
		}

		#endregion

		#endregion

		#region Private Methods

		#region Get Light Direction
		Vector4 GetDirection()
		{
			
			float y = (float)Math.Cos((double)this.fTheta);
			float x = (float)(Math.Sin((double)this.fTheta) * Math.Cos(this.fPhi));
			float z = (float)(Math.Sin((double)this.fTheta) * Math.Sin(this.fPhi));
			float w = 1.0f;

			return new Vector4(x,y,z,w);
		}
		#endregion

		#region UpdateMieRayleighTextures
        // TODO fix, this is probably problem in release version and when screen change, multiple render targets?
		void UpdateMieRayleighTextures()
		{
			/*game.GraphicsDevice.DepthStencilBuffer = newDepthBuffer;*/

            DepthStencilState ds = GraphicsDevice.DepthStencilState;

            //set render states
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

//             game.GraphicsDevice.SetRenderTarget(0, rayleighRT);
//             game.GraphicsDevice.SetRenderTarget(1, mieRT);
            GraphicsDevice.SetRenderTargets(rayleighRT, mieRT);

			game.GraphicsDevice.Clear(Color.CornflowerBlue);

			scatterEffect.CurrentTechnique = scatterEffect.Techniques["Update"];
			//scatterEffect.Begin();
			scatterEffect.Parameters["InvWavelength"].SetValue(parameters.InvWaveLengths);
			scatterEffect.Parameters["WavelengthMie"].SetValue(parameters.WaveLengthsMie);
			scatterEffect.Parameters["v3SunDir"].SetValue(new Vector3(-parameters.LightDirection.X,
				-parameters.LightDirection.Y, -parameters.LightDirection.Z));
			EffectPass pass = scatterEffect.CurrentTechnique.Passes[0];
			//pass.Begin();
			
			
            pass.Apply();
            quad.Render(Vector2.One * -1, Vector2.One);
			//pass.End();
			
			//scatterEffect.End();

//             game.GraphicsDevice.SetRenderTarget(1, null);
//             game.GraphicsDevice.SetRenderTarget(0, null);
// 
            this.mieTex = mieRT;
            this.rayleighTex = rayleighRT;
// 
            //mieRT.SaveAsPng(new FileStream("mieTex.png",FileMode.Create), 512, 512);
       //     rayleighTex.SaveAsPng(new FileStream("rayleighTex.png", FileMode.Create), 512, 512);
//             game.GraphicsDevice.DepthStencilBuffer = oldDepthBuffer;
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.DepthStencilState = ds;

			//mieTex.Save("mimie.dds", ImageFileFormat.Dds);
			//rayleighTex.Save("mirayleigh.dds", ImageFileFormat.Dds);
		}

		#endregion

		

		#region GenerateDome

		private void GenerateDome()
		{
			int Latitude = DomeN / 2;
			int Longitude = DomeN;
			DVSize = Longitude * Latitude;
			DISize = (Longitude - 1) * (Latitude - 1) * 2;
			DVSize *= 2;
			DISize *= 2;
            float angle = 180;

//             vertexDecl = new VertexDeclaration(GraphicsDevice,
//                                 VertexPositionTexture.VertexElements);

			domeVerts = new VertexPositionTexture[DVSize];

			// Fill Vertex Buffer
			int DomeIndex = 0;
			for (int i = 0; i < Longitude; i++)
			{
				double MoveXZ = 100.0f * (i / ((float)Longitude - 1.0f)) * MathHelper.Pi / angle;

				for (int j = 0; j < Latitude; j++)
				{
					double MoveY = MathHelper.Pi * j / (Latitude - 1);

					domeVerts[DomeIndex] = new VertexPositionTexture();
					domeVerts[DomeIndex].Position.X = (float)(Math.Sin(MoveXZ) * Math.Cos(MoveY));
					domeVerts[DomeIndex].Position.Y = (float)Math.Cos(MoveXZ);
					domeVerts[DomeIndex].Position.Z = (float)(Math.Sin(MoveXZ) * Math.Sin(MoveY));

					domeVerts[DomeIndex].Position *= 10.0f;

					domeVerts[DomeIndex].TextureCoordinate.X = 0.5f / (float)Longitude + i / (float)Longitude;
					domeVerts[DomeIndex].TextureCoordinate.Y = 0.5f / (float)Latitude + j / (float)Latitude;

					DomeIndex++;
				}
			}
			for (int i = 0; i < Longitude; i++)
			{
                double MoveXZ = 100.0 * (i / (float)(Longitude - 1)) * MathHelper.Pi / angle;

				for (int j = 0; j < Latitude; j++)
				{
					double MoveY = (MathHelper.Pi * 2.0) - (MathHelper.Pi * j / (Latitude - 1));

					domeVerts[DomeIndex] = new VertexPositionTexture();
					domeVerts[DomeIndex].Position.X = (float)(Math.Sin(MoveXZ) * Math.Cos(MoveY));
					domeVerts[DomeIndex].Position.Y = (float)Math.Cos(MoveXZ);
					domeVerts[DomeIndex].Position.Z = (float)(Math.Sin(MoveXZ) * Math.Sin(MoveY));

					domeVerts[DomeIndex].Position *= 10.0f;

					domeVerts[DomeIndex].TextureCoordinate.X = 0.5f / (float)Longitude + i / (float)Longitude;
					domeVerts[DomeIndex].TextureCoordinate.Y = 0.5f / (float)Latitude + j / (float)Latitude;

					DomeIndex++;
				}
			}

			// Fill index buffer
			ib = new short[DISize * 3];
			int index = 0;
			for (short i = 0; i < Longitude - 1; i++)
			{
				for (short j = 0; j < Latitude - 1; j++)
				{
					ib[index++] = (short)(i * Latitude + j);
					ib[index++] = (short)((i + 1) * Latitude + j);
					ib[index++] = (short)((i + 1) * Latitude + j + 1);

					ib[index++] = (short)((i + 1) * Latitude + j + 1);
					ib[index++] = (short)(i * Latitude + j + 1);
					ib[index++] = (short)(i * Latitude + j);
				}
			}
			short Offset = (short)(Latitude * Longitude);
			for (short i = 0; i < Longitude - 1; i++)
			{
				for (short j = 0; j < Latitude - 1; j++)
				{
					ib[index++] = (short)(Offset + i * Latitude + j);
					ib[index++] = (short)(Offset + (i + 1) * Latitude + j + 1);
					ib[index++] = (short)(Offset + (i + 1) * Latitude + j);

					ib[index++] = (short)(Offset + i * Latitude + j + 1);
					ib[index++] = (short)(Offset + (i + 1) * Latitude + j + 1);
					ib[index++] = (short)(Offset + i * Latitude + j);
				}
			}
		}

		#endregion

		#region GenerateMoon

		private void GenerateMoon()
		{
			quadVerts = new VertexPositionTexture[]
						{
							new VertexPositionTexture(
								new Vector3(1,-1,0),
								new Vector2(1,1)),
							new VertexPositionTexture(
								new Vector3(-1,-1,0),
								new Vector2(0,1)),
							new VertexPositionTexture(
								new Vector3(-1,1,0),
								new Vector2(0,0)),
							new VertexPositionTexture(
								new Vector3(1,1,0),
								new Vector2(1,0))
						};

			quadIb = new short[] { 0, 1, 2, 2, 3, 0 };
		}

		#endregion

		#region GetSunColor

		Vector4 GetSunColor(float fTheta, int nTurbidity)
		{
			float fBeta = 0.04608365822050f * nTurbidity - 0.04586025928522f;
			float fTauR, fTauA;
			float[] fTau = new float[3];

			float coseno = (float)Math.Cos((double)fTheta + Math.PI);
			double factor = (double)fTheta / Math.PI * 180.0;
			double jarl = Math.Pow(93.885 - factor, -1.253);
			float potencia = (float)jarl;
			float m = 1.0f / (coseno + 0.15f * potencia);

			int i;
			float[] fLambda = new float[3];
			fLambda[0] = parameters.WaveLengths.X;
			fLambda[1] = parameters.WaveLengths.Y;
			fLambda[2] = parameters.WaveLengths.Z;


			for (i = 0; i < 3; i++)
			{
				potencia = (float)Math.Pow((double)fLambda[i], 4.0);
				fTauR = (float)Math.Exp((double)(-m * 0.008735f * potencia));

				const float fAlpha = 1.3f;
				potencia = (float)Math.Pow((double)fLambda[i], (double)-fAlpha);
				if (m < 0.0f)
					fTau[i] = 0.0f;
				else
				{
					fTauA = (float)Math.Exp((double)(-m * fBeta * potencia));
					fTau[i] = fTauR * fTauA;
				}

			}

			Vector4 vAttenuation = new Vector4(fTau[0], fTau[1], fTau[2], 1.0f);
			return vAttenuation;
		}

		#endregion

		#region GeneratePermTex

		private void GeneratePermTex()
		{
			int[] perm = { 151,160,137,91,90,15,
			131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
			190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
			88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
			77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
			102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
			135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
			5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
			223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
			129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
			251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
			49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
			138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
			};

			int[] gradValues = { 1,1,0,    
				-1,1,0, 1,-1,0, 
				-1,-1,0, 1,0,1,
				-1,0,1, 1,0,-1,
				-1,0,-1, 0,1,1,
				0,-1,1, 0,1,-1,
				0,-1,-1, 1,1,0,
				0,-1,1, -1,1,0, 
				0,-1,-1
			};

			//EDIT
			permTex = new Texture2D(game.GraphicsDevice, 256, 256, true, SurfaceFormat.Color);

			byte[] pixels;
			pixels = new byte[256 * 256 * 4];
			for(int i = 0; i<256; i++)
			{
				for(int j = 0; j<256; j++) 
				{
				  int offset = (i*256+j)*4;
				  byte value = (byte)perm[(j + perm[i]) & 0xFF];
				  pixels[offset + 1] = (byte)(gradValues[value & 0x0F] * 64 + 64);
				  pixels[offset + 2] = (byte)(gradValues[value & 0x0F + 1] * 64 + 64);
				  pixels[offset + 3] = (byte)(gradValues[value & 0x0F + 2] * 64 + 64);
				  pixels[offset] = value;
				}
			}

			permTex.SetData<byte>(pixels);
		}

		#endregion

		#endregion

		#region Public Methods

		public void ApplyChanges()
		{
			this.UpdateMieRayleighTextures();
		}

		#endregion

	}
}