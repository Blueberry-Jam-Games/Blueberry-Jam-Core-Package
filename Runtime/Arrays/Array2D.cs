using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BJ
{
    /**
     * @brief A conventional, dense implemntation of 2D Arrays
     *        Resizing is done by copying the array element-wise
     */
    [System.Serializable]
    public class Array2D<T> : IArray2D<T>
    {
        [SerializeField]
        private T[] data;

        [SerializeField]
        private int width;

        [SerializeField]
        private int height;

        private T defaultValue;

        public int Count
        {
            get
            {
                return width * height;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public int Width => width;

        public int Height => height;

        public Array2D()
        {
            this.width = 5;
            this.height = 5;
            this.data = new T[this.width * this.height];
            defaultValue = default;
        }

        public Array2D(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.data = new T[width * height];
            defaultValue = default;
        }

        public Array2D(int width, int height, T defaultValue)
        {
            this.width = width;
            this.height = height;

            this.data = new T[width * height];
            for (int x = 0; x < Count; x++)
            {
                data[x] = defaultValue;
            }
            this.defaultValue = defaultValue;
        }

        public T this[int i, int j]
        {
            get
            {
                if (i >= 0 && i < width && j >= 0 && j < height)
                {
                    return data[Position(i, j)];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The width or height is too large.");
                }
            }
            set
            {
                if (i < 0 || j < 0)
                {
                    throw new ArgumentOutOfRangeException("Negative indecies are not supported for this array type.");
                }
                else if (i > width || j > height)
                {
                    // Do resize
                    int newWidth = width;
                    int newHeight = height;

                    if (i > width)
                    {
                        newWidth = Mathf.Max(2 * width, i);
                    }
                    if (j > height)
                    {
                        newHeight = Mathf.Max(2 * height, j);
                    }

                    Resize(newWidth, newHeight);

                    // Finally, write the new value
                    data[Position(i, j)] = value;
                }
                else
                {
                    data[Position(i, j)] = value;
                }
            }
        }

        public T this[int i]
        {
            get
            {
                int y = i % width;
                int x = Mathf.FloorToInt(i / width);
                return this[x, y];
            }
            set
            {
                int y = i % width;
                int x = Mathf.FloorToInt(i / width);
                this[x, y] = value;
            }
        }

        public void Add(KeyValuePair<Vector2Int, T> item)
        {
            this[item.Key.x, item.Key.y] = item.Value;
        }

        public void Clear()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    this[x, y] = defaultValue;
                }
            }
        }

        public bool Contains(KeyValuePair<Vector2Int, T> item)
        {
            if (this[item.Key.x, item.Key.y].Equals(item.Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Remove(KeyValuePair<Vector2Int, T> item)
        {
            int x = item.Key.x;
            int y = item.Key.y;

            if (x < 0 || y < 0 || x > width || y > height)
            {
                return false;
            }
            else if (this[item.Key.x, item.Key.y].Equals(item.Value))
            {
                this[item.Key.x, item.Key.y] = defaultValue;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(KeyValuePair<Vector2Int, T>[] array, int arrayIndex)
        {
            for (int x = arrayIndex; x < Count; x++)
            {
                array[x] = new KeyValuePair<Vector2Int, T>(Coordinates(x), this[x]);
            }
        }

        /**
         * @brief Calls todo for each position in the array. The loop is done with no iterator creation so there is no garbage produced.
         * @param todo A function called for each element that receives (x, y, Element)
         */
        public void FlatForeach(Action<int, int, T> todo)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    todo?.Invoke(x, y, this[x, y]);
                }
            }
        }

        /**
         * @brief Resizes the array copying the old data into a new array. If the capacity is larger, new spaces are left at default, if new size is smaller, excess elements are dropped.
         * @param newWidth The new width of the array.
         * @param newHeight The new height of the array.
         */
        public void Resize(int newWidth, int newHeight)
        {
            T[] newData = new T[newWidth * newHeight];

            int itrWidth = Mathf.Min(width, newWidth);
            int itrHeight = Mathf.Min(height, newHeight);

            // copy
            for (int x = 0; x < itrWidth; x++)
            {
                for (int y = 0; y < itrHeight; y++)
                {
                    newData[x * newWidth + y] = data[Position(x, y)];
                }
            }

            // override live data
            width = newWidth;
            height = newHeight;
            data = newData;
        }

        public int Position(int x, int y)
        {
            return x * width + y;
        }

        public Vector2Int Coordinates(int index)
        {
            int x = Mathf.FloorToInt(index / width);
            int y = index % height;
            return new Vector2Int(x, y);
        }

        private StringBuilder arrayStringBuilder;
        public override string ToString()
        {
            // if it's null make a new one
            arrayStringBuilder ??= new StringBuilder();
            // clear string builder
            arrayStringBuilder.Clear();

            arrayStringBuilder.AppendLine($"({width}x{height}) Array of type {typeof(T).Name}");
            arrayStringBuilder.Append("{{ ");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    arrayStringBuilder.Append(Array2DHelpers.ToStringOrNull(this[x, y]));
                    arrayStringBuilder.Append(" ");
                }
                if (x + 1 < width)
                {
                    arrayStringBuilder.Append("}\n");
                }
                else
                {
                    arrayStringBuilder.Append("}}");
                }
            }

            return arrayStringBuilder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Array2D<T> other = (Array2D<T>)obj;

            if (other.width != width || other.height != height)
            {
                return false;
            }

            bool same = true;
            other.FlatForeach((x, y, q) =>
            {
                if (!this[x, y].Equals(q))
                {
                    same = false;
                }
            });

            return same;
        }

        public override int GetHashCode()
        {
            // If value is null, use 0 as the hash code
            return width.GetHashCode() ^ height.GetHashCode();
        }

        public IEnumerator<KeyValuePair<Vector2Int, T>> GetEnumerator()
        {
            return new Array2DEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class Array2DEnumerator<T> : IEnumerator<KeyValuePair<Vector2Int, T>>
    {
        // Width starts at -1 because it moves on a MoveNext call, height does not.
        private int x = -1;

        private Array2D<T> root;

        KeyValuePair<Vector2Int, T> IEnumerator<KeyValuePair<Vector2Int, T>>.Current
        {
            get
            {
                return new KeyValuePair<Vector2Int, T>(root.Coordinates(x), root[x]);
            }
        }

        public T Current
        {
            get
            {
                return root[x];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        internal Array2DEnumerator(Array2D<T> root)
        {
            this.root = root;
        }

        public bool MoveNext()
        {
            x++;
            return x < root.Count;
        }

        public void Reset()
        {
            x = -1;
        }

        public void Dispose()
        {
            // pass
        }
    }
}
