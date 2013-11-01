#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

#if WINDOWS_PHONE

namespace JigLibX.Utils
{
    /// <summary>Unsafe helper methods for JigLibX</summary>
    public sealed class JiggleUnsafe
    {
        /// <summary>Retrieves a vector component by its index</summary>
        /// <param name="vec">Vector from which a component will be retrieved</param>
        /// <param name="index">Index of the component to retrieve</param>
        /// <returns>The requested component of the input vector</returns>
        public static float Get(ref Vector3 vec, int index)
        {
            switch(index)
            {
                case 0: { return vec.X; }
                case 1: { return vec.Y; }
                case 2: { return vec.Z; }
                default: throw new ArgumentException("Bad component index", "index");
            }
        }

        /// <summary>Retrieves a vector component by its index</summary>
        /// <param name="vec">Vector from which a component will be retrieved</param>
        /// <param name="index">Index of the component to retrieve</param>
        /// <returns>The requested component of the input vector</returns>
        public static float Get(Vector3 vec, int index)
        {
            return Get(ref vec, index);
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <returns>The requested row of the matrix as a Vector3</returns>
        public static Vector3 Get(Matrix mat, int index)
        {
            return Get(ref mat, index);
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <returns>The requested row of the matrix as a Vector3</returns>
        public static Vector3 Get(ref Matrix mat, int index)
        {
            switch(index)
            {
                case 0: { return new Vector3(mat.M11, mat.M12, mat.M13); }
                case 1: { return new Vector3(mat.M21, mat.M22, mat.M23); }
                case 2: { return new Vector3(mat.M31, mat.M32, mat.M33); }
                case 3: { return new Vector3(mat.M41, mat.M42, mat.M43); }
                default: { throw new ArgumentException("Bad row index", "index"); }
            }
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <param name="vec">Vector into which the row will be copied</param>
        public static void Get(ref Matrix mat, int index, out Vector3 vec)
        {
          switch (index) {
            case 0: { vec = new Vector3(mat.M11, mat.M12, mat.M13); break; }
            case 1: { vec = new Vector3(mat.M21, mat.M22, mat.M23); break; }
            case 2: { vec = new Vector3(mat.M31, mat.M32, mat.M33); break; }
            case 3: { vec = new Vector3(mat.M41, mat.M42, mat.M43); break; }
            default: { throw new ArgumentException("Bad row index", "index"); }
          }
        }
    }
}

#endif // WINDOWS_PHONE