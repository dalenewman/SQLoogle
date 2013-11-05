#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;

#if !NET_CF && !SILVERLIGHT

namespace Sqloogle.Libs.NLog.Internal
{
    /// <summary>
    ///     Various helper methods for accessing state of ASP application.
    /// </summary>
    internal class AspHelper
    {
        private static Guid IID_IObjectContext = new Guid("51372ae0-cae7-11cf-be81-00aa00a2fa25");

        private AspHelper()
        {
        }

        public static ISessionObject GetSessionObject()
        {
            ISessionObject session = null;

            IObjectContext obj;
            if (0 == NativeMethods.CoGetObjectContext(ref IID_IObjectContext, out obj))
            {
                var prop = (IGetContextProperties) obj;
                if (prop != null)
                {
                    session = (ISessionObject) prop.GetProperty("Session");
                    Marshal.ReleaseComObject(prop);
                }
                Marshal.ReleaseComObject(obj);
            }
            return session;
        }

        public static IApplicationObject GetApplicationObject()
        {
            IApplicationObject app = null;

            IObjectContext obj;
            if (0 == NativeMethods.CoGetObjectContext(ref IID_IObjectContext, out obj))
            {
                var prop = (IGetContextProperties) obj;
                if (prop != null)
                {
                    app = (IApplicationObject) prop.GetProperty("Application");
                    Marshal.ReleaseComObject(prop);
                }
                Marshal.ReleaseComObject(obj);
            }
            return app;
        }

        public static IRequest GetRequestObject()
        {
            IRequest request = null;

            IObjectContext obj;
            if (0 == NativeMethods.CoGetObjectContext(ref IID_IObjectContext, out obj))
            {
                var prop = (IGetContextProperties) obj;
                if (prop != null)
                {
                    request = (IRequest) prop.GetProperty("Request");
                    Marshal.ReleaseComObject(prop);
                }
                Marshal.ReleaseComObject(obj);
            }
            return request;
        }

        public static IResponse GetResponseObject()
        {
            IResponse Response = null;

            IObjectContext obj;
            if (0 == NativeMethods.CoGetObjectContext(ref IID_IObjectContext, out obj))
            {
                var prop = (IGetContextProperties) obj;
                if (prop != null)
                {
                    Response = (IResponse) prop.GetProperty("Response");
                    Marshal.ReleaseComObject(prop);
                }
                Marshal.ReleaseComObject(obj);
            }

            return Response;
        }

        public static object GetComDefaultProperty(object o)
        {
            if (o == null)
                return null;
            return o.GetType().InvokeMember(string.Empty, BindingFlags.GetProperty, null, o, new object[] {}, CultureInfo.InvariantCulture);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A866-11cf-83AE-10A0C90C2BD8")]
        public interface IApplicationObject
        {
            object GetValue(string name);
            void PutValue(string name, object val);
            // remaining methods removed
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020400-0000-0000-C000-000000000046")]
        public interface IDispatch
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("51372af4-cae7-11cf-be81-00aa00a2fa25")]
        public interface IGetContextProperties
        {
            int Count();
            object GetProperty(string name);
            // EnumNames omitted
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("51372ae0-cae7-11cf-be81-00aa00a2fa25")]
        public interface IObjectContext
        {
            // members not important
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("71EAF260-0CE0-11D0-A53E-00A0C90C2091")]
        public interface IReadCookie
        {
            void GetItem(object key, out object val);
            object HasKeys();
            void GetNewEnum();
            void GetCount(out int count);
            object GetKey(object key);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A861-11cf-93AE-00A0C90C2BD8")]
        public interface IRequest
        {
            IDispatch GetItem(string name);
            IRequestDictionary GetQueryString();
            IRequestDictionary GetForm();
            IRequestDictionary GetBody();
            IRequestDictionary GetServerVariables();
            IRequestDictionary GetClientCertificates();
            IRequestDictionary GetCookies();
            int GetTotalBytes();
            void BinaryRead(); // ignored
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A85F-11df-83AE-00A0C90C2BD8")]
        public interface IRequestDictionary
        {
            object GetItem(object var);
            object NewEnum();
            int GetCount();
            object Key(object varKey);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A864-11cf-83BE-00A0C90C2BD8")]
        public interface IResponse
        {
            void GetBuffer(); // placeholder
            void PutBuffer(); // placeholder
            void GetContentType(); // placeholder
            void PutContentType(); // placeholder
            void GetExpires(); // placeholder
            void PutExpires(); // placeholder
            void GetExpiresAbsolute(); // placeholder
            void PutExpiresAbsolute(); // placeholder
            void GetCookies();
            void GetStatus();
            void PutStatus();
            void Add();
            void AddHeader();
            void AppendToLog(); // anybody uses this?
            void BinaryWrite();
            void Clear();
            void End();
            void Flush();
            void Redirect();
            void Write(object text);

            // other members omitted
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A865-11cf-83AF-00A0C90C2BD8")]
        public interface ISessionObject
        {
            string GetSessionID();
            object GetValue(string name);
            void PutValue(string name, object val);
            int GetTimeout();
            void PutTimeout(int t);
            void Abandon();
            int GetCodePage();
            void PutCodePage(int cp);
            int GetLCID();
            void PutLCID();
            // GetStaticObjects
            // GetContents
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("D97A6DA0-A85D-11cf-83AE-00A0C90C2BD8")]
        public interface IStringList
        {
            object GetItem(object key);
            int GetCount();
            object NewEnum();
        }
    }
}

#endif