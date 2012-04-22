// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConsoleHelper.cs" company="">
//   
// </copyright>
// <summary>
//   The console helper.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace AjaxServer.utils
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using System.Security;

    /// <summary>
    ///   The console helper.
    /// </summary>
    public class ConsoleHelper
    {
        #region Constructors and Destructors

        /// <summary>
        ///   Finalizes an instance of the <see cref="ConsoleHelper" /> class.
        /// </summary>
        ~ConsoleHelper()
        {
            Destroy();
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///   The create.
        /// </summary>
        /// <returns> The create. </returns>
        public static int Create()
        {
            if (AllocConsole())
            {
                return 0;
            }
            else
            {
                return Marshal.GetLastWin32Error();
            }
        }

        /// <summary>
        ///   The destroy.
        /// </summary>
        /// <returns> The destroy. </returns>
        public static int Destroy()
        {
            if (FreeConsole())
            {
                return 0;
            }
            else
            {
                return Marshal.GetLastWin32Error();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///   The alloc console.
        /// </summary>
        /// <returns> The alloc console. </returns>
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        /// <summary>
        ///   The free console.
        /// </summary>
        /// <returns> The free console. </returns>
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage")]
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeConsole();

        #endregion
    }
}