/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package mappoieditor;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.geom.Line2D;

/**
 *
 * @author ptatousek
 */
public class MapLine {
    MapPOI fromPOI;
    MapPOI toPOI;
    int from;
    int to;
    
    public void Draw(Graphics g) {
        Graphics2D g2 = (Graphics2D) g;
        
        float dash[] = {20,10}; 
        BasicStroke bs = new BasicStroke(5, BasicStroke.CAP_ROUND, BasicStroke.JOIN_ROUND);         
        g2.setStroke(bs); 
        
        if (this.fromPOI != null && this.toPOI != null ){       
            float fromX = this.fromPOI.realX;
            float fromY = this.fromPOI.realY;        
            float toX = this.toPOI.realX;
            float toY = this.toPOI.realY;               
            Line2D line = new Line2D.Float(fromX, fromY, toX, toY);
            g.setColor( Color.black );
            g2.draw(line);
        } else {
            System.out.println("Error drawing line.");
        }
    }    
    
    @Override
    public String toString() {
        return "    <line from=\"" + this.from + "\" to=\"" + this.to + "\" />";
    }
}
