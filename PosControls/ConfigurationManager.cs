using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PosModels;

namespace PosControls
{
    internal class ConfigurationObjectManager
    {
        #region Defaults
        public static object GetDefault(string settingName)
        {
            #region ApplicationBackgroundBrush
            if (settingName == "ApplicationBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(10, 10, 65), 0.2));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0, 0, 0), 1));
                brush.EndPoint = new Point(0, 1);
                return brush;
            }
            #endregion
            #region ApplicationForegroundBrush
            if (settingName == "ApplicationForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region ApplicationTitleBarBrush
            if (settingName == "ApplicationTitleBarBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(45, 45, 110), 0.5));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(45, 45, 85), 1));
                return brush;
            }
            #endregion
            #region ListItemForegroundBrush
            if (settingName == "ListItemForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region ListItemBackgroundBrush
            if (settingName == "ListItemBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region ListItemDisabledForegroundBrush
            if (settingName == "ListItemDisabledForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region ListItemDisabledBackgroundBrush
            if (settingName == "ListItemDisabledBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region ListItemSelectedForegroundBrush
            if (settingName == "ListItemSelectedForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region ListItemSelectedBackgroundBrush
            if (settingName == "ListItemSelectedBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x80, 0x0), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x60, 0x0), 1));
                return brush;
            }
            #endregion
            #region ListItemDisabledSelectedForegroundBrush
            if (settingName == "ListItemDisabledSelectedForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region ListItemDisabledSelectedBackgroundBrush
            if (settingName == "ListItemDisabledSelectedBackgroundBrush")
            {
                return Brushes.DarkGreen;
            }
            #endregion
            #region TabButtonForegroundBrush
            if (settingName == "TabButtonForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region TabButtonBackgroundBrush
            if (settingName == "TabButtonBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region TabButtonDisabledForegroundBrush
            if (settingName == "TabButtonDisabledForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region TabButtonBackgroundBrush
            if (settingName == "TabButtonDisabledBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region TabButtonSelectedForegroundBrush
            if (settingName == "TabButtonSelectedForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region TabButtonSelectedBackgroundBrush
            if (settingName == "TabButtonSelectedBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x80, 0x0), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x60, 0x0), 1));
                return brush;
            }
            #endregion
            #region TabButtonDisabledSelectedForegroundBrush
            if (settingName == "TabButtonDisabledSelectedForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region TabButtonDisabledSelectedBackgroundBrush
            if (settingName == "TabButtonDisabledSelectedBackgroundBrush")
            {
                return Brushes.DarkGreen;
            }
            #endregion
            #region LabelForegroundBrush
            if (settingName == "LabelForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region DisabledLabelForegroundBrush
            if (settingName == "DisabledLabelForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region CaretBrush
            if (settingName == "CaretBrush")
            {
                return Brushes.DarkGray;
            }
            #endregion
            #region BorderBrush
            if (settingName == "BorderBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(175, 175, 220), 1));
                brush.GradientStops.Add(new GradientStop(Brushes.White.Color, 0.1));
                return brush;
            }
            #endregion
            #region DisabledBorderBrush
            if (settingName == "DisabledBorderBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region EnabledComboBoxForegroundBrush
            if (settingName == "EnabledComboBoxForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region EnabledComboBoxBackgroundBrush
            if (settingName == "EnabledComboBoxBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region DisabledComboBoxForegroundBrush
            if (settingName == "DisabledComboBoxForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region DisabledComboBoxBackgroundBrush
            if (settingName == "DisabledComboBoxBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region ButtonBackgroundBrush
            if (settingName == "ButtonBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region ButtonForegroundBrush
            if (settingName == "ButtonForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region SelectedButtonBackgroundBrush
            if (settingName == "SelectedButtonBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x80, 0x0), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x60, 0x0), 1));
                return brush;
            }
            #endregion
            #region EnabledSelectedButtonForegroundBrush
            if (settingName == "EnabledSelectedButtonForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region DisabledSelectedButtonForegroundBrush
            if (settingName == "DisabledSelectedButtonForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region DisabledButtonBackgroundBrush
            if (settingName == "DisabledButtonBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region DisabledButtonForegroundBrush
            if (settingName == "DisabledButtonForegroundBrush")
            {
                return Brushes.LightGray;
            }
            #endregion
            #region DisabledSelectedButtonBackgroundBrush
            if (settingName == "DisabledSelectedButtonBackgroundBrush")
            {
                return Brushes.DarkGreen;
            }
            #endregion
            #region EnabledCheckBoxBrush
            if (settingName == "EnabledCheckBoxBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region EnabledSelectedCheckBoxBrush
            if (settingName == "EnabledSelectedCheckBoxBrush")
            {
                return Brushes.White;
            }
            #endregion 
            #region DisabledCheckBoxBrush
            if (settingName == "DisabledCheckBoxBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x40), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x30), 1));
                return brush;
            }
            #endregion
            #region DisabledSelectedCheckBoxBrush
            if (settingName == "DisabledSelectedCheckBoxBrush")
            {
                return Brushes.Gray;
            }
            #endregion
            #region TextboxBackgroundBrush
            if (settingName == "TextboxBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x60, 0, 0x80));
            }
            #endregion
            #region TextboxForegroundBrush
            if (settingName == "TextboxForegroundBrush")
            {
                return Brushes.White;
            }
            #endregion
            #region DisabledTextboxBackgroundBrush
            if (settingName == "DisabledTextboxBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region DisabledTextboxForegroundBrush
            if (settingName == "DisabledTextboxForegroundBrush")
            {
                return Brushes.Gray;
            }
            #endregion
            #region EnabledRadioButtonBackgroundBrush
            if (settingName == "EnabledRadioButtonBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x80), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x0, 0x60), 1));
                return brush;
            }
            #endregion
            #region EnabledSelectedRadioButtonBackgroundBrush
            if (settingName == "EnabledSelectedRadioButtonBackgroundBrush")
            {
                var brush = new LinearGradientBrush();
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x80, 0x0), 0.3));
                brush.GradientStops.Add(new GradientStop(Color.FromRgb(0x0, 0x60, 0x0), 1));
                return brush;
            }
            #endregion
            #region DisabledRadioButtonBackgroundBrush
            if (settingName == "DisabledRadioButtonBackgroundBrush")
            {
                return new SolidColorBrush(Color.FromArgb(0xff, 0x29, 0x28, 0x51));
            }
            #endregion
            #region DisabledSelectedRadioButtonBackgroundBrush
            if (settingName == "DisabledSelectedRadioButtonBackgroundBrush")
            {
                return Brushes.DarkGreen;
            }
            #endregion
            #region ButtonFontFamily
            if (settingName == "ButtonFontFamily")
            {
                return "Century Schoolbook";
            }
            #endregion
            #region ForceWasteOnVoid
            if (settingName == "ForceWasteOnVoid")
            {
                return true;
            }
            #endregion
            #region LogoutOnPlaceOrder
            if (settingName == "LogoutOnPlaceOrder")
            {
                return true;
            }
            #endregion
            #region UseOnScreenTextEntry
            if (settingName == "UseOnScreenTextEntry")
            {
                return true;
            }
            #endregion
            #region UsePaperlessPrintDestinations
            if (settingName == "UsePaperlessPrintDestinations")
            {
                return true;
            }
            #endregion
            #region UseKeyboardHook
            if (settingName == "UseKeyboardHook")
            {
                return true;
            }
            #endregion
            #region UseSeating
            if (settingName == "UseSeating")
            {
                return false;
            }
            #endregion
            #region Not Implemented
            throw new NotImplementedException(settingName);
            #endregion
        }
        public static void WriteDefault(string settingName)
        {
            SerialObject serialObject = SerialObject.Get(settingName);
            object serializableObject = GetDefault(settingName);
            if (serializableObject is SolidColorBrush)
                serializableObject = new BrushWrapper(serializableObject as SolidColorBrush);
            if (serializableObject is LinearGradientBrush)
                serializableObject = new BrushWrapper(serializableObject as LinearGradientBrush);
            if (serializableObject is RadialGradientBrush)
                serializableObject = new BrushWrapper(serializableObject as RadialGradientBrush);
            if (serializableObject is DrawingBrush)
                serializableObject = new BrushWrapper(serializableObject as DrawingBrush);

            if (serialObject == null)
                SerialObject.Add(settingName, serializableObject);
            else
            {
                serialObject.SetSerializedObject(serializableObject);
                serialObject.Update();
            }
        }
        public static void WriteAllDefaults()
        {
            WriteDefault("ApplicationBackgroundBrush");
            WriteDefault("ApplicationForegroundBrush");
            WriteDefault("ApplicationTitleBarBrush");
            WriteDefault("BorderBrush");
            WriteDefault("DisabledBorderBrush");
            WriteDefault("CaretBrush");

            // List item defaults
            WriteDefault("ListItemForegroundBrush");
            WriteDefault("ListItemBackgroundBrush");
            WriteDefault("ListItemDisabledForegroundBrush");
            WriteDefault("ListItemDisabledBackgroundBrush");
            WriteDefault("ListItemSelectedForegroundBrush");
            WriteDefault("ListItemSelectedBackgroundBrush");
            WriteDefault("ListItemDisabledSelectedForegroundBrush");
            WriteDefault("ListItemDisabledSelectedBackgroundBrush");

            // Tab button defaults
            WriteDefault("TabButtonForegroundBrush");
            WriteDefault("TabButtonBackgroundBrush");
            WriteDefault("TabButtonDisabledForegroundBrush");
            WriteDefault("TabButtonDisabledBackgroundBrush");
            WriteDefault("TabButtonSelectedForegroundBrush");
            WriteDefault("TabButtonSelectedBackgroundBrush");
            WriteDefault("TabButtonDisabledSelectedForegroundBrush");
            WriteDefault("TabButtonDisabledSelectedBackgroundBrush");

            WriteDefault("EnabledRadioButtonBackgroundBrush");
            WriteDefault("EnabledSelectedRadioButtonBackgroundBrush");
            WriteDefault("DisabledRadioButtonBackgroundBrush");
            WriteDefault("DisabledSelectedRadioButtonBackgroundBrush");

            WriteDefault("EnabledComboBoxForegroundBrush");
            WriteDefault("EnabledComboBoxBackgroundBrush");
            WriteDefault("DisabledComboBoxForegroundBrush");
            WriteDefault("DisabledComboBoxBackgroundBrush");

            WriteDefault("LabelForegroundBrush");
            WriteDefault("DisabledLabelForegroundBrush");

            WriteDefault("ButtonBackgroundBrush");
            WriteDefault("SelectedButtonBackgroundBrush");
            WriteDefault("ButtonForegroundBrush");
            WriteDefault("DisabledButtonBackgroundBrush");
            WriteDefault("DisabledButtonForegroundBrush");
            WriteDefault("DisabledSelectedButtonBackgroundBrush");
            WriteDefault("EnabledSelectedButtonForegroundBrush");
            WriteDefault("DisabledSelectedButtonForegroundBrush");

            WriteDefault("EnabledCheckBoxBrush");
            WriteDefault("EnabledSelectedCheckBoxBrush");
            WriteDefault("DisabledCheckBoxBrush");
            WriteDefault("DisabledSelectedCheckBoxBrush");
            WriteDefault("TextboxBackgroundBrush");
            WriteDefault("TextboxForegroundBrush");
            WriteDefault("DisabledTextboxBackgroundBrush");
            WriteDefault("DisabledTextboxForegroundBrush");
            WriteDefault("ButtonFontFamily");
            WriteDefault("ForceWasteOnVoid");
            WriteDefault("UseOnScreenTextEntry");
            WriteDefault("UsePaperlessPrintDestinations");
            WriteDefault("UseKeyboardHook");
            WriteDefault("UseSeating");
            WriteDefault("LogoutOnPlaceOrder");            
        }
        #endregion
        #region Create
        public static Brush CreateBrush(string settingName, bool useDefault = false)
        {
            if (useDefault)
                return (Brush)GetDefault(settingName);
            try
            {
                SerialObject serialObject = SerialObject.Get(settingName);
                return ((BrushWrapper)serialObject.GetDeserializedObject()).CreateBrush();
            }
            catch
            {
                WriteDefault(settingName);
                SerialObject serialObject = SerialObject.Get(settingName);
                return ((BrushWrapper)serialObject.GetDeserializedObject()).CreateBrush();
            }
        }

        public static string CreateString(string settingName, bool useDefault = false)
        {
            if (useDefault)
                return (string)GetDefault(settingName);
            try
            {
                SerialObject serialObject = SerialObject.Get(settingName);
                if (serialObject == null)
                {
                    WriteDefault(settingName);
                    serialObject = SerialObject.Get(settingName);
                }
                return (string)serialObject.GetDeserializedObject();
            }
            catch
            {
                WriteDefault(settingName);
                SerialObject serialObject = SerialObject.Get(settingName);
                return (string)serialObject.GetDeserializedObject();
            }
        }

        public static bool CreateBoolean(string settingName, bool useDefault = false)
        {
            if (useDefault)
                return (bool)GetDefault(settingName);
            try
            {
                SerialObject serialObject = SerialObject.Get(settingName);
                if (serialObject == null)
                {
                    WriteDefault(settingName);
                    serialObject = SerialObject.Get(settingName);
                }
                return (bool)serialObject.GetDeserializedObject();
            }
            catch
            {
                WriteDefault(settingName);
                SerialObject serialObject = SerialObject.Get(settingName);
                return (bool)serialObject.GetDeserializedObject();
            }
        }
        #endregion
        #region Save
        public static void Save(string settingName, object serializableObject)
        {
            if (serializableObject is SolidColorBrush)
                serializableObject = new BrushWrapper(serializableObject as SolidColorBrush);
            if (serializableObject is LinearGradientBrush)
                serializableObject = new BrushWrapper(serializableObject as LinearGradientBrush);
            if (serializableObject is RadialGradientBrush)
                serializableObject = new BrushWrapper(serializableObject as RadialGradientBrush);
            if (serializableObject is DrawingBrush)
                serializableObject = new BrushWrapper(serializableObject as DrawingBrush);
            SerialObject serialObject = SerialObject.Get(settingName);
            if (serialObject == null)
                SerialObject.Add(settingName, serializableObject);
            else
            {
                serialObject.SetSerializedObject(serializableObject);
                serialObject.Update();
            }
        }
        #endregion
    }

    public class ConfigurationManager : Control
    {
        public static ConfigurationManager Singleton
        {
            get;
            private set;
        }
        
        public const double ReceiptTapeItemWidth = 375d;
        public const int ButtonNoWrapLength = 6;

// ReSharper disable RedundantNameQualifier
        public const WindowState WindowState = System.Windows.WindowState.Maximized;
// ReSharper restore RedundantNameQualifier
        public static double ProgramWidth = 1366;
        public static double ProgramHeight = 768;
        public static double ProgramScale = 0.75;

        private static bool? _isInDesignMode;
        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
                    var prop = DesignerProperties.IsInDesignModeProperty;
                    _isInDesignMode = (bool)DependencyPropertyDescriptor
                        .FromProperty(prop, typeof(FrameworkElement))
                        .Metadata.DefaultValue;
#endif
                }
                return _isInDesignMode.Value;
            }
        }

        private static bool _alwaysUseDefaults = true;
        public static bool AlwaysUseDefaults
        {
            get { return _alwaysUseDefaults; }
            set
            {
                _alwaysUseDefaults = value;
                InitializeCurrentValues(value);
            }
        }

        static ConfigurationManager()
        {
#if !DEBUG
            if (!System.Linq.Enumerable.SequenceEqual(
                typeof(ConfigurationManager).Assembly.GetName().GetPublicKeyToken(),
                System.Reflection.Assembly.GetEntryAssembly().GetName().GetPublicKeyToken()))
            {
                throw new Exception("You are not licensed to use PosControls.dll");
            }
#endif
            AlwaysUseDefaults = true;
            double screenRatio = SystemParameters.PrimaryScreenWidth / SystemParameters.PrimaryScreenHeight;
            const double officeDef = (16 / (double)10);
            const double newStandardDef = (16 / (double)9);
            if ((screenRatio >= officeDef) && (screenRatio <= newStandardDef))
            {
                ProgramWidth = SystemParameters.PrimaryScreenWidth;
                ProgramHeight = SystemParameters.PrimaryScreenHeight;
                ProgramScale = 1;
            }
            else
            {
                double scaleRatio = screenRatio / newStandardDef;
                ProgramWidth = SystemParameters.PrimaryScreenWidth / scaleRatio;
                ProgramHeight = SystemParameters.PrimaryScreenHeight;
                ProgramScale = scaleRatio;
            }
            Singleton = new ConfigurationManager();
            InitializeCurrentValues(true);
        }

        private static void InitializeCurrentValues(bool useDefaults)
        {
            ApplicationBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ApplicationBackgroundBrush", useDefaults);

            ApplicationForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ApplicationForegroundBrush", useDefaults);

            ApplicationTitleBarBrush =
                ConfigurationObjectManager.CreateBrush("ApplicationTitleBarBrush", useDefaults);

            ListItemForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemForegroundBrush", useDefaults);
            ListItemBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemBackgroundBrush", useDefaults);
            ListItemDisabledForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemDisabledForegroundBrush", useDefaults);
            ListItemDisabledBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemDisabledBackgroundBrush", useDefaults);
            ListItemSelectedForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemSelectedForegroundBrush", useDefaults);
            ListItemSelectedBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemSelectedBackgroundBrush", useDefaults);
            ListItemDisabledSelectedForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemDisabledSelectedForegroundBrush", useDefaults);
            ListItemDisabledSelectedBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ListItemDisabledSelectedBackgroundBrush", useDefaults);

            TabButtonForegroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonForegroundBrush", useDefaults);
            TabButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonBackgroundBrush", useDefaults);
            TabButtonDisabledForegroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonDisabledForegroundBrush", useDefaults);
            TabButtonDisabledBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonDisabledBackgroundBrush", useDefaults);
            TabButtonSelectedForegroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonSelectedForegroundBrush", useDefaults);
            TabButtonSelectedBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonSelectedBackgroundBrush", useDefaults);
            TabButtonDisabledSelectedForegroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonDisabledSelectedForegroundBrush", useDefaults);
            TabButtonDisabledSelectedBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("TabButtonDisabledSelectedBackgroundBrush", useDefaults);

            BorderBrush = ConfigurationObjectManager.CreateBrush("BorderBrush", useDefaults);

            DisabledBorderBrush =
                ConfigurationObjectManager.CreateBrush("DisabledBorderBrush", useDefaults);

            CaretBrush = ConfigurationObjectManager.CreateBrush("CaretBrush", useDefaults);

            LabelForegroundBrush =
                ConfigurationObjectManager.CreateBrush("LabelForegroundBrush", useDefaults);
            
            DisabledLabelForegroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledLabelForegroundBrush", useDefaults);

            ButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("ButtonBackgroundBrush", useDefaults);

            ButtonForegroundBrush =
                ConfigurationObjectManager.CreateBrush("ButtonForegroundBrush", useDefaults);

            DisabledButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledButtonBackgroundBrush", useDefaults);

            DisabledButtonForegroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledButtonForegroundBrush", useDefaults);

            DisabledSelectedButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledSelectedButtonBackgroundBrush", useDefaults);

            EnabledSelectedButtonForegroundBrush =
                ConfigurationObjectManager.CreateBrush("EnabledSelectedButtonForegroundBrush", useDefaults);
            
            DisabledSelectedButtonForegroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledSelectedButtonForegroundBrush", useDefaults);

            #region PushCheckBox
            EnabledCheckBoxBrush =
                ConfigurationObjectManager.CreateBrush("EnabledCheckBoxBrush", useDefaults);

            EnabledSelectedCheckBoxBrush =
                ConfigurationObjectManager.CreateBrush("EnabledSelectedCheckBoxBrush", useDefaults);

            DisabledCheckBoxBrush =
                ConfigurationObjectManager.CreateBrush("DisabledCheckBoxBrush", useDefaults);

            DisabledSelectedCheckBoxBrush =
                ConfigurationObjectManager.CreateBrush("DisabledSelectedCheckBoxBrush", useDefaults);
            #endregion

            #region PushComboBox
            EnabledComboBoxForegroundBrush =
                ConfigurationObjectManager.CreateBrush("EnabledComboBoxForegroundBrush", useDefaults);

            EnabledComboBoxBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("EnabledComboBoxBackgroundBrush", useDefaults);

            DisabledComboBoxForegroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledComboBoxForegroundBrush", useDefaults);

            DisabledComboBoxBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledComboBoxBackgroundBrush", useDefaults);
            #endregion

            #region PushRadioButton
            EnabledRadioButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("EnabledRadioButtonBackgroundBrush", useDefaults);

            EnabledSelectedRadioButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("EnabledSelectedRadioButtonBackgroundBrush", useDefaults);

            DisabledRadioButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledRadioButtonBackgroundBrush", useDefaults);

            DisabledSelectedRadioButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledSelectedRadioButtonBackgroundBrush", useDefaults);
            #endregion

            SelectedButtonBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("SelectedButtonBackgroundBrush", useDefaults);

            TextboxBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("TextboxBackgroundBrush", useDefaults);

            TextboxForegroundBrush =
                ConfigurationObjectManager.CreateBrush("TextboxForegroundBrush", useDefaults);

            DisabledTextboxBackgroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledTextboxBackgroundBrush", useDefaults);

            DisabledTextboxForegroundBrush =
                ConfigurationObjectManager.CreateBrush("DisabledTextboxForegroundBrush", useDefaults);

            string fontName = ConfigurationObjectManager.CreateString("ButtonFontFamily", useDefaults);
            ButtonFontFamily = new FontFamily(fontName);

            ForceWasteOnVoid =
                ConfigurationObjectManager.CreateBoolean("ForceWasteOnVoid", useDefaults);

            LogoutOnPlaceOrder =
                ConfigurationObjectManager.CreateBoolean("LogoutOnPlaceOrder", useDefaults);

            UseOnScreenTextEntry =
                ConfigurationObjectManager.CreateBoolean("UseOnScreenTextEntry", useDefaults);

            UsePaperlessPrintDestinations =
                ConfigurationObjectManager.CreateBoolean("UsePaperlessPrintDestinations", useDefaults);

            UseSeating =
                ConfigurationObjectManager.CreateBoolean("UseSeating", useDefaults);

#if DEBUG
            UseKeyboardHook = true;
#else
            UseKeyboardHook =
                ConfigurationObjectManager.CreateBoolean("UseKeyboardHook", useDefaults); 
#endif
        }

        public static void SetUseSeating(bool useSeating)
        {
            SerialObject serialObject = SerialObject.Get("UseSeating");
            if (serialObject == null)
                SerialObject.Add("UseSeating", useSeating);
            else
            {
                serialObject.SetSerializedObject(useSeating);
                serialObject.Update();
            }
            UseSeating = useSeating;
        }

        public static void SetForceWasteOnVoid(bool forceWasteOnVoid)
        {
            SerialObject serialObject = SerialObject.Get("ForceWasteOnVoid");
            if (serialObject == null)
                SerialObject.Add("ForceWasteOnVoid", forceWasteOnVoid);
            else
            {
                serialObject.SetSerializedObject(forceWasteOnVoid);
                serialObject.Update();
            }
            ForceWasteOnVoid = forceWasteOnVoid;
        }

        public static void SetUseKeyboardHook(bool useKeyboardHook)
        {
            SerialObject serialObject = SerialObject.Get("UseKeyboardHook");
            if (serialObject == null)
                SerialObject.Add("UseKeyboardHook", useKeyboardHook);
            else
            {
                serialObject.SetSerializedObject(useKeyboardHook);
                serialObject.Update();
            }
            UseKeyboardHook = useKeyboardHook;
        }

        public static void SetLogoutOnPlaceOrder(bool logoutOnPlaceOrder)
        {
            SerialObject serialObject = SerialObject.Get("LogoutOnPlaceOrder");
            if (serialObject == null)
                SerialObject.Add("LogoutOnPlaceOrder", logoutOnPlaceOrder);
            else
            {
                serialObject.SetSerializedObject(logoutOnPlaceOrder);
                serialObject.Update();
            }
            LogoutOnPlaceOrder = logoutOnPlaceOrder;
        }

        public static void SetBrush(string brushName, Brush brush)
        {
            PropertyInfo property = null;
            if (!brushName.Equals("BorderBrush"))
            {
                // Use reflection to change the value of the property
                property = typeof(ConfigurationManager).GetProperty(brushName);
                if (property == null)
                    throw new Exception("Invalid brush name");
                if (!(property.GetValue(null, null) is Brush))
                    throw new Exception("Invalid brush name");
            }
            var serialObject = SerialObject.Get(brushName);
            var wrapper = new BrushWrapper(brush);
            if (serialObject == null)
                SerialObject.Add(brushName, wrapper);
            else
            {
                serialObject.SetSerializedObject(wrapper);
                serialObject.Update();
            }
            if (property != null)
                property.SetValue(null, brush, null);
            else
                BorderBrush = brush;
        }

        #region Static Properties
        public static Brush ApplicationBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush ApplicationForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ApplicationTitleBarBrush
        {
            get;
            private set;
        }

        public static Brush ListItemForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemDisabledForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemDisabledBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemSelectedForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemSelectedBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemDisabledSelectedForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ListItemDisabledSelectedBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonForegroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonDisabledForegroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonDisabledBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonSelectedForegroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonSelectedBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonDisabledSelectedForegroundBrush
        {
            get;
            private set;
        }

        public static Brush TabButtonDisabledSelectedBackgroundBrush
        {
            get;
            private set;
        }

        public static new Brush BorderBrush
        {
            get;
            private set;
        }

        public static Brush DisabledBorderBrush
        {
            get;
            private set;
        }

        public static Brush CaretBrush
        {
            get;
            private set;
        }

        public static Brush LabelForegroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledLabelForegroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledSelectedButtonForegroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledSelectedButtonForegroundBrush
        {
            get;
            private set;
        }

        public static Brush ButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush ButtonForegroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledComboBoxForegroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledComboBoxBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledComboBoxForegroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledComboBoxBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledCheckBoxBrush
        {
            get;
            private set;
        }

        public static Brush DisabledCheckBoxBrush
        {
            get;
            private set;
        }

        public static Brush DisabledSelectedCheckBoxBrush
        {
            get;
            private set;
        }

        public static Brush EnabledSelectedCheckBoxBrush
        {
            get;
            private set;
        }

        public static Brush DisabledButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledButtonForegroundBrush
        {
            get;
            private set;
        }

        public static Brush SelectedButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledSelectedButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TextboxBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush TextboxForegroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledTextboxBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledTextboxForegroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledRadioButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush EnabledSelectedRadioButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledRadioButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static Brush DisabledSelectedRadioButtonBackgroundBrush
        {
            get;
            private set;
        }

        public static FontFamily ButtonFontFamily
        {
            get;
            private set;
        }

        public static bool ForceWasteOnVoid
        {
            get;
            private set;
        }

        public static bool LogoutOnPlaceOrder
        {
            get;
            private set;
        }

        public static bool UseOnScreenTextEntry
        {
            get;
            private set;
        }

        public static bool UsePaperlessPrintDestinations
        {
            get;
            private set;
        }

        public static bool UseKeyboardHook
        {
            get;
            private set;
        }

        public static bool UseSeating
        {
            get;
            private set;
        }
        #endregion

        public static object GetSetting(string settingName)
        {
            SerialObject serialObject = SerialObject.Get(settingName);
            if (serialObject == null)
                return null;
            return serialObject.GetDeserializedObject();
        }

        public static void SetSetting(string settingName, object obj)
        {
            if (obj == null)
                SerialObject.Delete(settingName);
            else
            {
                SerialObject serialObject = SerialObject.Get(settingName);
                if (serialObject == null)
                    SerialObject.Add(settingName, obj);
                else
                {
                    serialObject.SetSerializedObject(obj);
                    serialObject.Update();
                }
            }
            InitializeCurrentValues(false);
        }

        public static void SetSetting(string settingName, ISerializable obj)
        {
            if (obj == null)
                SerialObject.Delete(settingName);
            else
            {
                SerialObject serialObject = SerialObject.Get(settingName);
                if (serialObject == null)
                    SerialObject.Add(settingName, obj);
                else
                {
                    serialObject.SetSerializedObject(obj);
                    serialObject.Update();
                }
            }
            InitializeCurrentValues(false);
        }

        #region Bindable Instance Properties
        [Obfuscation(Exclude = true)]
        public double BindableProgramWidth
        {
            get { return ProgramWidth; }
        }

        [Obfuscation(Exclude = true)]
        public double BindableProgramHeight
        {
            get { return ProgramHeight; }
        }

        [Obfuscation(Exclude = true)]
        public double BindableProgramScale
        {
            get { return ProgramScale; }
        }

        [Obfuscation(Exclude = true)]
        public WindowState BindableWindowState
        {
            get { return WindowState; }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableApplicationBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ApplicationBackgroundBrush", true);
                return ApplicationBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableControlBackgroundBrush
        {
            get
            {
                if (!IsInDesignMode)
                    return Brushes.Transparent;
                return ConfigurationObjectManager.CreateBrush("ApplicationBackgroundBrush", true);
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableApplicationForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ApplicationForegroundBrush", true);
                return ApplicationForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableApplicationTitleBarBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ApplicationTitleBarBrush", true);
                return ApplicationTitleBarBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemForegroundBrush", true);
                return ListItemForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemBackgroundBrush", true);
                return ListItemBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemDisabledForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemDisabledForegroundBrush", true);
                return ListItemDisabledForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemDisabledBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemDisabledBackgroundBrush", true);
                return ListItemDisabledBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemSelectedForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemSelectedForegroundBrush", true);
                return ListItemSelectedForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemSelectedBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemSelectedBackgroundBrush", true);
                return ListItemSelectedBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemDisabledSelectedForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemDisabledSelectedForegroundBrush", true);
                return ListItemDisabledSelectedForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public static Brush BindableListItemDisabledSelectedBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ListItemDisabledSelectedBackgroundBrush", true);
                return ListItemDisabledSelectedBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonForegroundBrush", true);
                return TabButtonForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonBackgroundBrush", true);
                return TabButtonBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonDisabledForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonDisabledForegroundBrush", true);
                return TabButtonDisabledForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonDisabledBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonDisabledBackgroundBrush", true);
                return TabButtonDisabledBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonSelectedForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonSelectedForegroundBrush", true);
                return TabButtonSelectedForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonSelectedBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonSelectedBackgroundBrush", true);
                return TabButtonSelectedBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonDisabledSelectedForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonDisabledSelectedForegroundBrush", true);
                return TabButtonDisabledSelectedForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTabButtonDisabledSelectedBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TabButtonDisabledSelectedBackgroundBrush", true);
                return TabButtonDisabledSelectedBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableCaretBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("CaretBrush", true);
                return LabelForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableLabelForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("LabelForegroundBrush", true);
                return LabelForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledBorderBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledBorderBrush", true);
                return DisabledBorderBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledLabelForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledLabelForegroundBrush", true);
                return DisabledLabelForegroundBrush;
            }
        }


        [Obfuscation(Exclude = true)]
        public Brush BindableEnabledSelectedButtonForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("EnabledSelectedButtonForegroundBrush", true);
                return EnabledSelectedButtonForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledSelectedButtonForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledSelectedButtonForegroundBrush", true);
                return DisabledSelectedButtonForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableButtonBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ButtonBackgroundBrush", true);
                return ButtonBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableButtonForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("ButtonForegroundBrush", true);
                return ButtonForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableBorderBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("BorderBrush", true);
                return BorderBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledButtonBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledButtonBackgroundBrush", true);
                return DisabledButtonBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledButtonForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledButtonForegroundBrush", true);
                return DisabledButtonForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableSelectedButtonBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("SelectedButtonBackgroundBrush", true);
                return SelectedButtonBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableDisabledSelectedButtonBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("DisabledSelectedButtonBackgroundBrush", true);
                return DisabledSelectedButtonBackgroundBrush;
            }
        }


        [Obfuscation(Exclude = true)]
        public Brush BindableTextboxBackgroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TextboxBackgroundBrush", true);
                return TextboxBackgroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public Brush BindableTextboxForegroundBrush
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBrush("TextboxForegroundBrush", true);
                return TextboxForegroundBrush;
            }
        }

        [Obfuscation(Exclude = true)]
        public FontFamily BindableButtonFontFamily
        {
            get
            {
                if (AlwaysUseDefaults)
                    return new FontFamily(ConfigurationObjectManager.CreateString("ButtonFontFamily", true));
                return ButtonFontFamily;
            }
        }

        [Obfuscation(Exclude = true)]
        public bool BindableForceWasteOnVoid
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBoolean("ForceWasteOnVoid", true);
                return ForceWasteOnVoid;
            }
        }

        [Obfuscation(Exclude = true)]
        public bool BindableUseOnScreenTextEntry
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBoolean("UseOnScreenTextEntry", true);
                return UseOnScreenTextEntry;
            }
        }

        [Obfuscation(Exclude = true)]
        public bool BindableUsePaperlessPrintDestinations
        {
            get
            {
                if (AlwaysUseDefaults)
                    return ConfigurationObjectManager.CreateBoolean("UsePaperlessPrintDestinations", true);
                return UsePaperlessPrintDestinations;
            }
        }

        [Obfuscation(Exclude = true)]
        public string Today
        {
            get { return DateTime.Now.ToString(CultureInfo.InvariantCulture); }
        }
        #endregion
    }

}
