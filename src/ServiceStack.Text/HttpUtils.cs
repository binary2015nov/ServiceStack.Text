//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceStack
{
    /// <summary>
    /// Provides methods for the HTTP-specific request to a Uniform Resource Identifier (URI).
    /// </summary>
    public static class HttpUtils
    {
        public static string AddQueryParam(this string urlString, string key, object val, bool encode = true)
        {
            return urlString.AddQueryParam(key, val.ToString(), encode);
        }

        public static string AddQueryParam(this string urlString, object key, string val, bool encode = true)
        {
            return AddQueryParam(urlString, (key ?? "").ToString(), val, encode);
        }

        public static string AddQueryParam(this string urlString, string key, string val, bool encode = true)
        {
            if (string.IsNullOrEmpty(urlString)) return null;
            var prefix = string.Empty;
            if (!urlString.EndsWith("?") && !urlString.EndsWith("&"))
            {
                prefix = urlString.IndexOf('?') == -1 ? "?" : "&";
            }
            return urlString + prefix + key + "=" + (encode ? val.UrlEncode() : val);
        }

        public static string SetQueryParam(this string urlString, string key, string val)
        {
            if (string.IsNullOrEmpty(urlString)) return null;
            var qsPos = urlString.IndexOf('?');
            if (qsPos != -1)
            {
                var existingKeyPos = qsPos + 1 == urlString.IndexOf(key, qsPos, PclExport.Instance.InvariantComparison)
                    ? qsPos
                    : urlString.IndexOf("&" + key, qsPos, PclExport.Instance.InvariantComparison);

                if (existingKeyPos != -1)
                {
                    var endPos = urlString.IndexOf('&', existingKeyPos + 1);
                    if (endPos == -1)
                        endPos = urlString.Length;

                    var newUrl = urlString.Substring(0, existingKeyPos + key.Length + 1)
                        + "="
                        + val.UrlEncode()
                        + urlString.Substring(endPos);
                    return newUrl;
                }
            }
            var prefix = qsPos == -1 ? "?" : "&";
            return urlString + prefix + key + "=" + val.UrlEncode();
        }

        public static string AddHashParam(this string urlString, string key, object val)
        {
            return urlString.AddHashParam(key, val.ToString());
        }

        public static string AddHashParam(this string urlString, string key, string val)
        {
            if (string.IsNullOrEmpty(urlString)) return null;
            var prefix = urlString.IndexOf('#') == -1 ? "#" : "/";
            return urlString + prefix + key + "=" + val.UrlEncode();
        }

        public static string SetHashParam(this string urlString, string key, string val)
        {
            if (string.IsNullOrEmpty(urlString)) return null;
            var hPos = urlString.IndexOf('#');
            if (hPos != -1)
            {
                var existingKeyPos = hPos + 1 == urlString.IndexOf(key, hPos, PclExport.Instance.InvariantComparison)
                    ? hPos
                    : urlString.IndexOf("/" + key, hPos, PclExport.Instance.InvariantComparison);

                if (existingKeyPos != -1)
                {
                    var endPos = urlString.IndexOf('/', existingKeyPos + 1);
                    if (endPos == -1)
                        endPos = urlString.Length;

                    var newUrl = urlString.Substring(0, existingKeyPos + key.Length + 1)
                        + "="
                        + val.UrlEncode()
                        + urlString.Substring(endPos);
                    return newUrl;
                }
            }
            var prefix = urlString.IndexOf('#') == -1 ? "#" : "/";
            return urlString + prefix + key + "=" + val.UrlEncode();
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Json"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.-or- An error occurred while processing the request.</exception>
        public static string GetJsonFromUrl(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: method, contentType: contentType, requestBody: requestBody, 
                accept: MimeTypes.Json, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Json"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>Returns System.Threading.Tasks`1. The task object representing the asynchronous operation.</returns>
        public static Task<string> GetJsonFromUrlAsync(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: method, contentType: contentType, requestBody: requestBody, 
                accept: MimeTypes.Json, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Xml"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.-or- An error occurred while processing the request.</exception>
        public static string GetXmlFromUrl(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: method, contentType: contentType, requestBody: requestBody,
                accept: MimeTypes.Xml, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request as an asynchronous operation
        /// whose Accept HTTP header is <see cref="MimeTypes.Xml"/> with the optional request and response filter.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>Returns System.Threading.Tasks`1. The task object representing the asynchronous operation.</returns>
        public static Task<string> GetXmlFromUrlAsync(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: method, contentType: contentType, requestBody: requestBody,
                accept: MimeTypes.Xml, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Csv"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>    
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.-or- An error occurred while processing the request.</exception>
        public static string GetCsvFromUrl(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: method, contentType: contentType, requestBody: requestBody,
                accept: MimeTypes.Csv, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Csv"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>Returns System.Threading.Tasks`1. The task object representing the asynchronous operation.</returns>
        public static Task<string> GetCsvFromUrlAsync(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: method, contentType: contentType, requestBody: requestBody,
                accept: MimeTypes.Csv, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Html"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.-or- An error occurred while processing the request.</exception>
        public static string GetHtmlFromUrl(this string urlString, string method = HttpMethods.Get, 
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: method, contentType: contentType, requestBody: requestBody, 
                accept: MimeTypes.Html, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request whose 
        /// Accept HTTP header is <see cref="MimeTypes.Html"/>.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        public static Task<string> GetHtmlFromUrlAsync(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: method, contentType: contentType, requestBody: requestBody,
                accept: MimeTypes.Html, encoding: encoding, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostStringToUrl(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostStringToUrlAsync(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostToUrl(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostToUrlAsync(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostToUrl(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrl(urlString, method: HttpMethods.Post,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostToUrlAsync(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostJsonToUrl(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostJsonToUrlAsync(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostJsonToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostJsonToUrlAsync(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostXmlToUrl(this string urlString, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostXmlToUrlAsync(this string urlString, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post, requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostCsvToUrl(this string urlString, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PostCsvToUrlAsync(this string urlString, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Post, requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutStringToUrl(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutStringToUrlAsync(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutToUrl(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutToUrlAsync(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutToUrl(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrl(urlString, method: HttpMethods.Put,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutToUrlAsync(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutJsonToUrl(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutJsonToUrlAsync(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutJsonToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutJsonToUrlAsync(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutXmlToUrl(this string urlString, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutXmlToUrlAsync(this string urlString, string xml,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put, requestBody: xml, contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutCsvToUrl(this string urlString, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PutCsvToUrlAsync(this string urlString, string csv,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Put, requestBody: csv, contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PatchStringToUrl(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Patch,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PatchStringToUrlAsync(this string urlString, string requestBody = null,
            string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Patch,
                requestBody: requestBody, contentType: contentType,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PatchToUrl(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Patch,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PatchToUrlAsync(this string urlString, string formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Patch,
                contentType: MimeTypes.FormUrlEncoded, requestBody: formData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PatchToUrl(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrl(urlString, method: HttpMethods.Patch,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PatchToUrlAsync(this string urlString, object formData = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            string postFormData = formData != null ? QueryStringSerializer.SerializeToString(formData) : null;

            return GetStringFromUrlAsync(urlString, method: HttpMethods.Patch,
                contentType: MimeTypes.FormUrlEncoded, requestBody: postFormData,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PatchJsonToUrl(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Patch, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PatchJsonToUrlAsync(this string urlString, string json,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Patch, requestBody: json, contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PatchJsonToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Patch, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> PatchJsonToUrlAsync(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Patch, requestBody: data.ToJson(), contentType: MimeTypes.Json, accept: MimeTypes.Json,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string DeleteFromUrl(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Delete, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> DeleteFromUrlAsync(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Delete, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string OptionsFromUrl(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Options, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> OptionsFromUrlAsync(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Options, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string HeadFromUrl(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Head, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<string> HeadFromUrlAsync(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrlAsync(urlString, method: HttpMethods.Head, accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request using the specified request options.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="accept">The value of the Accept HTTP header. The default value is "*/*".</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request,
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>The string representation response to the HTTP-specific request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.-or- An error occurred while processing the request.</exception>
        public static string GetStringFromUrl(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, string accept = "*/*", Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = PclExport.Instance.CreateWebRequest(urlString);
            if (!method.IsNullOrEmpty())
                webReq.Method = method;
            if (!contentType.IsNullOrEmpty())
                webReq.ContentType = contentType;

            webReq.Accept = accept;

            if (requestBody != null)
            {
                using (var reqStream = PclExport.Instance.GetRequestStream(webReq))
                using (var writer = new StreamWriter(reqStream, encoding ?? PclExport.Instance.GetUseEncoding(false)))
                {
                    writer.Write(requestBody);
                }
            }
            requestFilter?.Invoke(webReq);
            using (var webRes = PclExport.Instance.GetResponse(webReq))
            {
                responseFilter?.Invoke((HttpWebResponse)webRes);
                return webRes.ReadToEnd(encoding);
            }
        }

        /// <summary>
        /// Gets the string representation response from the Internet resource to an HTTP-specific request as an asynchronous operation using the specified request options. 
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <param name="method">The request method to use to contact the Internet resource. The default value is GET.</param>
        /// <param name="requestBody">The optional request body associated with the Http-specific request.</param>
        /// <param name="contentType">The value of the Content-type HTTP header. The default value is null.</param>
        /// <param name="accept">The value of the Accept HTTP header. The default value is "*/*".</param>
        /// <param name="encoding">The character encoding to use for reading from the response to the HTTP-specific request, 
        /// if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <param name="requestFilter">The method to filter the HTTP-specific request.</param>
        /// <param name="responseFilter">The method to filter the response to the HTTP-specific request.</param>
        /// <returns>Returns System.Threading.Tasks`1. The task object representing the asynchronous operation.</returns>
        public static Task<string> GetStringFromUrlAsync(this string urlString, string method = HttpMethods.Get,
            string requestBody = null, string contentType = null, string accept = "*/*", Encoding encoding = null,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return Task.Factory.StartNew(() => GetStringFromUrl(urlString, method, requestBody, contentType, accept,
                encoding, requestFilter, responseFilter));            
        }

        public static byte[] GetBytesFromUrl(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return urlString.SendBytesToUrl(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<byte[]> GetBytesFromUrlAsync(this string urlString, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return urlString.SendBytesToUrlAsync(accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static byte[] PostBytesToUrl(this string urlString, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(urlString, method: HttpMethods.Post,
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<byte[]> PostBytesToUrlAsync(this string urlString, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrlAsync(urlString, method: HttpMethods.Post,
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static byte[] PutBytesToUrl(this string urlString, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrl(urlString, method: HttpMethods.Put,
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static Task<byte[]> PutBytesToUrlAsync(this string urlString, byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return SendBytesToUrlAsync(urlString, method: HttpMethods.Put,
                contentType: contentType, requestBody: requestBody,
                accept: accept, requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static byte[] SendBytesToUrl(this string urlString, string method = null,
            byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            var webReq = PclExport.Instance.CreateWebRequest(urlString);
            if (!method.IsNullOrEmpty())          
                webReq.Method = method;
            if (!contentType.IsNullOrEmpty())
                webReq.ContentType = contentType;

            webReq.Accept = accept;

            if (requestBody != null)
            {
                using (var req = PclExport.Instance.GetRequestStream(webReq))
                {
                    req.Write(requestBody, 0, requestBody.Length);
                }
            }
            requestFilter?.Invoke(webReq);
            using (var webRes = PclExport.Instance.GetResponse(webReq))
            {
                responseFilter?.Invoke((HttpWebResponse)webRes);

                using (var stream = webRes.GetResponseStream())
                {
                    return stream.ReadFully();
                }
            }
        }

        public static Task<byte[]> SendBytesToUrlAsync(this string urlString, string method = null,
            byte[] requestBody = null, string contentType = null, string accept = "*/*",
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return Task.Factory.StartNew(() => SendBytesToUrl(urlString, method, requestBody, contentType, accept,
                requestFilter, responseFilter));
        }

        public static bool IsAny300(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.MultipleChoices && status < HttpStatusCode.BadRequest;
        }

        public static bool IsAny400(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.BadRequest && status < HttpStatusCode.InternalServerError;
        }

        public static bool IsAny500(this Exception ex)
        {
            var status = ex.GetStatus();
            return status >= HttpStatusCode.InternalServerError && (int)status < 600;
        }

        public static bool IsNotModified(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.NotModified;
        }

        public static bool IsBadRequest(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.BadRequest;
        }

        public static bool IsNotFound(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.NotFound;
        }

        public static bool IsUnauthorized(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.Unauthorized;
        }

        public static bool IsForbidden(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.Forbidden;
        }

        public static bool IsInternalServerError(this Exception ex)
        {
            return GetStatus(ex) == HttpStatusCode.InternalServerError;
        }

        public static HttpStatusCode? GetResponseStatus(this string url)
        {
            try
            {
                var webReq = (HttpWebRequest)WebRequest.Create(url);
                using (var webRes = PclExport.Instance.GetResponse(webReq))
                {
                    var httpRes = webRes as HttpWebResponse;
                    return httpRes?.StatusCode;
                }
            }
            catch (Exception ex)
            {
                return ex.GetStatus();
            }
        }

        public static HttpStatusCode? GetStatus(this Exception ex)
        {
            if (ex == null)
                return null;

            if (ex is WebException webEx)
                return GetStatus(webEx);

            if (ex is IHasStatusCode hasStatus)
                return (HttpStatusCode)hasStatus.StatusCode;

            return null;
        }

        public static HttpStatusCode? GetStatus(this WebException webEx)
        {
            var httpRes = webEx?.Response as HttpWebResponse;
            return httpRes?.StatusCode;
        }

        public static bool HasStatus(this Exception ex, HttpStatusCode statusCode)
        {
            return GetStatus(ex) == statusCode;
        }

        public static string GetResponseBody(this Exception ex)
        {
            var webEx = ex as WebException;
            if (webEx == null || webEx.Response == null
#if !(SL5 || PCL || NETSTANDARD1_1)
                || webEx.Status != WebExceptionStatus.ProtocolError
#endif
            ) return null;

            var errorResponse = (HttpWebResponse)webEx.Response;
            using (var reader = new StreamReader(errorResponse.GetResponseStream(), PclExport.Instance.GetUseEncoding(false)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads all characters from the current position to the end of the data stream from the Internet response, with the specified character encoding.
        /// </summary>
        /// <param name="response">The response from an Uniform Resource Identifier (URI).</param>
        /// <param name="encoding">The character encoding to use, if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <returns>The rest of the data stream from the Internet response as a string, from the current position to the end. If
        /// the current position is at the end of the stream, returns an empty string.</returns>
        public static string ReadToEnd(this WebResponse response, Encoding encoding = null)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, encoding ?? PclExport.Instance.GetUseEncoding(false)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads all lines of characters from the current position to the end of the data stream from the Internet response, 
        /// with the specified character encoding.
        /// </summary>
        /// <param name="response">The response from an Uniform Resource Identifier (URI).</param>
        /// <param name="encoding">The character encoding to use, if the value is null use <c>PclExport.Instance.GetUseEncoding(false)</c>.</param>
        /// <returns>An System.Collections.Generic.IEnumerable`1 that contains System.String elements from the Internet response.</returns>
        public static IEnumerable<string> ReadLines(this WebResponse response, Encoding encoding = null)
        {
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream, encoding ?? PclExport.Instance.GetUseEncoding(false)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Gets a response from the Internet resource to an HTTP-specific request.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <returns>A <see cref="System.Net.HttpWebResponse"/> containing the response to the Internet request.</returns>
        /// <exception cref="System.FormatException">The URI specified in urlString is not a valid URI.</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort was previously called.-or- The time-out period
        /// for the request expired.</exception>
        public static HttpWebResponse GetWebResponse(this string urlString)
        {
            try
            {
                var webRequest = PclExport.Instance.CreateWebRequest(urlString);
                return (HttpWebResponse)PclExport.Instance.GetResponse(webRequest);
            }
            catch (WebException webEx)
            {
                if (webEx.Response is HttpWebResponse)
                    return (HttpWebResponse)webEx.Response;

                throw;
            }
        }

        /// <summary>
        /// Gets a response from the Internet resource to an HTTP-specific request as an asynchronous operation.
        /// </summary>
        /// <param name="urlString">A URI string that identifies the Internet resource.</param>
        /// <returns>Returns System.Threading.Tasks`1. The task object representing the asynchronous operation.</returns>
        public static Task<HttpWebResponse> GetWebResponseAsync(string urlString)
        {
            return Task.Factory.StartNew(p => GetWebResponse(p as string), urlString);
        }

        public static Task<TBase> ConvertTo<TDerived, TBase>(this Task<TDerived> task) where TDerived : TBase
        {
            var tcs = new TaskCompletionSource<TBase>();
            task.ContinueWith(t => tcs.SetResult(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
            task.ContinueWith(t => tcs.SetException(t.Exception.InnerExceptions), TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => tcs.SetCanceled(), TaskContinuationOptions.OnlyOnCanceled);
            return tcs.Task;
        }

        public static void UploadFile(this HttpWebRequest webRequest, Stream fileStream, string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            var mimeType = MimeTypes.GetMimeType(fileName);
            if (mimeType == null)
                throw new ArgumentException("Mime-type not found for file: " + fileName);

            UploadFile(webRequest, fileStream, fileName, mimeType);
        }

        public static void UploadFile(this HttpWebRequest webRequest, Stream fileStream, string fileName, string mimeType,
            string accept = null, Action<HttpWebRequest> requestFilter = null, string method = HttpMethods.Post)
        {
            var httpReq = webRequest;
            httpReq.Method = method;

            if (accept != null)
                httpReq.Accept = accept;

            requestFilter?.Invoke(httpReq);

            var boundary = Guid.NewGuid().ToString("N");

            httpReq.ContentType = "multipart/form-data; boundary=\"" + boundary + "\"";

            var boundarybytes = ("\r\n--" + boundary + "--\r\n").ToAsciiBytes();

            var headerTemplate = "\r\n--" + boundary +
                                 "\r\nContent-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: {1}\r\n\r\n";

            var header = string.Format(headerTemplate, fileName, mimeType);

            var headerbytes = header.ToAsciiBytes();
            var contentLength = fileStream.Length + headerbytes.Length + boundarybytes.Length;

#if NET45 || NET40
            httpReq.AllowAutoRedirect = false;
            httpReq.ContentLength = contentLength;
            httpReq.KeepAlive = false;
#else
            httpReq.Headers[HttpRequestHeader.ContentLength] = contentLength.ToString();
            httpReq.Headers[HttpRequestHeader.KeepAlive] = "false";
#endif

            using (var outputStream = PclExport.Instance.GetRequestStream(httpReq))
            {
                outputStream.Write(headerbytes, 0, headerbytes.Length);

                fileStream.CopyTo(outputStream, 4096);

                outputStream.Write(boundarybytes, 0, boundarybytes.Length);

                PclExport.Instance.CloseStream(outputStream);
            }
        }

        public static string PostXmlToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: data.ToXml(), contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PostCsvToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Post, requestBody: data.ToCsv(), contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutXmlToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: data.ToXml(), contentType: MimeTypes.Xml, accept: MimeTypes.Xml,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }

        public static string PutCsvToUrl(this string urlString, object data,
            Action<HttpWebRequest> requestFilter = null, Action<HttpWebResponse> responseFilter = null)
        {
            return GetStringFromUrl(urlString, method: HttpMethods.Put, requestBody: data.ToCsv(), contentType: MimeTypes.Csv, accept: MimeTypes.Csv,
                requestFilter: requestFilter, responseFilter: responseFilter);
        }
    }

    //Allow Exceptions to Customize HTTP StatusCode and StatusDescription returned
    public interface IHasStatusCode
    {
        int StatusCode { get; }
    }

    public interface IHasStatusDescription
    {
        string StatusDescription { get; }
    }
}

namespace ServiceStack
{
    public static class MimeTypes
    {
        public static Dictionary<string, string> ExtensionMimeTypes = new Dictionary<string, string>();

        public const string Html = "text/html";
        public const string Xml = "application/xml";
        public const string XmlText = "text/xml";
        public const string Json = "application/json";
        public const string JsonText = "text/json";
        public const string Jsv = "application/jsv";
        public const string JsvText = "text/jsv";
        public const string Csv = "text/csv";
        public const string ProtoBuf = "application/x-protobuf";
        public const string JavaScript = "text/javascript";

        public const string FormUrlEncoded = "application/x-www-form-urlencoded";
        public const string MultiPartFormData = "multipart/form-data";
        public const string JsonReport = "text/jsonreport";
        public const string Soap11 = "text/xml; charset=utf-8";
        public const string Soap12 = "application/soap+xml";
        public const string Yaml = "application/yaml";
        public const string YamlText = "text/yaml";
        public const string PlainText = "text/plain";
        public const string MarkdownText = "text/markdown";
        public const string MsgPack = "application/x-msgpack";
        public const string Wire = "application/x-wire";
        public const string NetSerializer = "application/x-netserializer";

        public const string ImagePng = "image/png";
        public const string ImageGif = "image/gif";
        public const string ImageJpg = "image/jpeg";

        public const string Bson = "application/bson";
        public const string Binary = "application/octet-stream";
        public const string ServerSentEvents = "text/event-stream";

        public static string GetExtension(string mimeType)
        {
            switch (mimeType)
            {
                case ProtoBuf:
                    return ".pbuf";
            }

            var parts = mimeType.Split('/');
            if (parts.Length == 1) return "." + parts[0];
            if (parts.Length == 2) return "." + parts[1];

            throw new NotSupportedException("Unknown mimeType: " + mimeType);
        }

        public static string GetMimeType(string fileNameOrExt)
        {
            if (string.IsNullOrEmpty(fileNameOrExt))
                throw new ArgumentNullException("fileNameOrExt");

            var parts = fileNameOrExt.Split('.');
            var fileExt = parts[parts.Length - 1];

            string mimeType;
            if (ExtensionMimeTypes.TryGetValue(fileExt, out mimeType))
            {
                return mimeType;
            }

            switch (fileExt)
            {
                case "jpeg":
                case "gif":
                case "png":
                case "tiff":
                case "bmp":
                case "webp":
                    return "image/" + fileExt;

                case "jpg":
                    return "image/jpeg";

                case "tif":
                    return "image/tiff";

                case "svg":
                    return "image/svg+xml";

                case "htm":
                case "html":
                case "shtml":
                    return "text/html";

                case "js":
                    return "text/javascript";

                case "ts":
                    return "text/x.typescript";

                case "jsx":
                    return "text/jsx";

                case "csv":
                case "css":
                case "sgml":
                    return "text/" + fileExt;

                case "txt":
                    return "text/plain";

                case "wav":
                    return "audio/wav";

                case "mp3":
                    return "audio/mpeg3";

                case "mid":
                    return "audio/midi";

                case "qt":
                case "mov":
                    return "video/quicktime";

                case "mpg":
                    return "video/mpeg";

                case "avi":
                case "mp4":
                case "ogg":
                case "webm":
                    return "video/" + fileExt;

                case "ogv":
                    return "video/ogg";

                case "rtf":
                    return "application/" + fileExt;

                case "xls":
                    return "application/x-excel";

                case "doc":
                    return "application/msword";

                case "ppt":
                    return "application/powerpoint";

                case "gz":
                case "tgz":
                    return "application/x-compressed";

                case "eot":
                    return "application/vnd.ms-fontobject";

                case "ttf":
                    return "application/octet-stream";

                case "woff":
                    return "application/font-woff";
                case "woff2":
                    return "application/font-woff2";

                default:
                    return "application/" + fileExt;
            }
        }
    }

    public static class HttpHeaders
    {
        public const string XParamOverridePrefix = "X-Param-Override-";

        public const string XHttpMethodOverride = "X-Http-Method-Override";

        public const string XAutoBatchCompleted = "X-AutoBatch-Completed"; // How many requests were completed before first failure

        public const string XTag = "X-Tag";

        public const string XUserAuthId = "X-UAId";

        public const string XTrigger = "X-Trigger"; // Trigger Events on UserAgent

        public const string XForwardedFor = "X-Forwarded-For"; // IP Address

        public const string XForwardedPort = "X-Forwarded-Port";  // 80

        public const string XForwardedProtocol = "X-Forwarded-Proto"; // http or https

        public const string XRealIp = "X-Real-IP";

        public const string XLocation = "X-Location";

        public const string XStatus = "X-Status";

        public const string Referer = "Referer";

        public const string CacheControl = "Cache-Control";

        public const string IfModifiedSince = "If-Modified-Since";

        public const string IfUnmodifiedSince = "If-Unmodified-Since";

        public const string IfNoneMatch = "If-None-Match";

        public const string IfMatch = "If-Match";

        public const string LastModified = "Last-Modified";

        public const string Accept = "Accept";

        public const string AcceptEncoding = "Accept-Encoding";

        public const string ContentType = "Content-Type";

        public const string ContentEncoding = "Content-Encoding";

        public const string ContentLength = "Content-Length";

        public const string ContentDisposition = "Content-Disposition";

        public const string Location = "Location";

        public const string SetCookie = "Set-Cookie";

        public const string ETag = "ETag";

        public const string Age = "Age";

        public const string Expires = "Expires";

        public const string Vary = "Vary";

        public const string Authorization = "Authorization";

        public const string WwwAuthenticate = "WWW-Authenticate";

        public const string AllowOrigin = "Access-Control-Allow-Origin";

        public const string AllowMethods = "Access-Control-Allow-Methods";

        public const string AllowHeaders = "Access-Control-Allow-Headers";

        public const string AllowCredentials = "Access-Control-Allow-Credentials";

        public const string ExposeHeaders = "Access-Control-Expose-Headers";

        public const string AccessControlMaxAge = "Access-Control-Max-Age";

        public const string Origin = "Origin";

        public const string RequestMethod = "Access-Control-Request-Method";

        public const string RequestHeaders = "Access-Control-Request-Headers";

        public const string AcceptRanges = "Accept-Ranges";

        public const string ContentRange = "Content-Range";

        public const string Range = "Range";

        public const string SOAPAction = "SOAPAction";

        public const string Allow = "Allow";

        public const string AcceptCharset = "Accept-Charset";

        public const string AcceptLanguage = "Accept-Language";

        public const string Connection = "Connection";

        public const string Cookie = "Cookie";

        public const string ContentLanguage = "Content-Language";

        public const string Expect = "Expect";

        public const string Pragma = "Pragma";
        
        public const string ProxyAuthenticate = "Proxy-Authenticate";

        public const string ProxyAuthorization = "Proxy-Authorization";

        public const string ProxyConnection = "Proxy-Connection";

        public const string SetCookie2 = "Set-Cookie2";

        public const string TE = "TE";

        public const string Trailer = "Trailer";

        public const string TransferEncoding = "Transfer-Encoding";

        public const string Upgrade = "Upgrade";

        public const string Via = "Via";

        public const string Warning = "Warning";

        public const string Date = "Date";
        public const string Host = "Host";
        public const string UserAgent = "User-Agent";

        public static HashSet<string> RestrictedHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Accept,
            Connection,
            ContentLength,
            ContentType,
            Date,
            Expect,
            Host,
            IfModifiedSince,
            Range,
            Referer,
            TransferEncoding,
            UserAgent,
            ProxyConnection,
        };
    }

    public static class HttpMethods
    {
        static readonly string[] allVerbs = new[] {
            "OPTIONS", "GET", "HEAD", "POST", "PUT", "DELETE", "TRACE", "CONNECT", // RFC 2616
            "PROPFIND", "PROPPATCH", "MKCOL", "COPY", "MOVE", "LOCK", "UNLOCK",    // RFC 2518
            "VERSION-CONTROL", "REPORT", "CHECKOUT", "CHECKIN", "UNCHECKOUT",
            "MKWORKSPACE", "UPDATE", "LABEL", "MERGE", "BASELINE-CONTROL", "MKACTIVITY",  // RFC 3253
            "ORDERPATCH", // RFC 3648
            "ACL",        // RFC 3744
            "PATCH",      // https://datatracker.ietf.org/doc/draft-dusseault-http-patch/
            "SEARCH",     // https://datatracker.ietf.org/doc/draft-reschke-webdav-search/
            "BCOPY", "BDELETE", "BMOVE", "BPROPFIND", "BPROPPATCH", "NOTIFY",
            "POLL",  "SUBSCRIBE", "UNSUBSCRIBE" //MS Exchange WebDav: http://msdn.microsoft.com/en-us/library/aa142917.aspx
        };

        public static HashSet<string> AllVerbs = new HashSet<string>(allVerbs);

        public static bool HasVerb(string httpVerb)
        {
#if NETFX_CORE
            return allVerbs.Any(p => p.Equals(httpVerb.ToUpper()));
#else
            return AllVerbs.Contains(httpVerb.ToUpper());
#endif
        }

        public const string Get = "GET";
        public const string Put = "PUT";
        public const string Post = "POST";
        public const string Delete = "DELETE";
        public const string Options = "OPTIONS";
        public const string Head = "HEAD";
        public const string Patch = "PATCH";
    }

    public static class CompressionTypes
    {
        public static readonly string[] AllCompressionTypes = new[] { Deflate, GZip };

        public const string Default = Deflate;
        public const string Deflate = "deflate";
        public const string GZip = "gzip";

        public static bool IsValid(string compressionType)
        {
            return compressionType == Deflate || compressionType == GZip;
        }

        public static void AssertIsValid(string compressionType)
        {
            if (!IsValid(compressionType))
            {
                throw new NotSupportedException(compressionType
                    + " is not a supported compression type. Valid types: gzip, deflate.");
            }
        }

        public static string GetExtension(string compressionType)
        {
            switch (compressionType)
            {
                case Deflate:
                case GZip:
                    return "." + compressionType;
                default:
                    throw new NotSupportedException(
                        "Unknown compressionType: " + compressionType);
            }
        }
    }

    public static class HttpStatus
    {
        public static string GetStatusDescription(int statusCode)
        {
            if (statusCode >= 100 && statusCode < 600)
            {
                int i = statusCode / 100;
                int j = statusCode % 100;

                if (j < Descriptions[i].Length)
                    return Descriptions[i][j];
            }

            return string.Empty;
        }

        private static readonly string[][] Descriptions = new string[][]
        {
            null,
            new[]
            { 
                /* 100 */ "Continue",
                /* 101 */ "Switching Protocols",
                /* 102 */ "Processing"
            },
            new[]
            { 
                /* 200 */ "OK",
                /* 201 */ "Created",
                /* 202 */ "Accepted",
                /* 203 */ "Non-Authoritative Information",
                /* 204 */ "No Content",
                /* 205 */ "Reset Content",
                /* 206 */ "Partial Content",
                /* 207 */ "Multi-Status"
            },
            new[]
            { 
                /* 300 */ "Multiple Choices",
                /* 301 */ "Moved Permanently",
                /* 302 */ "Found",
                /* 303 */ "See Other",
                /* 304 */ "Not Modified",
                /* 305 */ "Use Proxy",
                /* 306 */ string.Empty,
                /* 307 */ "Temporary Redirect"
            },
            new[]
            { 
                /* 400 */ "Bad Request",
                /* 401 */ "Unauthorized",
                /* 402 */ "Payment Required",
                /* 403 */ "Forbidden",
                /* 404 */ "Not Found",
                /* 405 */ "Method Not Allowed",
                /* 406 */ "Not Acceptable",
                /* 407 */ "Proxy Authentication Required",
                /* 408 */ "Request Timeout",
                /* 409 */ "Conflict",
                /* 410 */ "Gone",
                /* 411 */ "Length Required",
                /* 412 */ "Precondition Failed",
                /* 413 */ "Request Entity Too Large",
                /* 414 */ "Request-Uri Too Long",
                /* 415 */ "Unsupported Media Type",
                /* 416 */ "Requested Range Not Satisfiable",
                /* 417 */ "Expectation Failed",
                /* 418 */ string.Empty,
                /* 419 */ string.Empty,
                /* 420 */ string.Empty,
                /* 421 */ string.Empty,
                /* 422 */ "Unprocessable Entity",
                /* 423 */ "Locked",
                /* 424 */ "Failed Dependency"
            },
            new[]
            { 
                /* 500 */ "Internal Server Error",
                /* 501 */ "Not Implemented",
                /* 502 */ "Bad Gateway",
                /* 503 */ "Service Unavailable",
                /* 504 */ "Gateway Timeout",
                /* 505 */ "Http Version Not Supported",
                /* 506 */ string.Empty,
                /* 507 */ "Insufficient Storage"
            }
        };
    }
}
