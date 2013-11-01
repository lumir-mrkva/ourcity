using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace OurCityEngine.Utils {
    /// <summary>
    /// Helper for generating random elements
    /// </summary>
    public class RandomHelper {
        private static RandomHelper instance;
        private System.Random random;

        private RandomHelper()
        {
            random = new System.Random((int)System.DateTime.Now.Ticks);

            // system color fetch
            // get the Type of Color for reflection purposes
            Type type = typeof(Color);

            // fetch information about all public static properties on Color
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

            // examine each of those properties
            foreach (PropertyInfo property in properties) {
                if (property.PropertyType == type) // it's a Color
            {
                    if (property.CanRead && !property.CanWrite) // it's read only
                {
                        // get the actual get method from the property
                        MethodInfo getMethod = property.GetGetMethod();

                        // if you want the name, here it is
                        string colorName = property.Name;

                        // call it via reflection to get the Color inside it
                        Color propertyValue = (Color)getMethod.Invoke(null, null);

                        // a quick diagnostic, delete this line:
                        Console.WriteLine("{0}: {1}", colorName, propertyValue);

                        m_List.Add(propertyValue);
                    }
                }
            }
        }

        public static RandomHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new RandomHelper();
                    
                }
                
                return instance;
            }
        }

        /// <summary>
        /// Gets random color
        /// </summary>
        public Color Color {
            get {
                Vector3 color = new Vector3(random.Next(255), random.Next(255), random.Next(255));
                color /= 255.0f;
                return new Color(color);
            }
        }

        /// <summary>
        /// Get random standart color (system color)
        /// </summary>
        public Color StandardColor {
            get {
                return m_List[Random.Next(0, m_List.Count - 1)];
            }
        }
        List<Color> m_List = new List<Color>();

        /// <summary>
        /// Return actual random instance
        /// </summary>
        public Random Random {
            get {
                return random;
            }
        }
    }
}
