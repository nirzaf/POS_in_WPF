using System;
using System.Reflection;
using System.Windows.Controls;
using PosControls;
using TemPOS.Helpers;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for GeneralSettingsBrushSetupControl.xaml
    /// </summary>
    public partial class GeneralSettingsBrushSetupControl : UserControl
    {
        public GeneralSettingsBrushSetupControl()
        {
            InitializeComponent();
            InitializeBrushes();
        }

        #region Initialize Brushes
        private void InitializeBrushes()
        {
            #region Application
            colorSelectBoxApplicationBackground.InitializeBrush(
                "ApplicationBackgroundBrush", 
                ConfigurationManager.ApplicationBackgroundBrush);
            colorSelectBoxApplicationForeground.InitializeBrush(
                "ApplicationForegroundBrush",
                ConfigurationManager.ApplicationForegroundBrush);
            colorSelectBoxWindowTitleBar.InitializeBrush(
                "ApplicationTitleBarBrush",
                ConfigurationManager.ApplicationTitleBarBrush);
            #endregion
            #region Borders
            colorSelectBoxBorder.InitializeBrush(
                "BorderBrush",
                ConfigurationManager.BorderBrush);
            colorSelectBoxDisabledBorder.InitializeBrush(
                "DisabledBorderBrush",
                ConfigurationManager.DisabledBorderBrush);
            #endregion
            #region Buttons
            colorSelectBoxButtonBackground.InitializeBrush(
                "ButtonBackgroundBrush",
                ConfigurationManager.ButtonBackgroundBrush);
            colorSelectBoxButtonForeground.InitializeBrush(
                "ButtonForegroundBrush",
                ConfigurationManager.ButtonForegroundBrush);
            colorSelectBoxDisabledButtonBackground.InitializeBrush(
                "DisabledButtonBackgroundBrush",
                ConfigurationManager.DisabledButtonBackgroundBrush);
            colorSelectBoxDisabledButtonForeground.InitializeBrush(
                "DisabledButtonForegroundBrush",
                ConfigurationManager.DisabledButtonForegroundBrush);
            colorSelectBoxDisabledSelectedButtonBackground.InitializeBrush(
                "DisabledSelectedButtonBackgroundBrush",
                ConfigurationManager.DisabledSelectedButtonBackgroundBrush);
            colorSelectBoxSelectedButtonBackground.InitializeBrush(
                "SelectedButtonBackgroundBrush",
                ConfigurationManager.SelectedButtonBackgroundBrush);
            colorSelectBoxDisabledSelectedButtonForeground.InitializeBrush(
                "DisabledSelectedButtonForegroundBrush",
                ConfigurationManager.DisabledSelectedButtonForegroundBrush);
            colorSelectBoxEnabledSelectedButtonForeground.InitializeBrush(
                "EnabledSelectedButtonForegroundBrush",
                ConfigurationManager.EnabledSelectedButtonForegroundBrush);
            #endregion
            #region CheckBoxes
            colorSelectBoxEnabledCheckBox.InitializeBrush(
                "EnabledCheckBoxBrush",
                ConfigurationManager.EnabledCheckBoxBrush);
            colorSelectBoxDisabledCheckBox.InitializeBrush(
                "DisabledCheckBoxBrush",
                ConfigurationManager.DisabledCheckBoxBrush);
            colorSelectBoxEnabledSelectedCheckBox.InitializeBrush(
                "EnabledSelectedCheckBoxBrush",
                ConfigurationManager.EnabledSelectedCheckBoxBrush);
            colorSelectBoxDisabledSelectedCheckBox.InitializeBrush(
                "DisabledSelectedCheckBoxBrush",
                ConfigurationManager.DisabledSelectedCheckBoxBrush);
            #endregion
            #region TextBoxes
            colorSelectBoxCaret.InitializeBrush(
                "CaretBrush",
                ConfigurationManager.CaretBrush);
            colorSelectBoxDisabledTextboxBackground.InitializeBrush(
                "DisabledTextboxBackgroundBrush",
                ConfigurationManager.DisabledTextboxBackgroundBrush);
            colorSelectBoxDisabledTextboxForeground.InitializeBrush(
                "DisabledTextboxForegroundBrush",
                ConfigurationManager.DisabledTextboxForegroundBrush);
            colorSelectBoxTextboxBackground.InitializeBrush(
                "TextboxBackgroundBrush",
                ConfigurationManager.TextboxBackgroundBrush);
            colorSelectBoxTextboxForeground.InitializeBrush(
                "TextboxForegroundBrush",
                ConfigurationManager.TextboxForegroundBrush);
            #endregion
            #region ComboBoxes
            colorSelectBoxEnabledComboBoxForeground.InitializeBrush(
                "EnabledComboBoxForegroundBrush",
                ConfigurationManager.EnabledComboBoxForegroundBrush);
            colorSelectBoxDisabledComboBoxForeground.InitializeBrush(
                "DisabledComboBoxForegroundBrush",
                ConfigurationManager.DisabledComboBoxForegroundBrush);
            colorSelectBoxEnabledComboBoxBackground.InitializeBrush(
                "EnabledComboBoxBackgroundBrush",
                ConfigurationManager.EnabledComboBoxBackgroundBrush);
            colorSelectBoxDisabledComboBoxBackground.InitializeBrush(
                "DisabledComboBoxBackgroundBrush",
                ConfigurationManager.DisabledComboBoxBackgroundBrush);
            #endregion
            #region RadioButtons
            colorSelectBoxEnabledRadioButtonBackground.InitializeBrush(
                "EnabledRadioButtonBackgroundBrush",
                ConfigurationManager.EnabledRadioButtonBackgroundBrush);
            colorSelectBoxEnabledSelectedRadioButtonBackground.InitializeBrush(
                "EnabledSelectedRadioButtonBackgroundBrush",
                ConfigurationManager.EnabledSelectedRadioButtonBackgroundBrush);
            colorSelectBoxDisabledRadioButtonBackground.InitializeBrush(
                "DisabledRadioButtonBackgroundBrush",
                ConfigurationManager.DisabledRadioButtonBackgroundBrush);
            colorSelectBoxDisabledSelectedRadioButtonBackground.InitializeBrush(
                "DisabledSelectedRadioButtonBackgroundBrush",
                ConfigurationManager.DisabledSelectedRadioButtonBackgroundBrush);
            #endregion
            #region Tab ToggleButton's
            colorSelectBoxEnabledTabForeground.InitializeBrush(
                "TabButtonForegroundBrush",
                ConfigurationManager.TabButtonForegroundBrush);
            colorSelectBoxEnabledTabBackground.InitializeBrush(
                "TabButtonBackgroundBrush",
                ConfigurationManager.TabButtonBackgroundBrush);
            colorSelectBoxDisabledTabForeground.InitializeBrush(
                "TabButtonDisabledForegroundBrush",
                ConfigurationManager.TabButtonDisabledForegroundBrush);
            colorSelectBoxDisabledTabBackground.InitializeBrush(
                "TabButtonDisabledBackgroundBrush",
                ConfigurationManager.TabButtonDisabledBackgroundBrush);
            colorSelectBoxEnabledSelectedTabForeground.InitializeBrush(
                "TabButtonSelectedForegroundBrush",
                ConfigurationManager.TabButtonSelectedForegroundBrush);
            colorSelectBoxEnabledSelectedTabBackground.InitializeBrush(
                "TabButtonSelectedBackgroundBrush",
                ConfigurationManager.TabButtonSelectedBackgroundBrush);
            colorSelectBoxDisabledSelectedTabForeground.InitializeBrush(
                "TabButtonDisabledSelectedForegroundBrush",
                ConfigurationManager.TabButtonDisabledSelectedForegroundBrush);
            colorSelectBoxDisabledSelectedTabBackground.InitializeBrush(
                "TabButtonDisabledSelectedBackgroundBrush",
                ConfigurationManager.TabButtonDisabledSelectedBackgroundBrush);
            #endregion
            #region List Items
            colorSelectBoxDisabledListItemBackground.InitializeBrush(
                "ListItemDisabledBackgroundBrush",
                ConfigurationManager.ListItemDisabledBackgroundBrush);
            colorSelectBoxEnabledListItemBackground.InitializeBrush(
                "ListItemBackgroundBrush",
                ConfigurationManager.ListItemBackgroundBrush);
            colorSelectBoxSelectedDisabledListItemBackground.InitializeBrush(
                "ListItemDisabledSelectedBackgroundBrush",
                ConfigurationManager.ListItemDisabledSelectedBackgroundBrush);
            colorSelectBoxSelectedEnabledListItemBackground.InitializeBrush(
                "ListItemSelectedBackgroundBrush",
                ConfigurationManager.ListItemSelectedBackgroundBrush);

            colorSelectBoxEnabledListItemForeground.InitializeBrush(
                "ListItemForegroundBrush",
                ConfigurationManager.ListItemForegroundBrush);
            colorSelectBoxDisabledListItemForeground.InitializeBrush(
                "ListItemDisabledForegroundBrush",
                ConfigurationManager.ListItemDisabledForegroundBrush);
            colorSelectBoxEnabledSelectedListItemForeground.InitializeBrush(
                "ListItemSelectedForegroundBrush",
                ConfigurationManager.ListItemSelectedForegroundBrush);
            colorSelectBoxDisabledSelectedListItemForeground.InitializeBrush(
                "ListItemDisabledSelectedForegroundBrush",
                ConfigurationManager.ListItemDisabledSelectedForegroundBrush);
            #endregion
            #region Labels
            colorSelectBoxLabelForeground.InitializeBrush(
                "LabelForegroundBrush",
                ConfigurationManager.LabelForegroundBrush);
            colorSelectBoxDisabledLabelForeground.InitializeBrush(
                "DisabledLabelForegroundBrush",
                ConfigurationManager.DisabledLabelForegroundBrush);
            #endregion
        }
        #endregion

        #region SelectBrushChanged event
        [Obfuscation(Exclude = true)]
        private void colorSelectBox_SelectedBrushChanged(object sender, EventArgs e)
        {
            BrushSelectBox control = (sender as BrushSelectBox);
            if (control != null)
                ConfigurationManager.SetBrush(control.SelectedBrushName, control.SelectedBrush);
            WpfHelper.UpdateBrushBindings();
        }
        #endregion

    }
}
