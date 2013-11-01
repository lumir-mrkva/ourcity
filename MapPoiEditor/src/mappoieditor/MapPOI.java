/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mappoieditor;

import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;


/**
 *
 * @author ptatousek
 */
public class MapPOI {    
    public static final int POI = 0;
    public static final int WAYPOINT = 1;
    public static final int START = 2;
    
    // souradnice v xml
    float x,y,z;
    int identifier;
    int level;
    int type;
    int start = 0; // 0 - flase, 1 - true
    
    // souradnice kam se vykresluje
    int realX;
    int realY; //z
    
    public void Draw (Graphics g) {
        Graphics2D g2 = (Graphics2D) g;
        if (this.type == MapPOI.WAYPOINT) {
            g2.setColor( Color.blue );  
        } else if (this.type == MapPOI.START) {
            g2.setColor( Color.red );             
        } else {
            g2.setColor( Color.black );             
        }
        g2.drawOval(this.realX, this.realY, 2, 2);
        g2.drawString("" + this.identifier, this.realX + 4, this.realY + 4);
    }        
    
    @Override
    public String toString() {
        if (this.type == MapPOI.POI || this.type == MapPOI.START) {
            return "    <poi identifier=\"" + this.identifier + "\" x=\"" + this.x + "\" y=\"" + this.y + "\" z=\"" + this.z + "\" start=\"" + this.start + "\"/>";
        }
        return "    <waypoint identifier=\"" + this.identifier + "\" x=\"" + this.x + "\" y=\"" + this.y + "\" z=\"" + this.z + "\" />";
    }
}