using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Hud {
    /// <summary>
    /// Simple message in hud
    /// </summary>
    public class Message {
        string text;
        Color color;

        /// <summary>
        /// Constructs message
        /// </summary>
        /// <param name="text">Message text</param>
        /// <param name="timeout">Message timeout</param>
        /// <param name="color">Color of the message</param>
        /// <param name="persistant">Persistency of the message</param>
        public Message(string text, string objective, int timeout, Color color, bool persistant) {
            this.text = text;
            this.timeout = timeout;
            this.color = color;
            this.Persistant = persistant;
            if (persistant && objective == null) {
                Objective = text;
            } else {
                Objective = objective;
            }
        }

        public Message(string text)
            : this(text, 3, Color.White, false) { }

        public Message(string text,  int timeout, Color color, bool persistant) 
            : this(text, null, timeout, color, persistant) { }

        public Message(string text, string objective)
            : this(text, objective, 3, Color.White, true) { }
        
        public void Set(string text) {
            this.text = text;
        }

        public string Get() {
            return text;
        }

        public string ToString() {
            return text;
        }

        /// <summary>
        /// Is message persistant?
        /// </summary>
        public bool Persistant { get; set; }


        /// <summary>
        /// Objective in persistant message
        /// </summary>
        public string Objective { get; set; }


        /// <summary>
        /// Gets / sets color of the message
        /// </summary>
        public Color Color { get { return color; } set { color = value; } }

        /// <summary>
        /// Message timeout in seconds
        /// 
        /// default: 5
        /// </summary>
        public int Timeout { get { return timeout; } set{ timeout = value; }}
        int timeout = 5;

        /// <summary>
        /// When message was shown
        /// </summary>
        public DateTime ShowStart { get; internal set; }

        private bool timerStarted = false;
        private float p;
        private float p_2;

        /// <summary>
        /// Gets message and starts timer
        /// </summary>
        /// <returns></returns>
        public string Show() {
            if (!timerStarted) {
                ShowStart = DateTime.Now;
                timerStarted = true;
            }
            return text;
        }

        /// <summary>
        /// Has message been shown for enought time
        /// </summary>
        /// <returns></returns>
        public bool Expired() {
            return (DateTime.Now - ShowStart).TotalSeconds > timeout;
        }
    }

}
