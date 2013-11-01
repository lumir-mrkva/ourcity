using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace OurCityEngine.Utils
{
    /// <summary>
    /// Logger 
    /// </summary>
    public class Logger
    {
        private string filename;
        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Logger("log.txt");
                    instance.Log("log started");
                }

                return instance;
            }
        }
        
        #region Constructor
        private static Logger instance;
        private Logger(string filename)
        {
            this.filename = filename;
            this.active = true;
        }
        #endregion


        /// <summary>
        /// Is log enabled?
        /// </summary>
        public bool Enabled
        {
            get { return active; }
            set { active = value; }
        }
        private bool active;
 
        /// <summary>
        /// Write text to a log
        /// </summary>
        /// <param name="text">Text</param>
        public void Log(string text)
        {
            if (!active) return;
            text = System.DateTime.Now.ToLongTimeString() + " : " + text;
            Output(text);
        }
 
        private void Output(string text)
        {
            try
            {
                StreamWriter textOut = new StreamWriter(new FileStream("log.txt", FileMode.Append, FileAccess.Write));
                textOut.WriteLine(text);
                textOut.Close();
            }
            catch (System.Exception e)
            {
                string error = e.Message;
            }
        }
    }
}
