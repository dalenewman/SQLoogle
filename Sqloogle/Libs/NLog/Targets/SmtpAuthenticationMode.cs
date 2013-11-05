#region License
// /*
// See license included in this library folder.
// */
#endregion
#if !NET_CF && !SILVERLIGHT

namespace Sqloogle.Libs.NLog.Targets
{
    /// <summary>
    ///     SMTP authentication modes.
    /// </summary>
    public enum SmtpAuthenticationMode
    {
        /// <summary>
        ///     No authentication.
        /// </summary>
        None,

        /// <summary>
        ///     Basic - username and password.
        /// </summary>
        Basic,

        /// <summary>
        ///     NTLM Authentication.
        /// </summary>
        Ntlm,
    }
}

#endif