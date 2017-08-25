﻿// Copyright (c) ServiceStack, Inc. All Rights Reserved.
// License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;

namespace ServiceStack
{
    /// <summary>
    /// Provides extension methods on System.String instances that contain file or directory path information or uri.
    /// </summary>
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
            return Env.HasMultiplePlatformTargets
                ? PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..{sep}..")
                : PclExport.Instance.MapAbsolutePath(relativePath, $"{sep}..{sep}..");
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
        /// Combines the path elements in a specified System.String array into an base path.
        /// </summary>
        /// <param name="basePath">The base path to combine.</param>
        /// <param name="paths">An string array that contains the path elements to combine.</param>
        /// <returns>The combined paths string.</returns>
        public static string CombineWith(this string basePath, params string[] paths)
        {
            if (basePath.IsNullOrEmpty())
                return CombinePaths(paths);

            return basePath.Replace('\\', '/').TrimEnd('/') + CombinePaths(paths);
        }

        /// <summary>
        /// Combines the path elements in a specified System.String array.
        /// </summary>
        /// <param name="paths">An string array that contains the path elements to combine.</param>
        /// <returns>The combined paths string.</returns>
        public static string CombinePaths(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return "/";

            StringBuilder sb = new StringBuilder(128);
            foreach (var path in paths)
            {
                if (path.IsNullOrEmpty())
                    continue;
                
                if (path[0] != '/' && path[0] != '\\' && (sb.Length == 0 || sb[sb.Length - 1] != '/' && sb[sb.Length - 1] != '\\'))
                    sb.Append('/');

                sb.Append(path);
            }
            sb.Replace('\\', '/');
            return sb.ToString();
        }

        public static string ResolvePaths(this string path)
        {
            if (path == null || path.IndexOfAny("./", "/.") == -1)
                return path;

            var schemePos = path.IndexOf("://", StringComparison.Ordinal);
            var prefix = schemePos >= 0
                ? path.Substring(0, schemePos + 3)
                : "";

            var parts = path.Substring(prefix.Length).Split('/').ToList();
            var combinedPaths = new List<string>();
            foreach (var part in parts)
            {
                if (string.IsNullOrEmpty(part) || part == ".")
                    continue;

                if (part == ".." && combinedPaths.Count > 0)
                    combinedPaths.RemoveAt(combinedPaths.Count - 1);
                else
                    combinedPaths.Add(part);
            }

            var resolvedPath = string.Join("/", combinedPaths);
            if (path[0] == '/' && prefix.Length == 0)
                resolvedPath = '/' + resolvedPath;

            return path[path.Length - 1] == '/' && resolvedPath.Length > 0
                ? prefix + resolvedPath + '/'
                : prefix + resolvedPath;
        }

        public static string[] ToStrings(object[] thesePaths)
        {
            var to = new string[thesePaths.Length];
            for (var i = 0; i < thesePaths.Length; i++)
            {
                to[i] = thesePaths[i].ToString();
            }
            return to;
        }
    }
}