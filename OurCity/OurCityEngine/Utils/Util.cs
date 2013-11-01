using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OurCityEngine.Utils {
    /// <summary>
    /// Misc. utils
    /// </summary>
    public class Util {
        /// <summary>
        /// Converts Vector3 to angle in radians
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Vector3ToRadians(Vector3 vector) {
            return (float)(Math.Atan2(vector.Z, vector.X));
        }
    }
}
