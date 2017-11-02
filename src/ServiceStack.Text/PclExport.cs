﻿//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServiceStack.Text;
using ServiceStack.Text.Common;

namespace ServiceStack
{
    public static class PlatformNames
    {
        public const string Uwp = "UWP";
        public const string Android = "Android";
        public const string IOS = "IOS";
        public const string Mac = "MAC";
        public const string NetStandard = "NETStandard";
    }

    public abstract class PclExport
    {
        public static PclExport Instance
#if NET45
          = new Net45PclExport()
#elif NET40
          = new Net40PclExport()
#else
          = new NetStandardPclExport()
#endif
        ;

        public static bool ConfigureProvider(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                return false;

            var mi = type.GetMethod("Configure");
            if (mi != null)
            {
                mi.Invoke(null, new object[0]);
            }

            return true;
        }

        public static void Configure(PclExport instance)
        {
            Instance = instance ?? Instance;

            if (Instance != null && Instance.EmptyTask == null)
            {
                var tcs = new TaskCompletionSource<object>();
                tcs.SetResult(null);
                Instance.EmptyTask = tcs.Task;
            }
        }

        public Task EmptyTask;

        public bool SupportsExpression;

        public bool SupportsEmit;

        public char DirSep = '\\';

        public char AltDirSep = '/';

        public static readonly char[] DirSeps = { '\\', '/' };

        public string PlatformName = "Unknown";

        public TextInfo TextInfo = CultureInfo.InvariantCulture.TextInfo;

        public RegexOptions RegexOptions = RegexOptions.None;

        public StringComparison InvariantComparison = StringComparison.Ordinal;

        public StringComparison InvariantComparisonIgnoreCase = StringComparison.OrdinalIgnoreCase;

        public StringComparer InvariantComparer = StringComparer.Ordinal;

        public StringComparer InvariantComparerIgnoreCase = StringComparer.OrdinalIgnoreCase;

        public abstract string ReadAllText(string filePath);

        public virtual string ToTitleCase(string value)
        {
            string[] words = value.Split('_');

            for (int i = 0; i <= words.Length - 1; i++)
            {
                if ((!object.ReferenceEquals(words[i], string.Empty)))
                {
                    string firstLetter = words[i].Substring(0, 1);
                    string rest = words[i].Substring(1);
                    string result = firstLetter.ToUpper() + rest.ToLower();
                    words[i] = result;
                }
            }
            return string.Join("", words);
        }

