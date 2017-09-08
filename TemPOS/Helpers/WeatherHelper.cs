using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows;
using PosModels;
using PosModels.Types;

namespace TemPOS.Helpers
{
    public static class WeatherHelper
    {

        private const int BufferSize = 250000;
        private static readonly Window DispatchingWindow = new Window();
        private static TemperatureScale _temperatureScale = TemperatureScale.Fahrenheit;

        [Obfuscation(Exclude = true)]
        public static event EventHandler QueryCompleted;
        
        [Obfuscation(Exclude = true)]
        public static event EventHandler QueryFailed;

        public static TemperatureScale Scale
        {
            get { return _temperatureScale; }
            set
            {
                if (_temperatureScale == value) return;

                CurrentTemperature = _temperatureScale.ConvertTo(value, CurrentTemperature);
                _temperatureScale = value;
                switch (Scale)
                {
                    case TemperatureScale.Kelvin:
                        CurrentWeather = ((int)CurrentTemperature) + "K " + CurrentConditions;
                        break;
                    case TemperatureScale.Fahrenheit:
                        CurrentWeather = ((int)CurrentTemperature) + "°F " + CurrentConditions;
                        break;
                    case TemperatureScale.Celsius:
                        CurrentWeather = ((int)CurrentTemperature) + "°C " + CurrentConditions;
                        break;
                }

                // Notify weather was updated
                if (QueryCompleted != null)
                    QueryCompleted.Invoke(null, new EventArgs());
            }
        }

        public static string CurrentWeather
        {
            get;
            private set;
        }

        public static string CurrentConditions
        {
            get;
            private set;
        }

        public static double CurrentTemperature
        {
            get;
            private set;
        }

        public static int CurrentZipCode
        {
            get;
            private set;
        }

        public static string ServerUri
        {
            get;
            private set;
        }

        public static bool IsActive
        {
            get;
            private set;
        }

        public static Thread DownloadThread
        {
            get;
            private set;
        }

        public static int BytesReceived
        {
            get;
            private set;
        }

        public static void QueryCurrentWeather(int zipCode)
        {
            if (!IsActive)
            {
                ZipCode zipCodeDb = ZipCode.Get(zipCode);
                if (zipCodeDb != null)
                {
                    ZipCodeCity zipCodeCity = ZipCodeCity.Get(zipCodeDb.CityId);
                    ServerUri = "http://forecast.weather.gov/MapClick.php?lat=" +
                        zipCodeCity.Latitude + "&lon=" +
                        zipCodeCity.Longitude + "&site=all&smap=1";
                    CurrentZipCode = zipCode;
                    IsActive = true;
                    CurrentWeather = "";
                    CurrentTemperature = -1000;
                    BytesReceived = 0;
                    DownloadThread = new Thread(DownloadWebFileStart);
                    DownloadThread.Start();
                }
                else
                {
                    if (QueryFailed != null)
                        QueryFailed.Invoke(typeof(WeatherHelper), new EventArgs());
                }
            }
        }

        private static void DownloadWebFileStart()
        {
            string pageText = "";
            HttpWebRequest request = WebRequest.Create(ServerUri) as HttpWebRequest;
            if (request == null) return;
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                if (response != null)
                    using (Stream input = response.GetResponseStream())
                    {
                        int bytesRead;
                        byte[] buffer = new byte[BufferSize];
                        while (input != null && (bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            BytesReceived += bytesRead;
                            pageText += Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        }
                    }
                request.Abort();
                DispatchingWindow.Dispatcher.Invoke((Action)(() =>
                {
                    try
                    {
                        ParsePageText(pageText);
                        if (QueryCompleted != null)
                            QueryCompleted.Invoke(typeof(WeatherHelper), new EventArgs());
                    }
                    catch
                    {
                        if (QueryFailed != null)
                            QueryFailed.Invoke(typeof(WeatherHelper), new EventArgs());
                    }
                }));
                IsActive = false;
            }
            catch (WebException)
            {
                request.Abort();
                IsActive = false;
            }
        }

        private static void ParsePageText(string pageText)
        {
            // Conditions
            string frontSearchString = "<p class=\"myforecast-current\">";
            string backSearchString = "</p>";
            string conditions = StringExtractBetween(pageText, frontSearchString, backSearchString);
            // Temp
            frontSearchString = "<p class=\"myforecast-current-lrg\">";
            backSearchString = "</p>";
            string temp = StringExtractBetween(pageText, frontSearchString, backSearchString);

            CurrentConditions = conditions;
            double currentTemperature = Convert.ToDouble(temp.Substring(0, temp.IndexOf("°", StringComparison.Ordinal)));
            CurrentTemperature = TemperatureScale.Fahrenheit.ConvertTo(Scale, currentTemperature);
            CurrentWeather = Scale.ToString(CurrentTemperature) + " " + conditions;
        }

        private static string StringExtractBetween(string originalString, string frontSearchString, string backSearchString)
        {
            int index = originalString.IndexOf(frontSearchString, StringComparison.Ordinal) + frontSearchString.Length;
            originalString = originalString.Substring(index, originalString.Length - index);
            originalString = originalString.Substring(0, originalString.IndexOf(backSearchString, StringComparison.Ordinal));
            originalString = originalString.Replace("&deg;", "°");
            return originalString;
        }
    }
}
