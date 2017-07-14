using System;
using System.Text;

namespace ServiceStack.Text
{
    public static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder cache;
        public static StringBuilder Cache => cache;

        public static StringBuilder Allocate()
        {
            StringBuilder stringBuilder = new StringBuilder(256);
            cache = stringBuilder;
            return stringBuilder;
        }

        public static StringBuilder Allocate(string value)
        {
            StringBuilder stringBuilder = new StringBuilder(value);
            cache = stringBuilder;
            return stringBuilder;
        }

        public static void Free(StringBuilder sb)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            sb.Clear();
        }

        public static string Retrieve(StringBuilder sb, bool free = true)
        {
            if (sb == null)
                throw new ArgumentNullException(nameof(sb));

            var ret = sb.ToString();
            if (free)
                Free(sb);
            return ret;
        }
    }
}