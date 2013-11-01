using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using OurCityEngine.Screens;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Cameras;
using OurCityEngine.Waypoints;
using OurCityEngine.Hud;
using OurCityEngine.Utils;

namespace OurCity.Screens
{
    /// <summary>
    /// HUD
    /// </summary>
    public class HudOverlay : DrawableGameComponent, IHud
    {

        #region Fields
        ContentManager content;
        OurCityGame game;
        SpriteFont arialFont;
        GraphicsDevice graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D map;
        Texture2D arrow;
        Texture2D dot;
        ScreenManager ScreenManager {get;set;}

        int bbWidth, bbHeight;
        #endregion
        
        #region Constructor
        public HudOverlay(ScreenManager sm)
            : base(sm.Game)
        {
            content = new ContentManager(sm.Game.Services, "Content");
            game = (OurCityGame)sm.Game;
            ScreenManager = sm;
            graphics = ScreenManager.GraphicsDevice;
            spriteBatch = ScreenManager.SpriteBatch;
            font = ScreenManager.Font;
            MinimapSize = 200;
            NotifyPsycho = false;
        }
        #endregion

        #region Load
        protected override void LoadContent()
        {
            arialFont = content.Load<SpriteFont>("Fonts/ArialL");
            map = content.Load<Texture2D>("Map/map");
            arrow = content.Load<Texture2D>("Hud/arrow");
            dot = content.Load<Texture2D>("Hud/dot");
            
            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
            
        }

        protected override void UnloadContent()
        {
                content.Unload();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            if (!Enabled) return;
            bbWidth = graphics.Viewport.Width;
            bbHeight = graphics.Viewport.Height;
            UpdateNotify();

        }

        public override void Draw(GameTime gameTime)
        {
            if (!Enabled) return;

            spriteBatch.Begin();
            DrawMainMessageBox();
            DrawNotify();
            DrawObjectives();
            //DrawCarSpeedIndicator();
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            DrawMinimap();
            spriteBatch.End();
        }


        #region Main message
        private List<string> mainMessages = new List<string>();

        /// <summary>
        /// Display message at center of the hud
        /// </summary>
        /// <param name="message">Message</param>
        public void AddMainMessage(string message) {
            if (message != null)
                mainMessages.Add(message);
        }

        private void DrawMainMessageBox()
        {
            float row_height = font.MeasureString("ROW").Y;
            Vector2 titlePosition = new Vector2(bbWidth / 2, 80);
            foreach (string m in mainMessages) {
                
                Vector2 titleOrigin = font.MeasureString(m) / 2;
                Color titleColor = Color.Black;
                float titleScale = 1.25f;

                spriteBatch.DrawString(font, m, titlePosition + 2 * Vector2.One, Color.Black, 0,
                                       titleOrigin, titleScale, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, m, titlePosition, Color.White, 0,
                                       titleOrigin, titleScale, SpriteEffects.None, 0);
                
                titlePosition += new Vector2(0, row_height);
            }
            mainMessages.Clear();
        }
        #endregion

        #region Notify
        /// <summary>
        /// Notify player with message
        /// </summary>
        /// <param name="message">message</param>
        public void Notify(Message message) {
            if (message != null)
                notifyQueue.Enqueue(message);
        }

        Queue<Message> notifyQueue = new Queue<Message>();
        Message actualNotify;
        bool activeNotify = false;
        bool drawNotify = false;
        public bool NotifyPsycho { get; set; }
        private void UpdateNotify() {
            if (activeNotify) { // then draw message
                if (actualNotify.Expired()) { // then try load new message
                    //if persistant add to objectives
                    if (actualNotify.Persistant) {
                        AddObjective(actualNotify);
                    }

                    if (notifyQueue.Count > 0) {
                        actualNotify = notifyQueue.Dequeue();
                        actualNotify.Show();
                        drawNotify = true;
                    }  else { // no new message, disable
                        activeNotify = false;
                    }
                    return;
                } else { // enable draw of actual message
                    drawNotify = true;
                    return;
                }
            }
            if (notifyQueue.Count > 0 && !activeNotify) {
                actualNotify = notifyQueue.Dequeue();
                actualNotify.Show();
                activeNotify = true;
            }
        }

