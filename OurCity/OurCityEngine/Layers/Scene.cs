using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OurCityEngine.Layers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Layers {
    public class Scene : Layer {
        public ContentManager Content { get; set; }

        public Scene(Game game) : base(game) {
            Content = new ContentManager(game.Services, "Content");
        }

        public void UnloadContent() {
            Content.Unload();
        }
    }
}
