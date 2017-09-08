using System;
using System.Linq;
using System.Reflection;
using PosControls;
using PosModels;
using TemPOS.Commands;
using TemPOS.Networking;
using TemPOS.Types;

namespace TemPOS.Managers
{
    public static class RegisterManager
    {
        #region Licensed Access Only
        static RegisterManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(RegisterManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to link to this application");
            }
#endif
        }
        #endregion

        private static RegisterDrawer activeRegisterDrawer = null;

        [Obfuscation(Exclude = true)]
        public static event EventHandler ActiveRegisterDrawerChanged = null;

        public static RegisterDrawer ActiveRegisterDrawer
        {
            get { return activeRegisterDrawer; }
            set
            {
                activeRegisterDrawer = value;
                if (ActiveRegisterDrawerChanged != null)
                    ActiveRegisterDrawerChanged.Invoke(value, new EventArgs());
            }
        }

        public static int? RegisterId
        {
            get
            {
                return NetworkTools.GetLastLanByte();
            }
        }

        public static bool OpenRegisterExists
        {
            get
            {
                if (RegisterId == null)
                    return false;
                return(RegisterDrawer.GetOpen(RegisterId.Value)
                    .ToArray<RegisterDrawer>().Length > 0);
            }
        }

        public static void CheckAvailable(out bool drawer1Available, out bool drawer2Available)
        {
            if (RegisterId == null)
            {
                drawer1Available = false;
                drawer2Available = false;
                return;
            }
            drawer1Available = true;
            drawer2Available = true;
            foreach (RegisterDrawer drawer in RegisterDrawer.GetOpen(RegisterId.Value))
            {
                if (drawer.RegisterSubId == 0)
                    drawer1Available = false;
                if (drawer.RegisterSubId == 1)
                    drawer2Available = false;
            }
        }

        public static void SetActiveRegisterDrawer()
        {
            ActiveRegisterDrawer = null;
            if (RegisterId == null)
                return;
            foreach (RegisterDrawer drawer in RegisterDrawer.GetOpen(RegisterId.Value))
            {
                if (drawer.EmployeeId == SessionManager.ActiveEmployee.Id)
                {
                    ActiveRegisterDrawer = drawer;
                    return;
                }
            }
        }

        public static void CloseActiveRegisterDrawer()
        {
            if (ActiveRegisterDrawer != null)
            {
                if (ActiveRegisterDrawer.EndAmount == null)
                    ActiveRegisterDrawer.Close();
                ActiveRegisterDrawer = null;
            }
        }

        public static void FloatActiveRegisterDrawer()
        {
            if (ActiveRegisterDrawer != null)
            {
                RegisterMove.Add(ActiveRegisterDrawer.Id, 
                    ActiveRegisterDrawer.RegisterId.Value, 
                    ActiveRegisterDrawer.RegisterSubId.Value);
                ActiveRegisterDrawer.UnsetRegisterId();
                ActiveRegisterDrawer = null;
            }
        }

        public static bool DockRegisterDrawer(RegisterDrawer registerDrawer, int registerSubId)
        {
            // No network card or bad network setup
            if (RegisterId == null)
                return false;
            RegisterMove registerMove =
                RegisterMove.GetOpen(SessionManager.ActiveEmployee.Id);
            registerMove.SetTarget(RegisterId.Value, registerSubId);
            ActiveRegisterDrawer = registerDrawer;
            ActiveRegisterDrawer.SetRegisterId(RegisterId.Value, registerSubId);
            return true;
        }

        public static void StartRegister()
        {
            // No network card or bad network setup
            if (RegisterId == null)
            {
                PosDialogWindow.ShowDialog(MainWindow.Singleton,
                    Strings.CanNotDetermineTheRegisterIDCheckNetworkSetup, Strings.Error);
                return;
            }
#if !DEMO
            // Are and physical register drawers even setup?
            if ((DeviceManager.ActiveCashDrawer1 == null) &&
                (DeviceManager.ActiveCashDrawer2 == null))
            {
                PosDialogWindow.ShowDialog(MainWindow.Singleton,
                    Strings.ThereAreNoPhysicalCashRegisterDrawersSetup, Strings.Error);
                return;
            }

            // Are both register drawers assigned to other employees already?
            bool drawer1Available, drawer2Available;
            RegisterManager.CheckAvailable(out drawer1Available, out drawer2Available);
            if (!drawer1Available && !drawer2Available)
            {
                bool bothDrawersSetup = ((DeviceManager.ActiveCashDrawer1 != null) &&
                    (DeviceManager.ActiveCashDrawer2 != null));
                PosDialogWindow.ShowDialog(MainWindow.Singleton,
                    (bothDrawersSetup ? Strings.BothDrawersAre : Strings.TheRegisterDrawerIs) +
                    Strings.AlreadyBeingUsedBy +
                    (bothDrawersSetup ? Strings.OtherEmployees : Strings.AnotherEmployee),
                    Strings.Error);
                return;
            }
#endif
            PosDialogWindow window = RegisterDrawerStartControl.CreateInDefaultWindow();
            RegisterDrawerStartControl control = window.DockedControl as RegisterDrawerStartControl;

            PosDialogWindow.ShowPosDialogWindow(MainWindow.Singleton, window);
            if (!window.ClosedByUser &&
                (control.StartingAmount != null))
            {
                RegisterManager.ActiveRegisterDrawer =
                    RegisterDrawer.Add(RegisterId.Value,
                    0, // TODO: Set RegisterSubId to the physical register drawer used
                    SessionManager.ActiveEmployee.Id, control.StartingAmount.Value);

                // Resetup command buttons
                OrderEntryCommands.SetupNoOrderCommands();

                // Open the register so the drawer can be placed in it
                OpenCashDrawer();
            }
        }

        public static void OpenCashDrawer()
        {
            if (ActiveRegisterDrawer.RegisterSubId == 0)
                DeviceManager.OpenCashDrawer1();
            else if (ActiveRegisterDrawer.RegisterSubId == 1)
                DeviceManager.OpenCashDrawer2();
        }

    }
}
