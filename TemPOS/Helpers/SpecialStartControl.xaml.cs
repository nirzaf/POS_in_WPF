using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace TemPOS.Helpers
{
    /// <summary>
    /// Interaction logic for SpecialStartControl.xaml
    /// </summary>
    public partial class SpecialStartControl : UserControl
    {
        MediaPlayer player = new MediaPlayer();
        public SpecialStartControl()
        {
            InitializeComponent();
            player.MediaEnded += player_MediaEnded;
            player.Open(new Uri(@"G:\VistaUsers\Naquada\Music\Sound Effects\Cheers.wav", UriKind.Absolute));

            this.Loaded += SpecialStartControl_Loaded;
            WeatherHelper.QueryCompleted += WeatherHelper_QueryCompleted;
        }

        [Obfuscation(Exclude = true)]
        void SpecialStartControl_Loaded(object sender, RoutedEventArgs e)
        {
            WeatherHelper.QueryCurrentWeather(53704);
        }

        [Obfuscation(Exclude = true)]
        void WeatherHelper_QueryCompleted(object sender, EventArgs e)
        {
            //textBox.Text += WeatherHelper.CurrentWeather;
            //textBox.Text =
            //    WeatherHelper.ServerUri + Environment.NewLine +
            //    WeatherHelper.CurrentConditions + " : " + WeatherHelper.CurrentTemperature;
        }

        [Obfuscation(Exclude = true)]
        void player_MediaEnded(object sender, EventArgs e)
        {
            player.Stop();
            player.Position = new TimeSpan(0, 0, 0);
        }

        [Obfuscation(Exclude = true)]
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            player.Play();
        }
        
        [Obfuscation(Exclude = true)]
        private void TextBlockButton_Click(object sender, RoutedEventArgs e)
        {
            WeatherHelper.QueryCurrentWeather(54915);
        }

        [Obfuscation(Exclude = true)]
        private void CustomTextBox_CommitEdit(object sender, EventArgs e)
        {
            int i = 0;
        }

    }
}
