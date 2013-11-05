#region License
// /*
// See license included in this library folder.
// */
#endregion
#if !NET_CF && !SILVERLIGHT

namespace Sqloogle.Libs.NLog.Targets.Wrappers
{
    /// <summary>
    ///     Logon provider.
    /// </summary>
    public enum LogOnProviderType
    {
        /// <summary>
        ///     Use the standard logon provider for the system.
        /// </summary>
        /// <remarks>
        ///     The default security provider is negotiate, unless you pass NULL for the domain name and the user name
        ///     is not in UPN format. In this case, the default provider is NTLM.
        ///     NOTE: Windows 2000/NT:   The default security provider is NTLM.
        /// </remarks>
        Default = 0,
    }
}

#endif