        private void DrawNotify() {
            if (!drawNotify) return;
            string text = actualNotify.Show();
            Vector2 loc = new Vector2(bbWidth / 2, bbHeight / 2);
            spriteBatch.DrawString(font, text, loc + 2*Vector2.One, Color.Black, 0, font.MeasureString(text) / 2, 1, SpriteEffects.None, 0);
            if (NotifyPsycho)
                spriteBatch.DrawString(font, text, loc, RandomHelper.Instance.Color, 0, font.MeasureString(text)/2, 1, SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(font, text, loc, actualNotify.Color, 0, font.MeasureString(text) / 2, 1, SpriteEffects.None, 0);
            drawNotify = false;
        }
        #endregion

        #region Objectives
        /// <summary>
        /// Add objective to the hud screen
        /// </summary>
        /// <param name="objective">Persistant message</param>
        public void AddObjective(Message objective) {
            if (objectives.Contains(objective)) return;
            objectives.Clear(); //TODO temporary solution, please remove
            objectives.Add(objective);
        }

        /// <summary>
        /// Remove objective
        /// </summary>
        public void RemoveObjective(Message objective) {
            objectives.Remove(objective);
        }

        public void ClearObjectives() {
            objectives.Clear();
        }

        /// <summary>
        /// Set objective only 1 objective
        /// </summary>
        /// <param name="objective"></param>
        public void SetObjective(Message objective) {
            objectives.Clear();
            objectives.Add(objective);
        }

        List<Message> objectives = new List<Message>();
        Vector2 objectivesLocation = new Vector2(50, 50);

        private void DrawObjectives() {
            if (objectives.Count < 1) return;

            Vector2 location = objectivesLocation;
            Vector2 fsize = font.MeasureString("text");
            
            // draw title
            string title = "Objective:";
            if (objectives.Count > 1) title = "Objectives:";
            spriteBatch.DrawString(font, title, objectivesLocation + 2 * Vector2.One, Color.Black, 0, Vector2.One, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, title, objectivesLocation, Color.White, 0, Vector2.One, 1, SpriteEffects.None, 0);


            location += new Vector2(5, fsize.Y);
            fsize *= 0.5f;
            foreach (Message o in objectives) {
                //check persistency
                if (!o.Persistant) {
                    objectives.Remove(o);
                    continue;
                }

                //draw objective
                spriteBatch.DrawString(font, o.Objective, location + Vector2.One, Color.Black, 0, Vector2.One, 0.7f, SpriteEffects.None, 0);
                spriteBatch.DrawString(font, o.Objective, location, o.Color, 0, Vector2.One, 0.7f, SpriteEffects.None, 0);

                location += new Vector2(0, fsize.Y);
            }
        }
        #endregion

        #region Car speed indicator
        public float CarVelocity { get; set; }

        private void DrawCarSpeedIndicator()
        {
            String speed = CarVelocity.ToString();
            Vector2 sSize = arialFont.MeasureString("XXX");
            Vector2 location = new Vector2(bbWidth - sSize.X, bbHeight - sSize.Y);
            spriteBatch.DrawString(arialFont, speed, location+ 3*Vector2.One, 
                Color.Black,0,Vector2.One,1,SpriteEffects.None,0);
            spriteBatch.DrawString(arialFont, speed, location,
                new Color(1-1/CarVelocity, 0,0), 0, Vector2.One, 1, SpriteEffects.None, 0);
        }
        #endregion

        #region Minimap
        public Vector3 PlayerLocation { get; set; }
        public Vector2 MinimapPlayerLocation { get; set; }
        public Vector2 MinimapScreenLocation { get; set; }
        public float PlayerRotation { get; set; }
        public int MinimapSize { get; set; }

        #region MinimapPois
        
        private List<MinimapPoi> minimapPois = new List<MinimapPoi>();

        /// <summary>
        /// Add Poi to the minimap
        /// </summary>
        /// <param name="position">World coordinates</param>
        /// <param name="color">Color</param>
        /// <param name="shape">Shape of the Poi</param>
        /// <returns>Created Poi</returns>
        public MinimapPoi AddMinimapPoi(Vector3 position, Color color, PoiShape shape = PoiShape.Square) {
            MinimapPoi poi = new MinimapPoi(GetMinimapCoordinates(position), color, shape);
            minimapPois.Add(poi);
            return poi;
        }


        /// <summary>
        /// Draw Poi on minimap
        /// </summary>
        /// <param name="position"></param>
        public void DrawMinimapPoi(MinimapPoi poi) {
            Texture2D shape;
            switch (poi.shape) {
                case PoiShape.Square: shape = dot; break;
                default: shape = dot; break;
            }

            int half = 5;

            Vector2 loc = MinimapScreenLocation - MinimapPlayerLocation + poi.position + new Vector2(MinimapSize / 2 - half, MinimapSize / 2 - half);
            
            // pois too far stay at border of map
            if (loc.X > MinimapScreenLocation.X + MinimapSize - half) loc.X = MinimapScreenLocation.X + MinimapSize - half;
            if (loc.X < MinimapScreenLocation.X - half) loc.X = MinimapScreenLocation.X - half;
            if (loc.Y > MinimapScreenLocation.Y + MinimapSize - half) loc.Y = MinimapScreenLocation.Y + MinimapSize - half;
            if (loc.Y < MinimapScreenLocation.Y - half) loc.Y = MinimapScreenLocation.Y - half;

            spriteBatch.Draw(shape, loc , poi.color);
        }

        /// <summary>
        /// Draw all Pois on minimap
        /// </summary>
        /// <param name="position"></param>
        private void DrawMinimapPois() {
            foreach (MinimapPoi poi in minimapPois) {
                DrawMinimapPoi(poi);
            }

            minimapPois.Clear();
        }
        #endregion


        /// <summary>
        /// Draw minimap
        /// </summary>
        private void DrawMinimap()
        {
            MinimapPlayerLocation = GetMinimapCoordinates(PlayerLocation);
            MinimapScreenLocation = GetMinimapScreenLocation();

            // draw map
            spriteBatch.Draw(map, MinimapScreenLocation, PlayerCenteredMapRectangle(1), Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);

            /// draw checkpoint / pois
//             Vector3 checkpoint = new Vector3(0, 0, 0); // map origin
//             DrawMinimapPoi(new MinimapPoi(GetMinimapCoordinates(checkpoint), Color.Gold));
            DrawMinimapPois();

            // draw rotated arrow
            spriteBatch.Draw(arrow, MinimapScreenLocation +new Vector2(MinimapSize / 2, MinimapSize / 2), null, Color.White, PlayerRotation, new Vector2(9, 8.5f), 1.0f, SpriteEffects.None, 0f);

            
        }

        /// <summary>
        /// Convert world coordinates to minimap coordinates
        /// </summary>
        /// <param name="coords">World coordinates</param>
        /// <returns>Minimap coordinates</returns>
        private Vector2 GetMinimapCoordinates(Vector3 coords) {
            Vector2 size = new Vector2(MinimapSize/2,MinimapSize/2); //for centering map to player location, this should be elsewhere (TODO)
            Vector2 mapCenter = new Vector2(129 + 310 / minToMap, 117 + 880 / minToMap);
            return ((new Vector2(coords.X, coords.Z) / minToMap) + mapCenter) - size;
        }

        /// <summary>
        /// Minimap to map scale
        /// </summary>
        float minToMap = 2.09f;

        /// <summary>
        /// Get world coordinates from minimap coordinates
        /// <remarks>
        /// This isnt inverse function to GetMinimapCoordinates!
        /// </remarks>
        /// </summary>
        /// <param name="coords">World coordinates</param>
        /// <returns>Minimap coordinates</returns>
        public Vector2 GetWorldCoordinates(Vector2 coords) {
            Vector2 mapCenter = new Vector2(129 + 310 / minToMap, 117 + 880 / minToMap);
            return ((new Vector2(coords.X, coords.Y)  - mapCenter) * minToMap);
        }

        /// <summary>
        /// Determines top left corner of minimap
        /// </summary>
        /// <returns></returns>
        private Vector2 GetMinimapScreenLocation() {
            int margin = 50;
            return new Vector2(margin, bbHeight - MinimapSize - margin);
        }

        private Rectangle PlayerCenteredMapRectangle(int scale)
        {
            Vector2 playerLoc = MinimapPlayerLocation;
            Rectangle rec = new Rectangle((int)playerLoc.X, (int)playerLoc.Y, MinimapSize, MinimapSize);
            return rec;
        }
        #endregion

        private void GraphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            bbWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            bbHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;
        }


        
    }


}

