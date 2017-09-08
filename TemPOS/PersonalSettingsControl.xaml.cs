using System;
using System.Windows;
using PosControls;
using PosModels;
using PosModels.Types;
using TemPOS.Helpers;
using TemPOS.Managers;
using Strings = TemPOS.Types.Strings;

namespace TemPOS
{
    /// <summary>
    /// Interaction logic for PersonalSettingsControl.xaml
    /// </summary>
    public partial class PersonalSettingsControl
    {
        public PersonalSettingsControl()
        {
            InitializeComponent();
#if DEBUG
            radioButtonLangDebug.Visibility = Visibility.Visible;
            rowDefinitionDebug.Height = new GridLength(40, GridUnitType.Star);
#endif
            StringsCore.LanguageChanged += StringsCore_LanguageChanged;
            SetCurrentLanguage();
            SetCurrentTemperature();
        }

        void StringsCore_LanguageChanged(object sender, EventArgs e)
        {
            groupBoxLanguage.Header = Types.Strings.LanguageLanguage;
            radioButtonLangEnglish.Text = Types.Strings.LanguageEnglish;
            radioButtonLangSpanish.Text = Types.Strings.LanguageSpanish;
            radioButtonLangDutch.Text = Types.Strings.LanguageDutch;
            radioButtonLangFrench.Text = Types.Strings.LanguageFrench;
            radioButtonLangGerman.Text = Types.Strings.LanguageGerman;
            radioButtonLangItalian.Text = Types.Strings.LanguageItalian;
            radioButtonLangPortuguese.Text = Types.Strings.LanguagePortuguese;

            groupBoxTemperature.Header = Types.Strings.SettingsPersonalSettings;
            radioButtonLangFahrenheit.Text = Types.Strings.SettingsFahrenheit;
            radioButtonLangCelsius.Text = Types.Strings.SettingsCelsius;
            radioButtonLangKelvin.Text = Types.Strings.SettingsKelvin;
        }

        private void SetCurrentTemperature()
        {
            radioButtonLangCelsius.IsSelected = (WeatherHelper.Scale == TemperatureScale.Celsius);
            radioButtonLangFahrenheit.IsSelected = (WeatherHelper.Scale == TemperatureScale.Fahrenheit);
            radioButtonLangKelvin.IsSelected = (WeatherHelper.Scale == TemperatureScale.Kelvin);
        }

        private void SetCurrentLanguage()
        {
            radioButtonLangDutch.IsSelected = (StringsCore.Language == Languages.Dutch);
            radioButtonLangEnglish.IsSelected = (StringsCore.Language == Languages.English);
            radioButtonLangFrench.IsSelected = (StringsCore.Language == Languages.French);
            radioButtonLangGerman.IsSelected = (StringsCore.Language == Languages.German);
            radioButtonLangItalian.IsSelected = (StringsCore.Language == Languages.Italian);
            radioButtonLangPortuguese.IsSelected = (StringsCore.Language == Languages.Portuguese);
            radioButtonLangSpanish.IsSelected = (StringsCore.Language == Languages.Spanish);
#if DEBUG
            radioButtonLangDebug.IsSelected = (StringsCore.Language == Languages.Debug);
#endif
        }

        private void UpdateEmployeeLanguageSetting()
        {
            if (SessionManager.ActiveEmployee != null)
                EmployeeSetting.Set(SessionManager.ActiveEmployee.Id, "Language", (int)StringsCore.Language);
        }

        private void UpdateEmployeeTemperatureSetting()
        {
            if (SessionManager.ActiveEmployee != null)
                EmployeeSetting.Set(SessionManager.ActiveEmployee.Id, "TemperatureScale", (int)WeatherHelper.Scale);
        }

        private void radioButtonLangEnglish_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.English;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangSpanish_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.Spanish;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangFrench_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.French;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangGerman_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.German;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangItalian_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.Italian;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangDutch_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.Dutch;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangPortuguese_SelectionGained(object sender, EventArgs e)
        {
            StringsCore.Language = Languages.Portuguese;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
        }

        private void radioButtonLangDebug_SelectionGained(object sender, EventArgs e)
        {
#if DEBUG
            StringsCore.Language = Languages.Debug;
            SetCurrentLanguage();
            UpdateEmployeeLanguageSetting();
#endif
        }

        private void radioButtonLangFahrenheit_SelectionGained(object sender, EventArgs e)
        {
            WeatherHelper.Scale = TemperatureScale.Fahrenheit;
            SetCurrentTemperature();
            UpdateEmployeeTemperatureSetting();
        }

        private void radioButtonLangCelsius_SelectionGained(object sender, EventArgs e)
        {
            WeatherHelper.Scale = TemperatureScale.Celsius;
            SetCurrentTemperature();
            UpdateEmployeeTemperatureSetting();
        }

        private void radioButtonLangKelvin_SelectionGained(object sender, EventArgs e)
        {
            WeatherHelper.Scale = TemperatureScale.Kelvin;
            SetCurrentTemperature();
            UpdateEmployeeTemperatureSetting();
        }

        public static PosDialogWindow CreateInDefaultWindow()
        {
            var control = new PersonalSettingsControl();
#if DEBUG
            return new PosDialogWindow(control, Types.Strings.SettingsPersonalSettings, 710, 445);
#else
            return new PosDialogWindow(control, Strings.SettingsPersonalSettings, 710, 395);
#endif
        }

    }
}
