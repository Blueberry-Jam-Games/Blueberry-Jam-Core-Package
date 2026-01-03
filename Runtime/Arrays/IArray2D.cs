using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BJ
{
    /**
     * @brief IArray2D defines details for a 2D array. The index is 2 ints, X and Y which can be treated as Vector2Int as well. Values can be any type.
     */
    public interface IArray2D<T> : ICollection<KeyValuePair<Vector2Int, T>>, IEnumerable<KeyValuePair<Vector2Int, T>>
    {
        public int Width { get; }
        public int Height { get; }
        public T this[int i, int j]
        {
            get;
            set;
        }

        /**
         * @brief Allows indexing using a flatened interpretation of the array. For example, in a 5x5 2d array, index 0 is top left, index 5 is the left element in the second row.
         */
        public T this[int i]
        {
            get;
            set;
        }
    }

    public static class Array2DHelpers
    {
        public static Vector2Int Coordinates(int index, int width, int height)
        {
            int x = Mathf.FloorToInt(index / width);
            int y = index % height;
            return new Vector2Int(x, y);
        }

        internal static string ToStringOrNull<T>(T unknown)
        {
            if (unknown == null)
            {
                return "null";
            }
            else
            {
                return unknown.ToString();
            }
        }
    }
}
