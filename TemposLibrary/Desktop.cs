using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using TemposLibrary.Win32;

namespace TemposLibrary
{
	/// <summary>
	/// Access to the windows Desktop API in user32.dll
	/// </summary>
	public class Desktop : IDisposable
	{
        #region Licensed Access Only
        static Desktop()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(Desktop).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use TemposLibrary.dll");
            }
#endif
        }
        #endregion

        #region Constants
		private const int AccessRights = (int)(
            WinBase.ACCESS_MASK.DESKTOP_JOURNALRECORD |
            WinBase.ACCESS_MASK.DESKTOP_JOURNALPLAYBACK |
            WinBase.ACCESS_MASK.DESKTOP_CREATEWINDOW |
            WinBase.ACCESS_MASK.DESKTOP_ENUMERATE |
            WinBase.ACCESS_MASK.DESKTOP_WRITEOBJECTS |
            WinBase.ACCESS_MASK.DESKTOP_SWITCHDESKTOP |
            WinBase.ACCESS_MASK.DESKTOP_CREATEMENU |
            WinBase.ACCESS_MASK.DESKTOP_HOOKCONTROL |
            WinBase.ACCESS_MASK.DESKTOP_READOBJECTS);
		#endregion

		#region Private Variables
		private IntPtr _desktopHandle;
		private string _desktopName;
		private static StringCollection _desktopNames;
		private bool _isDisposed;
        private readonly List<IntPtr> _enumWindows = new List<IntPtr>();
        #endregion

		#region Public Properties
		/// <summary>
		/// Gets if a desktop is open
		/// </summary>
		public bool IsOpen
		{
			get
			{
				return (_desktopHandle != IntPtr.Zero);
			}
		}

		/// <summary>
		/// Gets the name of the desktop, returns null if no desktop is open
		/// </summary>
		public string DesktopName
		{
			get
			{
				return _desktopName;
			}
		}

		/// <summary>
		/// Gets a handle to the desktop, IntPtr.Zero if no desktop open
		/// </summary>
		public IntPtr DesktopHandle
		{
			get
			{
				return _desktopHandle;
			}
		}

		/// <summary>
		/// Opens the default desktop.
		/// </summary>
		public static readonly Desktop Default = OpenDefaultDesktop();

		/// <summary>
		/// Opens the desktop the user if viewing.
		/// </summary>
		public static readonly Desktop Input = OpenInputDesktop();
		#endregion

		#region Construction/Destruction
		/// <summary>
		/// Creates a new Desktop object.
		/// </summary>
		public Desktop()
		{
			// init variables.
			_desktopHandle = IntPtr.Zero;
			_desktopName = String.Empty;
			_isDisposed = false;
		}

		// constructor is private to prevent invalid handles being passed to it.
		private Desktop(IntPtr desktop)
		{
			// init variables.
            _desktopHandle = desktop;
			_desktopName = GetDesktopName(desktop);
			_isDisposed = false;
		}

		~Desktop()
		{
			// clean up, close the desktop.
			Close();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates a new desktop.  If a handle is open, it will be closed.
		/// </summary>
		/// <param name="name">The name of the new desktop.  Must be unique, and is case sensitive.</param>
		/// <returns>True if desktop was successfully created, otherwise false.</returns>
		public bool Create(string name)
		{
			// make sure object isnt disposed
			CheckDisposed();

			// close the open desktop
			if (_desktopHandle != IntPtr.Zero)
			{
				// attempt to close the desktop
				if (!Close()) return false;
			}

            User32.OpenInputDesktop(0, false, WinBase.ACCESS_MASK.DESKTOP_SWITCHDESKTOP);
	
			// make sure desktop doesnt already exist
			if (Exists(name))
			{
				// it exists, so open it
				return Open(name);
			}

			// attempt to create desktop
			_desktopHandle = User32.CreateDesktop(name, IntPtr.Zero,
                IntPtr.Zero, 0, AccessRights, IntPtr.Zero);

			_desktopName = name;

			return (_desktopHandle != IntPtr.Zero);
		}
		
		/// <summary>
		/// Closes the handle to a desktop
		/// </summary>
		/// <returns>True if an open handle was successfully closed.</returns>
		public bool Close()
		{
			// make sure object isnt disposed
			CheckDisposed();
            
			// check there is a desktop open
			if (_desktopHandle != IntPtr.Zero)
			{
				// close the desktop.
				bool result = User32.CloseDesktop(_desktopHandle);

				if (result)
				{
					_desktopHandle = IntPtr.Zero;
					_desktopName = String.Empty;
				}

				return result;
			}

			// no desktop was open, so desktop is closed
			return true;
		}

		/// <summary>
		/// Opens a desktop
		/// </summary>
		/// <param name="name">The name of the desktop to open.</param>
		/// <returns>True if the desktop was successfully opened.</returns>
		public bool Open(string name)
		{
			// make sure object isnt disposed
			CheckDisposed();

			// close the open desktop.
			if (_desktopHandle != IntPtr.Zero)
			{
				// attempt to close the desktop
				if (!Close()) return false;
			}

			// open the desktop.
			_desktopHandle = User32.OpenDesktop(name, 0, true, AccessRights);

			// something went wrong
			if (_desktopHandle == IntPtr.Zero) return false;

			_desktopName = name;

			return true;
		}

		/// <summary>
		/// Opens the current input desktop
		/// </summary>
		/// <returns>True if the desktop was succesfully opened.</returns>
		public bool OpenInput()
		{
			// make sure object isnt disposed
			CheckDisposed();

			// close the open desktop
			if (_desktopHandle != IntPtr.Zero)
			{
				// attempt to close the desktop
				if (!Close()) return false;
			}

			// open the desktop.
			_desktopHandle = User32.OpenInputDesktop(0, true, AccessRights);

			// OpenInputDesktop failed.
			if (_desktopHandle == IntPtr.Zero) return false;

            // set _desktopName to the return value of GetDesktopName
			_desktopName = GetDesktopName(_desktopHandle);

			return true;
		}

        /// <summary>
        /// Get all the processes running on this desktop
        /// </summary>
        public Process[] GetProcesses()
        {
            var processes = new List<Process>();
            IEnumerable<IntPtr> windows = GetDesktopWindows();
            foreach (IntPtr window in windows)
            {
                uint processId;
                User32.GetWindowThreadProcessId(window, out processId);
                processes.Add(Process.GetProcessById((int)processId));
            }
            return processes.ToArray();
        }

        /// <summary>
		/// Switches input to the currently opened desktop
		/// </summary>
		/// <returns>True if desktops were successfully switched</returns>
		public bool Show()
		{
			// make sure object isnt disposed
			CheckDisposed();

			// If a desktop handle exists, switch to that desktop
			return _desktopHandle != IntPtr.Zero && User32.SwitchDesktop(_desktopHandle);
		}

		/// <summary>
		/// Creates a new process in a desktop
		/// </summary>
		public Process CreateProcess(string path)
		{
			// make sure object isnt disposed
			CheckDisposed();

			// make sure a desktop is open
			if (!IsOpen) return null;

			// set startup parameters
			var si = new WinBase.STARTUPINFO();
			si.cb = Marshal.SizeOf(si);
			si.lpDesktop = _desktopName;

			var pi = new WinBase.PROCESS_INFORMATION();

			// start the process.
			bool result = Kernel32.CreateProcess(null, "\"" + path + "\"", IntPtr.Zero, IntPtr.Zero, true, WinBase.NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref si, ref pi);

			// return results
			return !result ? null : Process.GetProcessById(pi.dwProcessId);

			// Get the process
		}

        /// <summary>
        /// Creates a new process in a desktop
        /// </summary>
        public Process CreateProcess(string path, string arguments)
        {
            if (arguments == null)
                return CreateProcess(path);

            // make sure object isnt disposed
            CheckDisposed();

            // make sure a desktop is open
            if (!IsOpen) return null;

            // set startup parameters
            var si = new WinBase.STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = _desktopName;

            var pi = new WinBase.PROCESS_INFORMATION();

            // start the process.
            bool result = Kernel32.CreateProcess(null, "\"" + path + "\" " + arguments, IntPtr.Zero, IntPtr.Zero, true, WinBase.NORMAL_PRIORITY_CLASS, IntPtr.Zero, null, ref si, ref pi);

            // return results
            return !result ? null : Process.GetProcessById(pi.dwProcessId);

            // Get the process
        }

        /// <summary>
        /// Enumerates the windows on a desktop
        /// </summary>
        private IEnumerable<IntPtr> GetDesktopWindows()
        {
            // make sure object isnt disposed.
            CheckDisposed();

            // make sure a desktop is open.
            if (!IsOpen) return null;

            // init the arraylist.
            _enumWindows.Clear();

            // get windows.
            bool result = User32.EnumDesktopWindows(_desktopHandle, DesktopWindowsProc, IntPtr.Zero);

            // return results
            return !result ? null : _enumWindows;
        }

        private bool DesktopWindowsProc(IntPtr wndHandle, IntPtr lParam)
        {
            // add window handle to colleciton.
            _enumWindows.Add(wndHandle);

            return true;
        }
        #endregion

		#region Static Methods
		/// <summary>
		/// Enumerates all of the desktops.
		/// </summary>
		public static string[] GetDesktops()
		{
			// attempt to enum desktops.
			IntPtr windowStation = User32.GetProcessWindowStation();

			// check we got a valid handle.
			if (windowStation == IntPtr.Zero) return new string[0];

			string[] desktops;

			// lock the object. thread safety and all.
			lock(_desktopNames = new StringCollection())
			{
				bool result = User32.EnumDesktops(windowStation, DesktopProc, IntPtr.Zero);

				// something went wrong.
				if (!result) return new string[0];

				// turn the collection into an array.
				desktops = new string[_desktopNames.Count];
				for(int i = 0; i < desktops.Length; i++) desktops[i] = _desktopNames[i];
			}

			return desktops;
		}

		private static bool DesktopProc(string lpszDesktop, IntPtr lParam)
		{
			// add the desktop to the collection.
			_desktopNames.Add(lpszDesktop);

			return true;
		}

		/// <summary>
		/// Switches to the specified desktop.
		/// </summary>
		/// <param name="name">Name of desktop to switch input to.</param>
		/// <returns>True if desktops were successfully switched.</returns>
		public static bool Show(string name)
		{
			// attempt to open desktop.
			bool result;

			using (var d = new Desktop())
			{
				result = d.Open(name);

				// something went wrong.
				if (!result) return false;

				// attempt to switch desktops.
				result = d.Show();
			}

			return result;
		}

		/// <summary>
		/// Gets the desktop of the calling thread.
		/// </summary>
		/// <returns>Returns a Desktop object for the valling thread.</returns>
		public static Desktop GetCurrent()
		{
			// get the desktop.
			return new Desktop(User32.GetThreadDesktop(Thread.CurrentThread.ManagedThreadId));
		}

		/// <summary>
		/// Sets the desktop of the calling thread.
		/// NOTE: Function will fail if thread has hooks or windows in the current desktop.
		/// </summary>
		/// <param name="desktop">Desktop to put the thread in.</param>
		/// <returns>True if the threads desktop was successfully changed.</returns>
		public static bool SetCurrent(Desktop desktop)
		{
		    return desktop.IsOpen && User32.SetThreadDesktop(desktop.DesktopHandle);
		}

	    /// <summary>
		/// Opens a desktop.
		/// </summary>
		/// <param name="name">The name of the desktop to open.</param>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenDesktop(string name)
		{
			// open the desktop.
			var desktop = new Desktop();
			bool result = desktop.Open(name);

			// return result
			return !result ? null : desktop;
		}

        /// <summary>
        /// Get the name of the current desktop
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopName()
        {
            string name = null;
            IntPtr hDesktop = User32.OpenInputDesktop(0, false, WinBase.ACCESS_MASK.DESKTOP_READOBJECTS);
            if (hDesktop != IntPtr.Zero)
            {
                uint size = 256;
                var byteName = new byte[size];
                User32.GetUserObjectInformation(hDesktop, WinBase.USER_OBJECT_INFORMATION.Name,
                    byteName, size, out size);
                byteName[size] = 0;
                name = Encoding.UTF8.GetString(byteName, 0, (int)size);
                Kernel32.CloseHandle(hDesktop);
            }
            return name;
        }

		/// <summary>
		/// Opens the current input desktop.
		/// </summary>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenInputDesktop()
		{
			// open the desktop.
			var desktop = new Desktop();
			bool result = desktop.OpenInput();

            // return result
            return !result ? null : desktop;
		}

		/// <summary>
		/// Opens the default desktop.
		/// </summary>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop OpenDefaultDesktop()
		{
			// opens the default desktop.
			return OpenDesktop("Default");
		}

		/// <summary>
		/// Creates a new desktop.
		/// </summary>
		/// <param name="name">The name of the desktop to create.  Names are case sensitive.</param>
		/// <returns>If successful, a Desktop object, otherwise, null.</returns>
		public static Desktop CreateDesktop(string name)
		{
			// open the desktop.
			var desktop = new Desktop();
			bool result = desktop.Create(name);

            // return result
            return !result ? null : desktop;
		}

		/// <summary>
		/// Gets the name of a given desktop.
		/// </summary>
		/// <param name="desktop">Desktop object whos name is to be found.</param>
		/// <returns>If successful, the desktop name, otherwise, null.</returns>
		public static string GetDesktopName(Desktop desktop)
		{
			// get name.
			if (desktop.IsOpen) return null;

			return GetDesktopName(desktop.DesktopHandle);
		}

		/// <summary>
		/// Gets the name of a desktop from a desktop handle.
		/// </summary>
		/// <param name="desktopHandle"></param>
		/// <returns>If successful, the desktop name, otherwise, null.</returns>
		public static string GetDesktopName(IntPtr desktopHandle)
		{
			// check its not a null pointer.
			// null pointers wont work.
			if (desktopHandle == IntPtr.Zero) return null;

			// get the length of the name.
			int needed = 0;
		    User32.GetUserObjectInformation(desktopHandle,
                (int)WinBase.USER_OBJECT_INFORMATION.Name, IntPtr.Zero, 0, ref needed);

			// get the name.
			IntPtr ptr = Marshal.AllocHGlobal(needed);
            bool result = User32.GetUserObjectInformation(desktopHandle,
                (int)WinBase.USER_OBJECT_INFORMATION.Name, ptr, needed, ref needed);
			string name = Marshal.PtrToStringAnsi(ptr);
			Marshal.FreeHGlobal(ptr);

            // return result
            return !result ? null : name;
		}

		/// <summary>
		/// Checks if the specified desktop exists (using a case sensitive search).
		/// </summary>
		/// <param name="name">The name of the desktop.</param>
		/// <returns>True if the desktop exists, otherwise false.</returns>
		public static bool Exists(string name)
		{
			return Exists(name, false);
		}

		/// <summary>
		/// Checks if the specified desktop exists.
		/// </summary>
		/// <param name="name">The name of the desktop.</param>
		/// <param name="caseInsensitive">If the search is case insensitive.</param>
		/// <returns>True if the desktop exists, otherwise false.</returns>
		public static bool Exists(string name, bool caseInsensitive)
		{
			// Get desktops.
			string[] desktops = GetDesktops();

			// return true if desktop exists.
			foreach(string desktop in desktops)
			{
			    if (caseInsensitive && (desktop.ToLower() == name.ToLower()))                     
                    return true;
			    if (desktop == name) return true;
			}

		    return false;
		}

		/// <summary>
		/// Gets an array of all the processes running on the Input desktop.
		/// </summary>
		/// <returns>An array of the processes.</returns>
		public static Process[] GetInputProcesses()
		{
			// get all processes.
			Process[] processes = Process.GetProcesses();

		    // get the current desktop name.
			string currentDesktop = GetDesktopName(Input.DesktopHandle);

			// cycle through the processes.
		    return processes.Where(process => process.Threads.Cast<ProcessThread>()
                .Any(pt => GetDesktopName(User32.GetThreadDesktop(pt.Id)) == currentDesktop)).ToArray();
		}
		#endregion

		#region IDisposable
		/// <summary>
		/// Dispose Object.
		/// </summary>
		public void Dispose()
		{
			// dispose
			Dispose(true);

			// suppress finalisation
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose Object.
		/// </summary>
		/// <param name="disposing">True to dispose managed resources.</param>
		public virtual void Dispose(bool disposing)
		{
			if (!_isDisposed)
			{
				// dispose of managed resources,
				// close handles
				Close();
			}

			_isDisposed = true;
		}

		private void CheckDisposed()
		{
			// check if disposed
			if (_isDisposed)
			{
				// object disposed, throw exception
				throw new ObjectDisposedException("");
			}
		}
		#endregion

		#region Overrides
		/// <summary>
		/// Gets the desktop name.
		/// </summary>
		/// <returns>The name of the desktop</returns>
		public override string ToString()
		{
			// return the desktop name.
			return _desktopName;
		}
		#endregion
    }
}