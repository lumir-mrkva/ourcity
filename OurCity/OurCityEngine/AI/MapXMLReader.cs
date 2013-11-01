using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Xna.Framework;
using OurCityEngine.Utils;
using System.Globalization;

namespace OurCityEngine.AI
{
    /// <summary>
    /// Add Poi to the minimap
    /// </summary>
    /// <param name="position">World coordinates</param>
    /// <param name="color">Color</param>
    /// <param name="shape">Shape of the Poi</param>
    /// <returns>Created Poi</returns>
    class MapXMLReader
    {
        // XML document
        XmlDocument doc;

        // list of loaded pois
        public List<MapPOI> pois = new List<MapPOI>();
        // list of loaded lines
        public List<IntLine> lines = new List<IntLine>();
        // list of loaded waypoints
        public List<Vector3> waypoints = new List<Vector3>();

        /// <summary>
        /// Constructor
        /// </summary>
        public MapXMLReader()
        {   
        }

        /// <summary>
        /// Main fuction for reading and parsing XML
        /// </summary>
        public void ReadXML () {
            try {
                doc = new XmlDocument();
                doc.Load(xmlFile);
            } catch (Exception e){
                Logger.Instance.Log("error while opening file " + xmlFile + ":" + e);
                throw new Exception("error while opening file " + xmlFile + ":" + e);
            }

            // XML root element
            XmlNode lRootElem = doc.DocumentElement;

            foreach (XmlNode lNode in lRootElem)
            {
                // parsing poi
                if (lNode.Name.Equals("poi"))
                {                    
                    try {
                    MapPOI newPOI = new MapPOI();
                    newPOI.identifier = Convert.ToInt32(lNode.Attributes["identifier"].Value);
                    newPOI.start = Convert.ToInt32(lNode.Attributes["start"].Value);
                                            

                    newPOI.location = new Vector3(
                        GetFloatFromStrig(lNode.Attributes["x"].Value), 
                        GetFloatFromStrig(lNode.Attributes["y"].Value), 
                        GetFloatFromStrig(lNode.Attributes["z"].Value)
                        );
                    this.pois.Add(newPOI);

                    } catch (Exception e){
                        Logger.Instance.Log("Error while parsing pois: " + e);
                        throw new Exception("Error while parsing pois: " + e);
                    }                    
                }
                // parsing line
                if (lNode.Name.Equals("line"))
                {                    
                    try {
                        IntLine newIntLine = new IntLine(Convert.ToInt32(lNode.Attributes["from"].Value),
                                                    Convert.ToInt32(lNode.Attributes["to"].Value));
                        this.lines.Add(newIntLine);
                    } catch (Exception e){
                        Logger.Instance.Log("Error while parsing lines: " + e);
                        throw new Exception("Error while parsing lines: " + e);
                    }
                }

                // parsing waypoint
                if (lNode.Name.Equals("waypoint"))
                {
                    try
                    {
                        Vector3 newWaypoint = new Vector3(
                            GetFloatFromStrig(lNode.Attributes["x"].Value),
                            GetFloatFromStrig(lNode.Attributes["y"].Value),
                            GetFloatFromStrig(lNode.Attributes["z"].Value)
                            );
                        this.waypoints.Add(newWaypoint);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Log("Error while parsing waypoints: " + e);
                        throw new Exception("Error while parsing waypoints: " + e);
                    }                    
                }
            }
        }

        /// <summary>
        /// Convert string to float
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Float value</returns>
        private float GetFloatFromStrig (String input) {
            IFormatProvider format = CultureInfo.InvariantCulture.NumberFormat;
            float x = 0f;
            try
            {
                x = Single.Parse(input, format);
            }
            catch (FormatException) {
                Logger.Instance.Log("Error parsing number " + input + " from xml");
                throw new Exception("Error parsing number " + input + " from xml");
            }
            return x;
        }

        public String xmlFile { get; internal set; }
    }



    /// <summary>
    /// Int line
    /// </summary>
    class IntLine
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="from">starting poi</param>
        /// <param name="to">finish poi</param>
        public IntLine(int from, int to)
        {
            this.from = from;
            this.to = to;
        }
        public int from { get; internal set; }
        public int to { get; internal set; }
    }

}
