/*  
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mappoieditor;

import java.awt.Graphics;
import java.util.ArrayList;
import java.util.List;

/**
 *
 * @author ptatousek
 */
public class MapObject {
    List<MapPOI> pois = new ArrayList();
    List<MapLine> lines = new ArrayList();
    List<MapPOI> waypoints = new ArrayList();
    int maxPOIIdentifier = 0;
    
    float scale = 1.045f; // original
    
    float zeroLevelY = -20;
    float firstLevelY = 0;
    float secondLevelY = 20;        
       
    float mapImgWidth = 2206;
    float mapImgHeight = 1192;    
    
    float mapCenterY = 556;
    float mapCenterX = 1064;
   
    
    float correctionX = 0;
    float correctionY = 0;
    
    int selectedLevel = 1;
    
    
    public void redraw(Graphics g) {
         for (int i=0; i < this.pois.size(); i++) {
             this.pois.get(i).Draw(g);
         }         
         for (int i=0; i < this.lines.size(); i++) {
             this.lines.get(i).Draw(g);
         }
         for (int i=0; i < this.waypoints.size(); i++) {
             this.waypoints.get(i).Draw(g);
         }                                   
    }    
    
    public void build() {
        boolean from;
        boolean to;
         for (int i=0; i < this.lines.size(); i++) {
             from = false;
             to = false;
             if (this.lines.get(i).from == this.lines.get(i).to) {
                 this.lines.remove(i);
             } else {
                 for (int j=0; j < this.pois.size(); j++) {
                     if (this.lines.get(i).from == this.pois.get(j).identifier) {
                         this.lines.get(i).fromPOI = this.pois.get(j);
                         from = true;
                     }
                     if (this.lines.get(i).to == this.pois.get(j).identifier) {
                         this.lines.get(i).toPOI = this.pois.get(j);
                         to = true;
                     }                 
                 }             
                 if (!from || !to) {
                     this.lines.remove(i);
                 }
             }
         }
    }
    
    public boolean deletePOI(int id) {
        for (int i=0; i < this.pois.size(); i++) {
             if (this.pois.get(i).identifier == id) {
                this.pois.remove(i);
                return true;
            }
        }      
        return false;
    }    
    
    public boolean makePOIStart(int id) {
        for (int i=0; i < this.pois.size(); i++) {
             if (this.pois.get(i).identifier == id) {
                this.pois.get(i).start = 1;
                this.pois.get(i).type = MapPOI.START;
                return true;
            }
        }      
        return false;        
    }

    public boolean makeWaypoint(int id) {
        for (int i=0; i < this.pois.size(); i++) {
             if (this.pois.get(i).identifier == id) {
                this.pois.get(i).type = MapPOI.WAYPOINT;
                return true;
            }
        }      
        return false;        
    }       
    
    public void addPOI(float x, float y, float z, int realX, int realY) {
        MapPOI newPOI = new MapPOI();
        this.maxPOIIdentifier++;
        newPOI.identifier = this.maxPOIIdentifier;
        newPOI.x = x;
        newPOI.y = y;
        newPOI.z = z;
        newPOI.realY = realY;
        newPOI.realX = realX;
        this.pois.add(newPOI);
    }
    
    public boolean connectPOIs(int from, int to) {
        MapLine newLine = new MapLine();
        newLine.from = from;
        newLine.to = to;
        boolean fromFound = false;
        boolean toFound = false;
        for (int j=0; j < this.pois.size(); j++) {
         if (newLine.from == this.pois.get(j).identifier) {
             newLine.fromPOI = this.pois.get(j);
             fromFound = true;
         }
         if (newLine.to == this.pois.get(j).identifier) {
             newLine.toPOI = this.pois.get(j);
             toFound = true;
         }                 
        }             
        if (!fromFound || !toFound) {
            return false;
        }        
        this.lines.add(newLine);
        return true;
    }
    
    public float getRealX (int z) {
        return (z/this.scale) + this.mapCenterX;
    }
    
    public float getRealY (int x) {        
        return (x/this.scale) + this.mapCenterY;
    }

    public float getGameZ (int x) {
        return (x  - this.mapCenterX) * this.scale;
    }            
    
    public float getGameX (int y) {
        return (y  - this.mapCenterY) * this.scale;
    }
    
    public float getGameY () {
        if (this.selectedLevel == 0) {
            return zeroLevelY;
        }
        if (this.selectedLevel == 1) {
            return firstLevelY;
        }
        if (this.selectedLevel == 2) {
            return secondLevelY;
        }
        return 0;
    }    
}
