using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BJ
{
    public static class ListStringBuilder
    {
        public delegate String Stringifier(UnityEngine.Object obj);
        private static StringBuilder sb;

        public static string Stringify(IList<UnityEngine.Object> ts)
        {
            RefreshStringBuilder();
            for (int i = 0, count = ts.Count; i < count; i++)
            {
                sb.Append(ts[i].ToString());
            }
            return sb.ToString();
        }

        public static string StringifyDelegate(IList<UnityEngine.Object> ts, Stringifier del)
        {
            RefreshStringBuilder();
            for (int i = 0, count = ts.Count; i < count; i++)
            {
                sb.Append(del(ts[i]));
            }
            return sb.ToString();
        }

        private static void RefreshStringBuilder()
        {
            if (sb == null)
            {
                sb = new StringBuilder();
            }
            else
            {
                sb.Clear();
            }
        }
    }
}