        // HACK: The only way to detect anonymous types right now.
        public virtual bool IsAnonymousType(Type type)
        {
            return type.IsGeneric() && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal));
        }

        public virtual string ToInvariantUpper(char value)
        {
            return value.ToString().ToUpperInvariant();
        }

        public virtual bool FileExists(string filePath)
        {
            return false;
        }

        public virtual bool DirectoryExists(string dirPath)
        {
            return false;
        }

        public virtual void CreateDirectory(string dirPath)
        {
        }

        public virtual void RegisterLicenseFromConfig()
        {            
        }

        public virtual string GetEnvironmentVariable(string name)
        {
            return null;
        }

        public virtual string[] GetFileNames(string dirPath, string searchPattern = null)
        {
            return TypeConstants.EmptyStringArray;
        }

        public virtual string[] GetDirectoryNames(string dirPath, string searchPattern = null)
        {
            return TypeConstants.EmptyStringArray;
        }

        /// <summary>
        /// When overridden in a descendant class, writes a message followed by a line terminator, to the platform-specific output stream.
        /// The default is <see cref="System.Diagnostics.Debug"/>.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public virtual void WriteLine(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// When overridden in a descendant class, writes a formatted message followed by a line terminator, to the platform-specific output stream.
        /// The default is <see cref="System.Diagnostics.Debug"/>.    
        /// </summary>
        /// <param name="format">A composite format string (see Remarks) that contains text intermixed with zero or more format items,
        /// which correspond to objects in the args array.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public virtual void WriteLine(string format, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(format, args);
        }

        /// <summary>
        /// When overridden in a descendant class, provides a instance of the <see cref="System.Text.Encoding"/> class. A parameter
        /// specifies whether to provide a Unicode byte order mark.
        /// </summary>
        /// <param name="byteOrderMark">true to specify that the <see cref="System.Text.Encoding.GetPreamble"/> method returns
        /// a Unicode byte order mark; otherwise, false. See the Remarks section for more information.</param>
        /// <returns>A System.Text.Encoding instance.</returns>
        public virtual Encoding GetUseEncoding(bool byteOrderMark)
        {
            return new UTF8Encoding(byteOrderMark);
        }

        /// <summary>
        /// When overridden in a descendant class, Initializes a new System.Net.HttpWebRequest instance for the specified URI scheme.
        /// </summary>
        /// <param name="uriString">A URI string that identifies the Internet resource.</param>
        /// <returns>A System.Net.HttpWebRequest instance for the specific URI scheme.</returns>
        /// <exception cref="System.ArgumentNullException">The uriString is null.</exception>
        /// <exception cref="System.NotSupportedException">The request scheme specified in uriString has not been registered.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have permission to connect to the requested URI or a URI
        /// that the request is redirected to.</exception>
        /// <exception cref="System.NotSupportedException">The request scheme specified in uriString is the http or https scheme.</exception>
        /// <exception cref="System.FormatException">The URI specified in uriString is not a valid URI.</exception>
        public virtual HttpWebRequest CreateWebRequest(string uriString)
        {
            return WebRequest.CreateHttp(uriString);
        }

        public virtual Stream GetRequestStream(WebRequest webReq)
        {
            var async = webReq.GetRequestStreamAsync();
            async.Wait();
            return async.Result;
        }

        public virtual WebResponse GetResponse(WebRequest webReq)
        {
            try
            {
                var async = webReq.GetResponseAsync();
                async.Wait();
                return async.Result;
            }
            catch (Exception ex)
            {
                throw ex.UnwrapIfSingleException();
            }
        }

        public virtual bool IsDebugBuild(Assembly assembly)
        {
            return assembly.AllAttributes()
                .Any(x => x.GetType() == typeof(System.Diagnostics.DebuggableAttribute));
        }

        public virtual string MapAbsolutePath(string relativePath, string appendPartialPathModifier)
        {
            return relativePath;
        }

        public virtual Assembly LoadAssembly(string assemblyPath)
        {
            return null;
        }

        public virtual void AddHeader(WebRequest webReq, string name, string value)
        {
            webReq.Headers[name] = value;
        }

        public virtual void SetContentLength(HttpWebRequest httpReq, long value)
        {
            httpReq.Headers[HttpRequestHeader.ContentLength] = value.ToString();
        }

        public virtual void SetAllowAutoRedirect(HttpWebRequest httpReq, bool value)
        {
        }

        public virtual void SetKeepAlive(HttpWebRequest httpReq, bool value)
        {
        }

        public virtual Assembly[] GetAllAssemblies()
        {
            return new Assembly[0];
        }

        public virtual Type FindType(string typeName, string assemblyName)
        {
            return null;
        }

        public virtual string GetAssemblyCodeBase(Assembly assembly)
        {
            return assembly.FullName;
        }

        public virtual string GetAssemblyPath(Type source)
        {
            return null;
        }

        public virtual string GetAsciiString(byte[] bytes)
        {
            return GetAsciiString(bytes, 0, bytes.Length);
        }

        public virtual string GetAsciiString(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        public virtual byte[] GetAsciiBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public virtual SetMemberDelegate CreateSetter(FieldInfo fieldInfo)
        {
            return fieldInfo.SetValue;
        }

        public virtual SetMemberDelegate<T> CreateSetter<T>(FieldInfo fieldInfo)
        {
            return (o,x) => fieldInfo.SetValue(o,x);
        }

        public virtual GetMemberDelegate CreateGetter(FieldInfo fieldInfo)
        {
            return fieldInfo.GetValue;
        }

        public virtual GetMemberDelegate<T> CreateGetter<T>(FieldInfo fieldInfo)
        {
            return x => fieldInfo.GetValue(x);
        }

        public virtual Type UseType(Type type)
        {
            return type;
        }

        public virtual bool InSameAssembly(Type t1, Type t2)
        {
            return t1.AssemblyQualifiedName != null && t1.AssemblyQualifiedName.Equals(t2.AssemblyQualifiedName);
        }

        public virtual Type GetGenericCollectionType(Type type)
        {
            return type.GetTypeInterfaces()
                .FirstOrDefault(t => t.IsGenericType()
                && t.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public virtual SetMemberDelegate CreateSetter(PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.SetMethod();
            if (propertySetMethod == null) return null;

            return (o, convertedValue) =>
                propertySetMethod.Invoke(o, new[] { convertedValue });
        }

        public virtual SetMemberDelegate<T> CreateSetter<T>(PropertyInfo propertyInfo)
        {
            var propertySetMethod = propertyInfo.SetMethod();
            if (propertySetMethod == null) return null;

            return (o, convertedValue) =>
                propertySetMethod.Invoke(o, new[] { convertedValue });
        }

        public virtual GetMemberDelegate CreateGetter(PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetMethodInfo();
            if (getMethodInfo == null) return null;

            return o => propertyInfo.GetMethodInfo().Invoke(o, TypeConstants.EmptyObjectArray);
        }

        public virtual GetMemberDelegate<T> CreateGetter<T>(PropertyInfo propertyInfo)
        {
            var getMethodInfo = propertyInfo.GetMethodInfo();
            if (getMethodInfo == null) return null;

            return o => propertyInfo.GetMethodInfo().Invoke(o, TypeConstants.EmptyObjectArray);
        }

        public virtual string ToXsdDateTimeString(DateTime dateTime)
        {
            return System.Xml.XmlConvert.ToString(dateTime.ToStableUniversalTime(), DateTimeSerializer.XsdDateTimeFormat);
        }

        public virtual string ToLocalXsdDateTimeString(DateTime dateTime)
        {
            return System.Xml.XmlConvert.ToString(dateTime, DateTimeSerializer.XsdDateTimeFormat);
        }

        public virtual DateTime ParseXsdDateTime(string dateTimeStr)
        {
            return System.Xml.XmlConvert.ToDateTimeOffset(dateTimeStr).DateTime;
        }

        public virtual DateTime ParseXsdDateTimeAsUtc(string dateTimeStr)
        {
            return DateTimeSerializer.ParseManual(dateTimeStr, DateTimeKind.Utc)
                ?? DateTime.ParseExact(dateTimeStr, DateTimeSerializer.XsdDateTimeFormat, CultureInfo.InvariantCulture);
        }

        public virtual DateTime ToStableUniversalTime(DateTime dateTime)
        {
            // Silverlight 3, 4 and 5 all work ok with DateTime.ToUniversalTime, but have no TimeZoneInfo.ConverTimeToUtc implementation.
            return dateTime.ToUniversalTime();
        }

        public virtual ParseStringDelegate GetDictionaryParseMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual ParseStringSegmentDelegate GetDictionaryParseStringSegmentMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual ParseStringDelegate GetSpecializedCollectionParseMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual ParseStringSegmentDelegate GetSpecializedCollectionParseStringSegmentMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual ParseStringDelegate GetJsReaderParseMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual ParseStringSegmentDelegate GetJsReaderParseStringSegmentMethod<TSerializer>(Type type)
            where TSerializer : ITypeSerializer
        {
            return null;
        }

        public virtual void CloseStream(Stream stream)
        {
            stream.Flush();
        }

        public virtual void ResetStream(Stream stream)
        {
            stream.Position = 0;
        }

        public virtual LicenseKey VerifyLicenseKeyText(string licenseKeyText)
        {
            return licenseKeyText.ToLicenseKey();
        }

        public virtual LicenseKey VerifyLicenseKeyTextFallback(string licenseKeyText)
        {
            return licenseKeyText.ToLicenseKeyFallback();
        }

        public virtual void BeginThreadAffinity()
        {
        }

        public virtual void EndThreadAffinity()
        {
        }

        public virtual DataContractAttribute GetWeakDataContract(Type type)
        {
            return null;
        }

        public virtual DataMemberAttribute GetWeakDataMember(PropertyInfo pi)
        {
            return null;
        }

        public virtual DataMemberAttribute GetWeakDataMember(FieldInfo pi)
        {
            return null;
        }

        public virtual void RegisterForAot()
        {            
        }

        public virtual string GetStackTrace()
        {
            return null;
        }

        public virtual Task WriteAndFlushAsync(Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            return EmptyTask;
        }
    }

}