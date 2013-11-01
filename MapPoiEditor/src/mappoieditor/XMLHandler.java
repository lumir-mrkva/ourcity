/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mappoieditor;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;

/**
 *
 * @author ptatousek
 */
public class XMLHandler {
    
     public static void save(MapObject map, File file) {
         XMLHandler.writeHead(file);
         for (int i=0; i < map.pois.size(); i++) {
             XMLHandler.writeString(file, map.pois.get(i).toString() + "\n", true);
         }         
         for (int i=0; i < map.lines.size(); i++) {
             XMLHandler.writeString(file, map.lines.get(i).toString() + "\n", true);
         }                  
         for (int i=0; i < map.waypoints.size(); i++) {
             XMLHandler.writeString(file, map.waypoints.get(i).toString() + "\n", true);
         }                           
         XMLHandler.writeTail(file);
     }
    
     public static void writeHead(File file) {
         XMLHandler.writeString(file, "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n", false);
         XMLHandler.writeString(file, "<map>\n", true);
     }
    
     public static void writeTail(File file){
         XMLHandler.writeString(file, "</map>\n", true);
     }
     
     public static void writeString(File file, String s, boolean append) {
        try{
            FileWriter fstream = new FileWriter(file, append);
            BufferedWriter out = new BufferedWriter(fstream);
            out.write(s);
            out.close();
        }catch (Exception e){//Catch exception if any
            System.err.println("Error: " + e.getMessage());
        }         
     }
     
     public static MapObject load(File file) {
        MapObject map = new MapObject();
        try {
            DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
            DocumentBuilder db = dbf.newDocumentBuilder();
            Document doc = db.parse(file);
            doc.getDocumentElement().normalize();            

            // nacteni poi a vytvoreni objektu
            NodeList nodeLst = doc.getElementsByTagName("poi");
            for (int s = 0; s < nodeLst.getLength(); s++) {

                    Node fstNode = nodeLst.item(s);
                    if (fstNode.getNodeType() == Node.ELEMENT_NODE) {
                      Element poiElement = (Element) fstNode;
                      MapPOI mapPoi = new MapPOI();
                      mapPoi.identifier = Integer.valueOf(poiElement.getAttribute( "identifier" ));                      
                      if (poiElement.hasAttribute("start"))
                        mapPoi.start = Integer.valueOf(poiElement.getAttribute( "start" ));
                      mapPoi.x =  Float.valueOf(poiElement.getAttribute( "x" ));
                      mapPoi.y =  Float.valueOf(poiElement.getAttribute( "y" ));
                      mapPoi.z =  Float.valueOf(poiElement.getAttribute( "z" ));
                      mapPoi.realY = (int) map.getRealY((int) mapPoi.x);
                      mapPoi.realX = (int) map.getRealX((int) mapPoi.z);
                      mapPoi.type = MapPOI.POI;
                      map.pois.add(mapPoi);
                        System.out.println("Adding poi (x:" + mapPoi.x + ", y:" + mapPoi.y + ", z:"  + mapPoi.z + ") to x:"  + mapPoi.realX + ", y:" + mapPoi.realY);
                      if (mapPoi.identifier > map.maxPOIIdentifier) {
                          map.maxPOIIdentifier = mapPoi.identifier;
                      }                              
                    }
            }
            
            // nacteni cest a vytvoreni objektu
            nodeLst = doc.getElementsByTagName("line");
            for (int s = 0; s < nodeLst.getLength(); s++) {

                    Node fstNode = nodeLst.item(s);
                    if (fstNode.getNodeType() == Node.ELEMENT_NODE) {
                      Element lineElement = (Element) fstNode;
                      MapLine mapLine = new MapLine();
                      mapLine.from = Integer.valueOf(lineElement.getAttribute( "from" ));
                      mapLine.to = Integer.valueOf(lineElement.getAttribute( "to" ));
                      map.lines.add(mapLine);
                    }
            }
            
            // nacteni poi a vytvoreni objektu
            nodeLst = doc.getElementsByTagName("waipoint");
            for (int s = 0; s < nodeLst.getLength(); s++) {

                    Node fstNode = nodeLst.item(s);
                    if (fstNode.getNodeType() == Node.ELEMENT_NODE) {
                      Element poiElement = (Element) fstNode;
                      MapPOI mapPoi = new MapPOI();
                      mapPoi.identifier = Integer.valueOf(poiElement.getAttribute( "identifier" ));
                      mapPoi.x =  Float.valueOf(poiElement.getAttribute( "x" ));
                      mapPoi.y =  Float.valueOf(poiElement.getAttribute( "y" ));
                      mapPoi.z =  Float.valueOf(poiElement.getAttribute( "z" ));
                      mapPoi.realY = (int) map.getRealY((int) mapPoi.x);
                      mapPoi.realX = (int) map.getRealX((int) mapPoi.z);
                      mapPoi.type = MapPOI.WAYPOINT;
                      map.waypoints.add(mapPoi);
                    }
            }                   
        } catch (Exception e) {
            System.err.println("Error: " + e.getMessage());
        }
        return map;
    }
     
     
}