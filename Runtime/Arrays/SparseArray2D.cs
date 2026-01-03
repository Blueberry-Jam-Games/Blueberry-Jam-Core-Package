using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BJ
{
    /**
     * @brief An implentation of a 2D array that uses a HashMap backing, best for cases where sporadic points are edited at random.
     *        Most other operations are slower than a basic Array2D
     */
    public class SparseArray2D<T> : IArray2D<T>
    {
        // TODO serialize
        private Dictionary<Vector2Int, T> data;

        private T defaultValue;

        /**
         * @brief Width is calculated as the difference between the rightmost element ever entered and the leftmost element ever entered.
         *        To change it, call TrimDimensions
         */
        public int Width => highX - lowX + 1;

        /**
         * @brief Height is calculated as the difference between the highest element ever entered and the lowest element ever entered.
         *        To change it, call TrimDimensions
         */
        public int Height => highY - lowY + 1;

        private int lowX = int.MaxValue;
        private int highX = int.MinValue;
        private int lowY = int.MaxValue;
        private int highY = int.MinValue;

        private StringBuilder sparseStringBuilder;
        private Vector2Int coordsCache;


        public SparseArray2D()
        {
            this.data = new Dictionary<Vector2Int, T>(25);
            defaultValue = default;
        }

        public SparseArray2D(T defaultValue)
        {
            this.data = new Dictionary<Vector2Int, T>(25);
            this.defaultValue = defaultValue;
        }

        public T this[int i]
        {
            get
            {
                // Vector2Int coords = this.Coordinates(i, Width, Height);
                Vector2Int coords = Array2DHelpers.Coordinates(i, Width, Height);
                if (data.TryGetValue(coords, out T value))
                {
                    return value;
                }
                else
                {
                    return defaultValue;
                }
            }
            set
            {
                // copied to avoid allocating a Vector2Int
                int x = Mathf.FloorToInt(i / Width);
                int y = i % Height;
                this[x, y] = value;
            }
        }
        public T this[int i, int j]
        {
            get
            {
                if (coordsCache == null)
                {
                    coordsCache = new Vector2Int(i, j);
                }
                coordsCache.x = i;
                coordsCache.y = j;
                if (data.TryGetValue(coordsCache, out T value))
                {
                    return value;
                }
                else
                {
                    return defaultValue;
                }
            }
            set
            {
                if (coordsCache == null)
                {
                    coordsCache = new Vector2Int(i, j);
                }
                coordsCache.x = i;
                coordsCache.y = j;

                data[coordsCache] = value;

                if (i < lowX) lowX = i;
                if (i > highX) highX = i;

                if (j < lowY) lowY = j;
                if (j > highY) highY = j;
            }
        }

        public int Count
        {
            get
            {
                return Width * Height;
            }
        }

        public bool IsReadOnly => false;

        public void Add(KeyValuePair<Vector2Int, T> item)
        {
            this[item.Key.x, item.Key.y] = item.Value;
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<Vector2Int, T> item)
        {
            return data.ContainsKey(item.Key) && data[item.Key].Equals(item.Value);
        }

        // What even uses this?
        public void CopyTo(KeyValuePair<Vector2Int, T>[] array, int arrayIndex)
        {
            int index = 0;
            foreach (KeyValuePair<Vector2Int, T> kvp in data)
            {
                array[index] = kvp;
                index++;
            }
        }

        public IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool Remove(KeyValuePair<Vector2Int, T> item)
        {
            if (data.ContainsKey(item.Key) && data[item.Key].Equals(item.Value))
            {
                data.Remove(item.Key);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveAt(int x, int y)
        {
            if (coordsCache == null)
            {
                coordsCache = new Vector2Int(x, y);
            }
            coordsCache.x = x;
            coordsCache.y = y;

            if (data.ContainsKey(coordsCache))
            {
                data.Remove(coordsCache);
                return true;
            }
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public void TrimDimensions()
        {
            lowX = int.MaxValue;
            highX = int.MinValue;
            lowY = int.MaxValue;
            highY = int.MinValue;

            foreach (KeyValuePair<Vector2Int, T> kvp in data)
            {
                int x = kvp.Key.x;
                int y = kvp.Key.y;

                if (x < lowX) lowX = x;
                if (x > highX) highX = x;

                if (y < lowY) lowY = y;
                if (y > highY) highY = y;
            }
        }

        public override string ToString()
        {
            sparseStringBuilder ??= new StringBuilder();
            sparseStringBuilder.Clear();

            sparseStringBuilder.AppendLine($"({Width}x{Height}) Array of type {typeof(T).Name}, Top left ({lowX}, {lowY}), Bottom right: {highX}, {highY})");

            sparseStringBuilder.Append("Y> ");
            for (int y = lowY; y <= highY; y++)
            {
                sparseStringBuilder.Append($"{y} ");
            }
            sparseStringBuilder.Append("<\n");

            sparseStringBuilder.Append("{");
            for (int x = lowX; x <= highX; x++)
            {
                sparseStringBuilder.Append($"{x}: {{");
                for (int y = lowY; y <= highY; y++)
                {
                    sparseStringBuilder.Append(Array2DHelpers.ToStringOrNull(this[x, y]));
                    sparseStringBuilder.Append(" ");
                }
                if (x + 1 <= highY)
                {
                    sparseStringBuilder.Append("}\n");
                }
                else
                {
                    sparseStringBuilder.Append("}}");
                }
            }

            return sparseStringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SparseArray2D<T> other = (SparseArray2D<T>)obj;

            if (other.Width != Width || other.Height != Height)
            {
                return false;
            }

            bool same = true;

            foreach (KeyValuePair<Vector2Int, T> kvp in data)
            {
                if (!other.Contains(kvp))
                {
                    same = false;
                }
            }
            foreach (KeyValuePair<Vector2Int, T> kvp in other.data)
            {
                if (Contains(kvp))
                {
                    same = false;
                }
            }

            return same;
        }

        public override int GetHashCode()
        {
            // If value is null, use 0 as the hash code
            return Width.GetHashCode() ^ Height.GetHashCode() ^ data.GetHashCode();
        }
    }
}
