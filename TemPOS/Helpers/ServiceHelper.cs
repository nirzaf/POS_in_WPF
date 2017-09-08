using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;

namespace TemPOS.Helpers
{
    public static class ServiceHelper
    {
        #region ServiceDetails
        public class ServiceDetails
        {
            public string Name
            {
                get;
                private set;
            }
            public string DisplayName
            {
                get;
                private set;
            }
            public string PathName
            {
                get;
                private set;
            }
            public string ServiceType
            {
                get;
                private set;
            }
            public string StartMode
            {
                get;
                private set;
            }
            public static ServiceDetails Create(string name, string displayName, string pathName,
                string serviceType, string startMode)
            {
                return new ServiceDetails {
                    Name = name,
                    DisplayName = displayName,
                    PathName = pathName,
                    ServiceType = serviceType,
                    StartMode = startMode
                };
            }
            public ServiceController GetServiceController()
            {
                foreach (ServiceController service in ServiceController.GetServices())
                {
                    if (service.ServiceName.Equals(Name) &&
                        service.DisplayName.Equals(DisplayName))
                    {
                        return service;
                    }
                }
                return null;
            }
        }
        #endregion

        public static Exception LastException = null;

        public static bool IsSqlServiceLocal
        {
            get
            {
                return (SqlServerServiceController != null);
            }
        }

        public static bool IsSqlBrowserServiceLocal
        {
            get
            {
                return ServiceController.GetServices()
                    .Any(service => service.ServiceName.StartsWith("SQLBrowser"));
            }
        }

        public static bool IsSqlServiceRunningLocally
        {
            get
            {
                ServiceController serviceController = SqlServerServiceController;
                return (serviceController != null) && (serviceController.Status == ServiceControllerStatus.Running);
            }
        }

        public static ServiceController SqlServerServiceController
        {
            get
            {
                foreach (ServiceDetails details in GetServiceDetails())
                {
                    if ((details.PathName == null) ||
                        !details.PathName.Contains("MSSQL11") ||
                        !details.PathName.Contains("sqlservr.exe")) continue;
                    ServiceController serviceController = details.GetServiceController();
                    return serviceController;
                }
                return null;
            }
        }

        public static bool IsSqlBrowserServiceRunningLocally
        {
            get
            {
                return (
                    from service in ServiceController.GetServices()
                    where service.ServiceName.StartsWith("SQLBrowser")
                    select (service.Status == ServiceControllerStatus.Running)).FirstOrDefault();
            }
        }

        public static bool IsTaskManagerAccessServiceInstalled
        {
            get
            {
                return ServiceController.GetServices().Any(service =>
                    service.ServiceName.Equals("Task Manager Access"));
            }
        }

        public static bool IsTaskManagerAccessServiceRunning
        {
            get
            {
                return ServiceController.GetServices().Any(service =>
                    service.ServiceName.Equals("Task Manager Access") &&
                    (service.Status == ServiceControllerStatus.Running));
            }
        }

        public static bool StartTaskManagerAccessService(int timeoutMilliseconds)
        {
            return (
                from service in ServiceController.GetServices()
                where service.ServiceName.StartsWith("Task Manager Access")
                select StartService(service, timeoutMilliseconds)).FirstOrDefault();
        }

        public static bool StopTaskManagerAccessService(int timeoutMilliseconds)
        {
            return (
                from service in ServiceController.GetServices()
                where service.ServiceName.StartsWith("Task Manager Access")
                select StopService(service, timeoutMilliseconds)).FirstOrDefault();
        }

        public static bool StartSqlService(int timeoutMilliseconds)
        {
            return StartService(SqlServerServiceController, timeoutMilliseconds);
        }

        public static bool StopSqlService(int timeoutMilliseconds)
        {
            return StopService(SqlServerServiceController, timeoutMilliseconds);
        }

        public static bool StartSqlBrowserService(int timeoutMilliseconds)
        {
            return (
                from service in ServiceController.GetServices()
                where service.ServiceName.Equals("SQLBrowser")
                select StartService(service, timeoutMilliseconds)).FirstOrDefault();
        }

        public static bool StopSqlBrowserService(int timeoutMilliseconds)
        {
            return (
                from service in ServiceController.GetServices()
                where service.ServiceName.Equals("SQLBrowser")
                select StopService(service, timeoutMilliseconds)).FirstOrDefault();
        }

        private static bool StartService(string serviceName, int timeoutMilliseconds)
        {
            return StartService(new ServiceController(serviceName), timeoutMilliseconds);
        }

        private static bool StartService(ServiceController service, int timeoutMilliseconds)
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }

        private static bool StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            return StopService(service, timeoutMilliseconds);
        }
        
        private static bool StopService(ServiceController service, int timeoutMilliseconds)
        {
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }

        private static bool RestartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch (Exception ex)
            {
                LastException = ex;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the file paths to all services with the specified service name.
        /// </summary>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>The path to each service with the supplied service name</returns>
        public static IEnumerable<ServiceDetails> GetServiceDetails(string serviceName = null)
        {
            String query;
            if (serviceName == null)
                query = "SELECT Name,DisplayName,PathName,ServiceType,StartMode FROM Win32_Service WHERE (NAME IS NOT NULL)";
            else
                query = "SELECT Name,DisplayName,PathName,ServiceType,StartMode FROM Win32_Service WHERE Name = \"" + serviceName + "\"";

            using (ManagementObjectSearcher mos = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject mo in mos.Get())
                {
                    yield return ServiceDetails.Create(
                        (mo["Name"] != null ? mo["Name"].ToString() : null),
                        (mo["DisplayName"] != null ? mo["DisplayName"].ToString() : null),
                        (mo["PathName"] != null ? mo["PathName"].ToString() : null),
                        (mo["ServiceType"] != null ? mo["ServiceType"].ToString() : null),
                        (mo["StartMode"] != null ? mo["StartMode"].ToString() : null));
                }
            }
        }
    }
}
