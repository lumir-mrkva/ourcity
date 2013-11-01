using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Hud {
    
    /// <summary>
    /// Point of interest on minimap
    /// </summary>
    public class MinimapPoi {
        public Color color;
        public Vector2 position;
        public PoiShape shape;

        public MinimapPoi(Vector2 position, Color color, PoiShape shape) {
            this.position = position;
            this.color = color;
            this.shape = shape;
        }

        public MinimapPoi(Vector2 position, Color color)
            : this(position, color, PoiShape.Square) {

        }
    }

    public enum PoiShape {
        Square
    }
}
