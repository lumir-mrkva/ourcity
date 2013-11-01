using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OurCityEngine.PhysicObjects;
using OurCityEngine.Utils;

namespace OurCityEngine.AI
{

    /// <summary>
    /// Map object
    /// </summary>
    class MapObject
    {
        // waypoints
        public List<Vector3> Waypoints { get { return waypoints; } }

        // xml file with map definitions
        String mapFile = "map.xml";

        // internal waypoints list
        List<Vector3> waypoints = new List<Vector3>();
        // list of all pois on the map
        List<MapPOI> pois = new List<MapPOI>();
        // simply lines
        List<IntLine> intLines = new List<IntLine>();
        // map lines (between two pois)
        List<MapLine> lines = new List<MapLine>();
        // list of startign lines (where cars can be spawned)
        List<MapLine> startLines = new List<MapLine>();

        // iterator for starting lines
        int startlineInterator = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        public MapObject() 
        {
            // load map
            this.LoadXML();
            // build map
            this.BuildMapLines();
            // populate lines
            this.PopulateNextLines();
        }


        /// <summary>
        /// Load and parse XML file
        /// </summary>
        public void LoadXML() 
        {
            MapXMLReader xmlReader = new MapXMLReader();
            xmlReader.xmlFile = this.mapFile;
            xmlReader.ReadXML();
            this.pois = xmlReader.pois;
            this.intLines = xmlReader.lines;
            this.waypoints = xmlReader.waypoints;
        }


        /// <summary>
        /// Build map lines - connect two POIs
        /// </summary>
        public void BuildMapLines()
        {
            foreach (IntLine line in this.intLines)
            {
                MapPOI from = null;
                MapPOI to = null;

                foreach (MapPOI poi in this.pois)
                {         
                    if (line.from == poi.identifier) {
                        from = poi;
                    }
                    else if (line.to == poi.identifier) {
                        to = poi;
                    }
                }
                try
                {
                    MapLine newLine = new MapLine(from, to);
                    this.lines.Add(new MapLine(from, to));

                    if (from.start == MapPOI.IS_START_POI)
                    {
                        this.startLines.Add(newLine);
                    }
                }
                catch (Exception e) {
                    throw new Exception("Error buliding map lines: " + e);
                }
            }
        }


        /// <summary>
        /// Populate each POI's list with next line to follow
        /// </summary>
        public void PopulateNextLines() 
        {
            try
            {
                foreach (MapLine lineTo in this.lines)
                {
                    foreach (MapLine lineFrom in this.lines)
                    {
                        if (lineTo.to.identifier == lineFrom.from.identifier)
                        {
                            lineTo.to.lines.Add(lineFrom);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error populating next lines: " + e);
            }
        }


        /// <summary>
        /// return next starting line (where cars can be spawned)
        /// </summary>        
        /// <returns>Next start line - MapLine</returns>
        public MapLine GetNextStartLine() {

            if (this.startLines.Count > 0)
            {
                return this.startLines[(this.startlineInterator++) % this.startLines.Count];
            }
            else if (this.lines.Count > 0)
            {
                return this.lines[(this.startlineInterator++) % this.lines.Count];
            }

            return null;
        }
    }    
}
