using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace TemposClientAdministration.Helpers
{
    public static class UpdateBuilder
    {
        private static Encoding ZipEncoding = Encoding.Unicode;
        public static StringDelegate PrintLine;

        public static void Build()
        {
            // Stop the update server
            bool wasRunning = UpdateServer.IsRunning;
            if (wasRunning)
            {
                PrintLine("Stopping Update Server...");
                UpdateServer.Stopped += new EventHandler(UpdateServer_Stopped);
                UpdateServer.Stop();
            }
            else
            {
                BuildNow(false);
            }
        }

        static void UpdateServer_Stopped(object sender, EventArgs e)
        {
            UpdateServer.Stopped -= UpdateServer_Stopped;
            BuildNow(true);
        }

        private static void BuildNow(bool wasRunning)
        {
#if DEBUG
            string build = "Debug";
            PrintLine("Warning: Building a DEBUG update!!!");
#else
            string build = "Release";
#endif
            string rootPath = @"D:\Viipe.com\PointOfSale";
            string appPath = @"D:\Viipe.com\PointOfSale\PointOfSale\bin\" + build;
            string sqlDLLPath = @"D:\Viipe.com\PointOfSale\SQL Procedures\bin\" + build;
            string updateInstallerPath =
                @"D:\Viipe.com\PointOfSale\TemposUpdateInstaller\bin\" + build;
            string zipPath = rootPath + @"\update.zip";

            // Remove the old one
            if (File.Exists(zipPath))
                File.Delete(zipPath);

            // Create the new one
            PrintLine("Creating Zip File...");
            ZipFile zipFile = new ZipFile(zipPath, ZipEncoding);
            zipFile.UseZip64WhenSaving = Zip64Option.Always;
            zipFile.UseUnicodeAsNecessary = true;
            zipFile.CaseSensitiveRetrieval = true;

            // Add Contents
            AddFile(zipFile, appPath + @"\PointOfSale.exe");
            AddFile(zipFile, appPath + @"\PosModels.dll");
            AddFile(zipFile, appPath + @"\PosControls.dll");
            AddFile(zipFile, appPath + @"\TemposLibrary.dll");
            AddFile(zipFile, sqlDLLPath + @"\TemposProcs.dll");
            AddFile(zipFile, updateInstallerPath + @"\TemposUpdateInstaller.exe");

            // Save the zipFile
            PrintLine("Saving Zip File...");
            zipFile.Save();

            if (wasRunning)
            {
                PrintLine("Restarting Update Server...");
                UpdateServer.Start();
            }

            PrintLine("Build Update Completed");
        }

        private static void AddFile(ZipFile zipFile, string path)
        {
            PrintLine("Adding \"" + path + "\"...");
            zipFile.AddFile(path, "");
        }
    }
}
