using System;

namespace ServiceStack
{
    abstract class Tracer
    {
        public static ITracer Default = new NullTracer();
    }

    class NullTracer : ITracer
    {
        public void WriteDebug(string error) { }

        public void WriteDebug(string format, params object[] args) { }

        public void WriteWarning(string warning) { }

        public void WriteWarning(string format, params object[] args) { }

        public void WriteError(Exception ex) { }

        public void WriteError(string error) { }

        public void WriteError(string format, params object[] args) { }

    }

    class ConsoleTracer : ITracer
    {
        public void WriteDebug(string error)
        {
            PclExport.Instance.WriteLine(error);
        }

        public void WriteDebug(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }

        public void WriteWarning(string warning)
        {
            PclExport.Instance.WriteLine(warning);
        }

        public void WriteWarning(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }

        public void WriteError(Exception ex)
        {
            PclExport.Instance.WriteLine(ex.ToString());
        }

        public void WriteError(string error)
        {
            PclExport.Instance.WriteLine(error);
        }

        public void WriteError(string format, params object[] args)
        {
            PclExport.Instance.WriteLine(format, args);
        }
    }

    static class TracerExceptions
    {
        public static TException Trace<TException>(this TException ex) 
            where TException : Exception
        {
            Tracer.Default.WriteError(ex);
            return ex;
        }
    }
}