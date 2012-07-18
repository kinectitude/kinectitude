using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kinectitude.Core.Base
{
    /// <summary>
    /// Array access that allows only reading
    /// </summary>
    public class ReadonlyArray <T>
    {
        private readonly T[] array;

        /// <summary>
        /// If true, the array is backed by something that can change.
        /// </summary>
        public bool IsBacked { get; private set; }

        /// <summary>
        /// Creates a read-only version of the array
        /// </summary>
        /// <param name="array">Array to make read-only copy of</param>
        /// <param name="isBacked">If true, changing the passed array will change the read-only copy</param>
        public ReadonlyArray(T[] array, bool isBacked = false)
        {
            if (isBacked)
            {
                this.array = array;
            }
            else
            {
                this.array = new T[array.Length];
                Array.Copy(array, this.array, array.Length);
            }
        }

        /// <summary>
        /// Gets an element in the arrray
        /// </summary>
        /// <param name="i">The array element to get</param>
        /// <returns>The ith element in the array (starting with index 0)</returns>
        public T this[int i]
        {
            get { return array[i]; }
        }

        /// <summary>
        /// The length of the array
        /// </summary>
        public int Length
        {
            get { return array.Length; }
        }

    }
}
