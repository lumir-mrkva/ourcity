using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using OurCityEngine.Utils;

namespace OurCityEngine.AI
{

    /// <summary>
    /// Map line (between two POIS)
    /// </summary>
    public class MapLine
    {
        // maximum of total pois (from, to and counted)
        const int MAX_POI = 4;

        // array of counted pois
        public MapPOI[] mapPois = new MapPOI[MAX_POI];

        // start POI
        public MapPOI from { get; internal set; }
        // finish POI
        public MapPOI to { get; internal set; }

        /// <summary>
        /// Map line constructor
        /// </summary>
        public MapLine(MapPOI from, MapPOI to)
        {
            this.from = from;
            this.from.lineInternal = false;
            this.to = to;
            this.to.lineInternal = false;
            //-- 
            this.from.routeIdentifier = 0;
            this.mapPois[0] = this.from;
            this.CalculateAdditionalPoints();
        }


        /// <summary>
        /// Calculate additional pois (between from and to)
        /// </summary>
        public void CalculateAdditionalPoints()
        {
            float count = 1f / MAX_POI;
            for (int i = 1; i < MAX_POI - 1; i++)
            {
                this.mapPois[i] = new MapPOI();
                this.mapPois[i].location = Vector3.Lerp(this.from.location, this.to.location, count * i);
                this.mapPois[i].lineInternal = true;
                this.mapPois[i].identifier = -1;
                this.mapPois[i].routeIdentifier = i;
            }
            this.mapPois[MAX_POI-1] = this.to;
            this.mapPois[MAX_POI-1].lineInternal = false;
            this.mapPois[MAX_POI-1].routeIdentifier = MAX_POI - 1;         
        }


        /// <summary>
        /// Return next poi on the line
        /// </summary>
        /// <param name="previous">Previous POI</param>
        /// <returns>Next POI</returns>
        public MapPOI GetNextPOI(MapPOI previous)
        {
            Logger.Instance.Log("prev id: " + previous.routeIdentifier);
            if ((previous.identifier == to.identifier) && previous.identifier > -1)
            {
                Logger.Instance.Log("car finish this line");
                return null; // car finished this line
            }
            else
            {
                if (previous.routeIdentifier + 1 < MAX_POI)
                {
                    return this.mapPois[previous.routeIdentifier + 1]; // return next part-line poi
                }
                else
                {
                    return this.to;
                }
            }
        }
    }

}
