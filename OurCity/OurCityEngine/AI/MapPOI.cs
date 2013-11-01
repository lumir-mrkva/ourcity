using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCityEngine.AI
{

    /// <summary>
    /// Map POI
    /// </summary>
    public class MapPOI
    {
        // yes, this is starting poi
        public const int IS_START_POI = 1;
        // no, this is not starting poi
        public const int NOT_START_POI = 0;

        // lines that can be followed from this poi
        public List<MapLine> lines = new List<MapLine>();

        // location (Vector3)
        public Vector3 location { get; internal set; }
        // is this starting poi? (can cars be spawned here?)
        public int start { get; internal set; }
        // identifier of this poi
        public int identifier { get; internal set; }
        // internal identifier for MapLines's internal pois
        public int routeIdentifier { get; internal set; }
        // is this MapLine's internal line?
        public bool lineInternal { get; internal set; }  


        /// <summary>
        /// Constructor
        /// </summary>        
        public MapPOI()
        {
            this.identifier = -1;
            this.start = 0;
            this.lineInternal = false;
        }
    }
}
