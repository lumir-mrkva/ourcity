#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

#if !WINDOWS_PHONE

namespace JigLibX.Utils
{
    /// <summary>Unsafe helper methods for JigLibX</summary>
    public sealed class JiggleUnsafe
    {
        /// <summary>Retrieves a vector component by its index</summary>
        /// <param name="vec">Vector from which a component will be retrieved</param>
        /// <param name="index">Index of the component to retrieve</param>
        /// <returns>The requested component of the input vector</returns>
        public static unsafe float Get(ref Vector3 vec, int index)
        {
            fixed (Vector3* adr = &vec)
            {
                return ((float*)adr)[index];
            }
        }

        /// <summary>Retrieves a vector component by its index</summary>
        /// <param name="vec">Vector from which a component will be retrieved</param>
        /// <param name="index">Index of the component to retrieve</param>
        /// <returns>The requested component of the input vector</returns>
        public static unsafe float Get(Vector3 vec, int index)
        {
            Vector3* adr = &vec;
            return ((float*)adr)[index];
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <returns>The requested row of the matrix as a Vector3</returns>
        public static unsafe Vector3 Get(Matrix mat, int index)
        {
            float* adr = &mat.M11;
            adr += index;
            return ((Vector3*)adr)[index];
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <returns>The requested row of the matrix as a Vector3</returns>
        public static unsafe Vector3 Get(ref Matrix mat, int index)
        {
            fixed (float* adr = &mat.M11)
            {
                return ((Vector3*)(adr+index))[index];
            }
        }

        /// <summary>Retrieves a row from a matrix by its index</summary>
        /// <param name="mat">Matrix from which a row will be retrieved</param>
        /// <param name="index">Index of the row to retrieve</param>
        /// <param name="vec">Vector into which the row will be copied</param>
        public static unsafe void Get(ref Matrix mat, int index, out Vector3 vec)
        {
            fixed (float* adr = &mat.M11)
            {
                vec = ((Vector3*)(adr + index))[index];
            }
        }
    }
}

#endif // !WINDOWS_PHONE