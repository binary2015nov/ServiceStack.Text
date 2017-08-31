//Copyright (c) ServiceStack, Inc. All Rights Reserved.
//License: https://raw.github.com/ServiceStack/ServiceStack/master/license.txt

#if SL5

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceStack
{
    public class Sl5PclExport : PclExport
    {
        public static Sl5PclExport Provider = new Sl5PclExport();

        public bool EmulateHttpViaPost { get; set; }

        public Sl5PclExport()
        {
            this.PlatformName = PlatformNames.Silverlight5;
        }

        public static PclExport Configure()
        {
            Configure(Provider);
            return Provider;
        }

        public override string ReadAllText(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public override Assembly LoadAssembly(string assemblyPath)
        {
            var sri = System.Windows.Application.GetResourceStream(new Uri(assemblyPath, UriKind.Relative));
            var myPart = new System.Windows.AssemblyPart();
            var assembly = myPart.Load(sri.Stream);
            return assembly;
        }

        public override Assembly[] GetAllAssemblies()
        {
//TODO: Workout how to fix broken CoreCLR SL5 build that uses dynamic
#if !(SL5 && CORECLR) && !NO_DYNAMIC
            return ((dynamic)AppDomain.CurrentDomain).GetAssemblies() as Assembly[];
#else
            return new Assembly[0];
#endif
        }

        public override Type GetGenericCollectionType(Type type)
        {
            return type.GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        public override HttpWebRequest CreateWebRequest(string urlString)
        {
            var creator = EmulateHttpViaPost
                ? System.Net.Browser.WebRequestCreator.BrowserHttp
                : System.Net.Browser.WebRequestCreator.ClientHttp;

            var webReq = (HttpWebRequest) creator.Create(new Uri(urlString));

            //Methods others than GET and POST are only supported by Client request creator, see
            //http://msdn.microsoft.com/en-us/library/cc838250(v=vs.95).aspx
            if (webReq.GetType().Name == "BrowserHttpWebRequest" && webReq.Method != "GET" && webReq.Method != "POST")
            {
                webReq.Headers[HttpHeaders.XHttpMethodOverride] = webReq.Method;
                webReq.Method = "POST";
            }
            return webReq;
        }

        public override Stream GetRequestStream(WebRequest webReq)
        {
            var async = GetRequestStreamAsync(webReq);
            async.Wait();
            return async.Result;
        }

        public static Task<Stream> GetRequestStreamAsync(WebRequest webReq)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                webReq.BeginGetRequestStream(iar =>
                {
                    try
                    {
                        var response = webReq.EndGetRequestStream(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        public override WebResponse GetResponse(WebRequest webReq)
        {
            var task = GetResponseAsync(webReq);
            task.Wait();
            return task.Result;
        }

        public static Task<HttpWebResponse> GetResponseAsync(WebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse) request.EndGetResponse(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }
    }

    // Stopwatch shim for Silverlight
    public sealed class Stopwatch
    {
        private long startTick;
        private long elapsed;
        private bool isRunning;

        public static Stopwatch StartNew()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            return sw;
        }

        public Stopwatch() {}

        public void Reset()
        {
            elapsed = 0;
            isRunning = false;
            startTick = 0;
        }

        public void Start()
        {
            if (!isRunning)
            {
                startTick = GetCurrentTicks();
                isRunning = true;
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                elapsed += GetCurrentTicks() - startTick;
                isRunning = false;
            }
        }

        public bool IsRunning
        {
            get { return isRunning; }
        }

        public TimeSpan Elapsed
        {
            get { return TimeSpan.FromMilliseconds(ElapsedMilliseconds); }
        }

        public long ElapsedMilliseconds
        {
            get { return GetCurrentElapsedTicks() / TimeSpan.TicksPerMillisecond; }
        }

        public long ElapsedTicks
        {
            get { return GetCurrentElapsedTicks(); }
        }

        private long GetCurrentElapsedTicks()
        {
            return (long) (this.elapsed + (IsRunning ? (GetCurrentTicks() - startTick) : 0));
        }

        private long GetCurrentTicks()
        {
            return Environment.TickCount * TimeSpan.TicksPerMillisecond;
        }

        public static long GetTimestamp() 
        {
            return DateTime.UtcNow.Ticks;
        }
    }
}

#endif
