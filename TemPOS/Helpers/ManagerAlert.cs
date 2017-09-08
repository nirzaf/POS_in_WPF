using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using PosControls;
using TemPOS.Types;
using PosControls.Helpers;
using PosModels.Types;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS.Helpers
{
    public static class ManagerAlert
    {
        private static bool _forceStop;
        private static readonly DispatcherTimer Timer = new DispatcherTimer();
        private static readonly Queue<Alert> Alerts = new Queue<Alert>();

        public static bool IsEnabled
        {
            get;
            private set;
        }

        static ManagerAlert()
        {
            IsEnabled = false;
            SessionManager.ActiveEmployeeChanged +=
                SessionManager_ActiveEmployeeChanged;
            Timer.Tick += timer_Tick;
            Timer.Interval = new TimeSpan(0, 0, 1);
        }

        private static void SessionManager_ActiveEmployeeChanged(object sender, EventArgs e)
        {
            if (SessionManager.ActiveEmployee == null)
                IsEnabled = false;
            else
                IsEnabled = 
                    (SessionManager.ActiveEmployee
                    .HasPermission(Permissions.ManagerAlerts));             
        }

        private static void Show(Alert alert)
        {
            if (alert == null) return;
            if (alert.Type == Alert.AlertType.Message)
                Show(alert.Message, alert.DialogButtonChoices);
            else if (alert.Type == Alert.AlertType.MessageWithCustomButtons)
                Show(alert.Message, alert.ButtonChoices);
            else if (alert.Type == Alert.AlertType.Control)
                Show(alert.Control, alert.Width, alert.Height);
        }

        public static DialogButton? Show(string message, DialogButtons dialogButtons = DialogButtons.Ok)
        {
            if (!IsEnabled) return null;
            Window window = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            return PosDialogWindow.ShowDialog(message, Types.Strings.ManagerAlert, dialogButtons);
        }

        public static int? Show(string message, string[] buttonChoices)
        {
            if (!IsEnabled) return null;
            Window window = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            return PosDialogWindow.ShowDialog(window, message, Types.Strings.ManagerAlert, buttonChoices);
        }

        public static bool? Show(FrameworkElement control, double width, double height)
        {
            if (!IsEnabled) return null;
            Window parent = Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            PosDialogWindow window = new PosDialogWindow(control, Types.Strings.ManagerAlert, width, height);
            return window.ShowDialog(parent);
        }

        public static void QueueAlert(string message, DialogButtons dialogButtons = DialogButtons.Ok)
        {
            if (!IsEnabled) return;
            Alerts.Enqueue(new Alert(message, dialogButtons));
        }

        public static void QueueAlert(string message, string[] buttonChoices)
        {
            if (!IsEnabled) return;
            Alerts.Enqueue(new Alert(message, buttonChoices));
        }

        public static void QueueAlert(FrameworkElement control, double width, double height)
        {
            if (!IsEnabled) return;
            Alerts.Enqueue(new Alert(control, width, height));
        }

        public static void Start()
        {
            Timer.IsEnabled = true;
            _forceStop = false;
        }

        public static void Stop()
        {
            _forceStop = true;
        }

        static void timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = sender as DispatcherTimer;
            if (timer == null) return;
            if (_forceStop)
            {
                _forceStop = false;
                timer.IsEnabled = false;
                return;
            }
            if (!IsEnabled) return;
            timer.IsEnabled = false;
            while (Alerts.Count > 0)
                Show(Alerts.Dequeue());
            timer.IsEnabled = true;
        }

        #region The Alert class
        private class Alert
        {
            public enum AlertType
            {
                Message,
                MessageWithCustomButtons,
                Control
            }

            public AlertType Type
            {
                get;
                private set;
            }

            public string Message
            {
                get;
                private set;
            }

            public FrameworkElement Control
            {
                get;
                private set;
            }

            public double Width
            {
                get;
                private set;
            }

            public double Height
            {
                get;
                private set;
            }

            public DialogButtons DialogButtonChoices
            {
                get;
                private set;
            }

            public string[] ButtonChoices
            {
                get;
                private set;
            }

            public Alert(string message, DialogButtons dialogButtons = DialogButtons.Ok)
            {
                Type = AlertType.Message;
                Message = message;
                DialogButtonChoices = dialogButtons;
            }

            public Alert(string message, string[] buttonChoices)
            {
                Type = AlertType.MessageWithCustomButtons;
                Message = message;
                ButtonChoices = buttonChoices;
            }

            public Alert(FrameworkElement control, double width, double height)
            {
                Type = AlertType.Control;
                Control = control;
                Width = width;
                Height = height;
            }
        }
        #endregion
    }
}
