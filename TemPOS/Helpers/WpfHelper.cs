using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using PosControls;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace TemPOS.Helpers
{
    public static class WpfHelper
    {
        private enum BindingUpdateType
        {
            String,
            Brush
        }
        public static bool? ShowDialogForActiveWindow(this Window thisWindow)
        {
            thisWindow.Owner = 
                Application.Current.Windows.Cast<Window>().SingleOrDefault(x => x.IsActive);
            return thisWindow.ShowDialog();
        }

        public static void UpdateStringBindings()
        {
            foreach (Window window in Application.Current.Windows)
            {
                UpdateStringDependencyProperties(window);
                UpdateChildBindings(window, BindingUpdateType.String);
            }

        }

        public static void UpdateBrushBindings()
        {
            foreach (Window window in Application.Current.Windows)
            {
                UpdateBrushDependencyProperties(window);
                UpdateChildBindings(window, BindingUpdateType.Brush);                
            }
        }

        private static void UpdateChildBindings(DependencyObject dependencyObject,
            BindingUpdateType updateType)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(dependencyObject).OfType<DependencyObject>())
            {
                UpdateBinding(child, updateType);
                IEnumerator enumerator = LogicalTreeHelper.GetChildren(child).GetEnumerator();
                enumerator.Reset();
                if (enumerator.MoveNext())
                    UpdateChildBindings(child, updateType);
            }
        }

        private static void UpdateBinding(DependencyObject child, BindingUpdateType updateType)
        {
            switch (updateType)
            {
                case BindingUpdateType.Brush:
                    UpdateBrushDependencyProperties(child);
                    break;
                case BindingUpdateType.String:
                    UpdateStringDependencyProperties(child);
                    break;
            }
        }

        private static void UpdateStringDependencyProperties(DependencyObject dependencyObject)
        {
            if (dependencyObject is TextBlock)
            {
                DoBindingUpdate(dependencyObject, TextBlock.TextProperty);
            }
            else if (dependencyObject is HeaderedContentControl)
            {
                DoBindingUpdate(dependencyObject, HeaderedContentControl.HeaderProperty);
            }
            else if (dependencyObject is TextBlockButton)
            {
                DoBindingUpdate(dependencyObject, TextBlockButton.TextProperty);
            }
            else if (dependencyObject is Label)
            {
                DependencyObject owner = GetContainerControl(dependencyObject);
                DoBindingUpdate(dependencyObject, ContentControl.ContentProperty);
            }
            else if (dependencyObject is ContentControl)
            {
                DoBindingUpdate(dependencyObject, ContentControl.ContentProperty);
            }
            else if (dependencyObject is StatusBar)
            {
                
            }
        }

        private static void UpdateBrushDependencyProperties(DependencyObject dependencyObject)
        {
            if (dependencyObject is FrameworkElement)
            {
                var obj = dependencyObject as FrameworkElement;
                if (obj.ContextMenu != null)
                    UpdateContextMenu(obj.ContextMenu);
            }
            if (dependencyObject is Grid)
            {
                DoBindingUpdate(dependencyObject, Panel.BackgroundProperty);
            }
            else if (dependencyObject is StackPanel)
            {
                DoBindingUpdate(dependencyObject, Panel.BackgroundProperty);
            }
            else if (dependencyObject is Button)
            {
                DoBindingUpdate(dependencyObject, Control.BackgroundProperty);
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
            }
            else if (dependencyObject is ToggleButton)
            {
                DoBindingUpdate(dependencyObject, Control.BackgroundProperty);
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
            }
            else if (dependencyObject is PushCheckBox)
            {
                DoBindingUpdate(dependencyObject, PushCheckBox.DisabledBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushCheckBox.EnabledBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushCheckBox.EnabledSelectedBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushCheckBox.DisabledSelectedBackgroundProperty);
            }
            else if (dependencyObject is PushComboBox)
            {
                DoBindingUpdate(dependencyObject, PushComboBox.EnabledForegroundProperty);
                DoBindingUpdate(dependencyObject, PushComboBox.EnabledBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushComboBox.DisabledForegroundProperty);
                DoBindingUpdate(dependencyObject, PushComboBox.DisabledBackgroundProperty);
            }
            else if (dependencyObject is PushRadioButton)
            {
                DoBindingUpdate(dependencyObject, PushRadioButton.EnabledBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushRadioButton.EnabledSelectedBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushRadioButton.DisabledBackgroundProperty);
                DoBindingUpdate(dependencyObject, PushRadioButton.DisabledSelectedBackgroundProperty);
            }
            else if (dependencyObject is CustomTextBox)
            {
                DoBindingUpdate(dependencyObject, CustomTextBox.EnabledBackgroundBrushProperty);
                DoBindingUpdate(dependencyObject, CustomTextBox.EnabledForegroundBrushProperty);
                DoBindingUpdate(dependencyObject, CustomTextBox.DisabledBackgroundBrushProperty);
                DoBindingUpdate(dependencyObject, CustomTextBox.DisabledForegroundBrushProperty);
            }
            else if (dependencyObject is Label)
            {
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
            }
            else if (dependencyObject is Border)
            {
                DoBindingUpdate(dependencyObject, Border.BorderBrushProperty);
                DoBindingUpdate(dependencyObject, Border.BackgroundProperty);
            }
            else if (dependencyObject is StatusBar)
            {
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
            }
            else if (dependencyObject is FormattedListBoxItem)
            {
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
                DoBindingUpdate(dependencyObject, Control.BackgroundProperty);
            }
            else if (dependencyObject is TicketItemTemplate)
            {
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
                DoBindingUpdate(dependencyObject, Control.BackgroundProperty);
            }
            else if (dependencyObject is TextBlockButton)
            {
                DoBindingUpdate(dependencyObject, Control.ForegroundProperty);
                DoBindingUpdate(dependencyObject, Control.BackgroundProperty);
            }
        }

        private static DependencyObject GetContainerControl(DependencyObject childDependencyObject)
        {
            if (childDependencyObject == null)
                return null;
            DependencyObject result = LogicalTreeHelper.GetParent(childDependencyObject);
            while ((result != null) && !(result is Window) && !(result is Control))
                result = LogicalTreeHelper.GetParent(result);
            return result;
        }

        private static UserControl GetUserControl(DependencyObject parentDependencyObject)
        {
            if (parentDependencyObject == null)
                return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parentDependencyObject); i++)
            {
                DependencyObject depObject = VisualTreeHelper.GetChild(parentDependencyObject, i);
                if (depObject is UserControl)
                    return depObject as UserControl;
                if (VisualTreeHelper.GetChildrenCount(depObject) <= 0) continue;
                UserControl childFound = GetUserControl(depObject);
                if (childFound != null)
                    return childFound;
            }
            return null;
        }

        private static void UpdateContextMenu(ContextMenu contextMenu)
        {
            UserControl userControl = GetUserControl(contextMenu);
            if (userControl == null) return;
            UpdateBrushDependencyProperties(userControl);
            UpdateChildBindings(userControl, BindingUpdateType.Brush);
        }

        private static void DoBindingUpdate(DependencyObject dependencyObject, DependencyProperty dependencyProperty)
        {
            BindingExpressionBase bindingBase =
                BindingOperations.GetBindingExpressionBase(dependencyObject, dependencyProperty);
            if (bindingBase != null)
                bindingBase.UpdateTarget();
        }
    }
}
