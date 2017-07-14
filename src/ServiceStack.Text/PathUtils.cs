// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using ServiceStack.Text;

namespace ServiceStack
{
    public static class PathUtils
    {
        public static string MapAbsolutePath(this string relativePath, string appendPartialPathModifier)
        {
            return PclExport.Instance.MapAbsolutePath(relativePath, appendPartialPathModifier);
        }

        /// <summary>
        /// Maps the path of a file in the context of a VS project in a Console App
        /// </summary>
        /// <param name="relativePath">the relative path</param>
        /// <returns>the absolute path</returns>
        /// <remarks>Assumes static content is two directories above the /bin/ directory,
        /// eg. in a unit test scenario  the assembly would be in /bin/Debug/.</remarks>
        public static string MapProjectPath(this string relativePath)
        {
            var sep = PclExport.Instance.DirSep;
#if !NETSTANDARD1_1
            return PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..");
#else
            return PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..{sep}..");
#endif
        }
        /// <summary>
        /// Maps the path of a file in the context of a VS 2017+ multi-platform project in a Console App
        /// </summary>
        /// <param name="relativePath">the relative path</param>
        /// <returns>the absolute path</returns>
        /// <remarks>Assumes static content is two directories above the /bin/ directory,
        /// eg. in a unit test scenario  the assembly would be in /bin/Debug/net45</remarks>
        public static string MapProjectPlatformPath(this string relativePath)
        {
            var sep = PclExport.Instance.DirSep;
            return PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..{sep}..");
        }

        /// <summary>
        /// Maps the path of a file in the bin\ folder of a self-hosted scenario
        /// </summary>
        /// <param name="relativePath">the relative path</param>
        /// <returns>the absolute path</returns>
        /// <remarks>Assumes static content is copied to /bin/ folder with the assemblies</remarks>
        public static string MapAbsolutePath(this string relativePath)
        {
            return PclExport.Instance.MapAbsolutePath(relativePath, null);
        }

        /// <summary>
        /// Maps the path of a file in an ASP.NET hosted scenario
        /// </summary>
        /// <param name="relativePath">the relative path</param>
        /// <returns>the absolute path</returns>
        /// <remarks>Assumes static content is in the parent folder of the /bin/ directory</remarks>
        public static string MapHostAbsolutePath(this string relativePath)
        {
            var sep = PclExport.Instance.DirSep;
#if !NETSTANDARD1_1
            return PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..");
#else
            return PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..{sep}..");
#endif
        }

        internal static List<To> Map<To>(System.Collections.IEnumerable items, Func<object, To> converter)
        {
            if (items == null)
                return new List<To>();

            var list = new List<To>();
            foreach (var item in items)
            {
                list.Add(converter(item));
            }
            return list;
        }

        /// <summary>
        /// Combines an array of strings into an url.
        /// </summary>
        /// <param name="baseUrl">The base url to combine.</param>
        /// <param name="paths">An array of parts of the url.</param>
        /// <returns>The combined urls.</returns>
        public static string CombineWith(this string baseUrl, params string[] paths)
        {
            if (string.IsNullOrEmpty(baseUrl))
                return CombinePaths(paths);

            return baseUrl.Replace('\\', '/').TrimEnd('/') + CombinePaths(paths);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CombinePaths(params string[] paths)
        {
            if (paths.Length == 0) return "/";

            var sb = StringBuilderCache.Allocate();
            PathUtils.AppendPaths(sb, paths);
            return StringBuilderCache.Retrieve(sb);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void AppendPaths(StringBuilder sb, string[] paths)
        {
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                if (path[0] != '/' && path[0] != '\\')
                    sb.Append('/');
                int length = sb.Length;
                sb.Append(path);
                sb.Replace('\\', '/', length, path.Length);
            }
        }
    }

}