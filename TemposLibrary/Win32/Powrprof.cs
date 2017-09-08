using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace TemposLibrary.Win32
{
    public class Powrprof
    {
        /// <summary>
        /// Suspends the system by shutting power down. Depending on the Hibernate parameter, the system either enters a suspend (sleep) state or hibernation (S4).
        /// </summary>
        /// <param name="hibernate">If this parameter is TRUE, the system hibernates. If the parameter is FALSE, the system is suspended.</param>
        /// <param name="forceCritical">Windows Server 2003, Windows XP, and Windows 2000:  If this parameter is TRUE, 
        /// the system suspends operation immediately; if it is FALSE, the system broadcasts a PBT_APMQUERYSUSPEND event to each 
        /// application to request permission to suspend operation.</param>
        /// <param name="disableWakeEvent">If this parameter is TRUE, the system disables all wake events. If the parameter is FALSE, any system wake events remain enabled.</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        /// <remarks>See http://msdn.microsoft.com/en-us/library/aa373201(VS.85).aspx</remarks>
        [DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);
    }
}
