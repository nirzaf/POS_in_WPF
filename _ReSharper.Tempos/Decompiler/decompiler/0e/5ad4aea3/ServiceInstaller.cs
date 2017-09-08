// Type: System.ServiceProcess.ServiceInstaller
// Assembly: System.ServiceProcess, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.ServiceProcess.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Globalization;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System.ServiceProcess
{
  public class ServiceInstaller : ComponentInstaller
  {
    private string serviceName = "";
    private string displayName = "";
    private string description = "";
    private string[] servicesDependedOn = new string[0];
    private ServiceStartMode startType = ServiceStartMode.Manual;
    private EventLogInstaller eventLogInstaller;
    private bool delayedStartMode;
    private static bool environmentChecked;
    private static bool isWin9x;
    private const string NetworkServiceName = "NT AUTHORITY\\NetworkService";
    private const string LocalServiceName = "NT AUTHORITY\\LocalService";

    [DefaultValue("")]
    [ServiceProcessDescription("ServiceInstallerDisplayName")]
    public string DisplayName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.displayName;
      }
      set
      {
        if (value == null)
          value = "";
        this.displayName = value;
      }
    }

    [DefaultValue("")]
    [ComVisible(false)]
    [ServiceProcessDescription("ServiceInstallerDescription")]
    public string Description
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.description;
      }
      set
      {
        if (value == null)
          value = "";
        this.description = value;
      }
    }

    [ServiceProcessDescription("ServiceInstallerServicesDependedOn")]
    public string[] ServicesDependedOn
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.servicesDependedOn;
      }
      set
      {
        if (value == null)
          value = new string[0];
        this.servicesDependedOn = value;
      }
    }

    [ServiceProcessDescription("ServiceInstallerServiceName")]
    [TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    [DefaultValue("")]
    public string ServiceName
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.serviceName;
      }
      set
      {
        if (value == null)
          value = "";
        if (!ServiceController.ValidServiceName(value))
        {
          throw new ArgumentException(System.ServiceProcess.Res.GetString("ServiceName", (object) value, (object) 80.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
        }
        else
        {
          this.serviceName = value;
          this.eventLogInstaller.Source = value;
        }
      }
    }

    [DefaultValue(ServiceStartMode.Manual)]
    [ServiceProcessDescription("ServiceInstallerStartType")]
    public ServiceStartMode StartType
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.startType;
      }
      set
      {
        if (!Enum.IsDefined(typeof (ServiceStartMode), (object) value))
          throw new InvalidEnumArgumentException("value", (int) value, typeof (ServiceStartMode));
        this.startType = value;
      }
    }

    [DefaultValue(false)]
    [ServiceProcessDescription("ServiceInstallerDelayedAutoStart")]
    public bool DelayedAutoStart
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.delayedStartMode;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.delayedStartMode = value;
      }
    }

    static ServiceInstaller()
    {
    }

    public ServiceInstaller()
    {
      this.eventLogInstaller = new EventLogInstaller();
      this.eventLogInstaller.Log = "Application";
      this.eventLogInstaller.Source = "";
      this.eventLogInstaller.UninstallAction = UninstallAction.Remove;
      this.Installers.Add((Installer) this.eventLogInstaller);
    }

    public override void CopyFromComponent(IComponent component)
    {
      if (!(component is ServiceBase))
        throw new ArgumentException(System.ServiceProcess.Res.GetString("NotAService"));
      this.ServiceName = ((ServiceBase) component).ServiceName;
    }

    public override void Install(IDictionary stateSaver)
    {
      this.Context.LogMessage(System.ServiceProcess.Res.GetString("InstallingService", new object[1]
      {
        (object) this.ServiceName
      }));
      try
      {
        ServiceInstaller.CheckEnvironment();
        string servicesStartName = (string) null;
        string password = (string) null;
        ServiceProcessInstaller processInstaller = (ServiceProcessInstaller) null;
        if (this.Parent is ServiceProcessInstaller)
        {
          processInstaller = (ServiceProcessInstaller) this.Parent;
        }
        else
        {
          for (int index = 0; index < this.Parent.Installers.Count; ++index)
          {
            if (this.Parent.Installers[index] is ServiceProcessInstaller)
            {
              processInstaller = (ServiceProcessInstaller) this.Parent.Installers[index];
              break;
            }
          }
        }
        if (processInstaller == null)
          throw new InvalidOperationException(System.ServiceProcess.Res.GetString("NoInstaller"));
        switch (processInstaller.Account)
        {
          case ServiceAccount.LocalService:
            servicesStartName = "NT AUTHORITY\\LocalService";
            break;
          case ServiceAccount.NetworkService:
            servicesStartName = "NT AUTHORITY\\NetworkService";
            break;
          case ServiceAccount.User:
            servicesStartName = processInstaller.Username;
            password = processInstaller.Password;
            break;
        }
        string binaryPath = this.Context.Parameters["assemblypath"];
        if (string.IsNullOrEmpty(binaryPath))
          throw new InvalidOperationException(System.ServiceProcess.Res.GetString("FileName"));
        if (binaryPath.IndexOf('"') == -1)
          binaryPath = "\"" + binaryPath + "\"";
        if (!ServiceInstaller.ValidateServiceName(this.ServiceName))
          throw new InvalidOperationException(System.ServiceProcess.Res.GetString("ServiceName", (object) this.ServiceName, (object) 80.ToString((IFormatProvider) CultureInfo.CurrentCulture)));
        else if (this.DisplayName.Length > (int) byte.MaxValue)
        {
          throw new ArgumentException(System.ServiceProcess.Res.GetString("DisplayNameTooLong", new object[1]
          {
            (object) this.DisplayName
          }));
        }
        else
        {
          string dependencies = (string) null;
          if (this.ServicesDependedOn.Length > 0)
          {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < this.ServicesDependedOn.Length; ++index)
            {
              string name = this.ServicesDependedOn[index];
              try
              {
                name = new ServiceController(name, ".").ServiceName;
              }
              catch
              {
              }
              stringBuilder.Append(name);
              stringBuilder.Append(char.MinValue);
            }
            stringBuilder.Append(char.MinValue);
            dependencies = ((object) stringBuilder).ToString();
          }
          IntPtr num1 = SafeNativeMethods.OpenSCManager((string) null, (string) null, 983103);
          IntPtr num2 = IntPtr.Zero;
          if (num1 == IntPtr.Zero)
          {
            throw new InvalidOperationException(System.ServiceProcess.Res.GetString("OpenSC", new object[1]
            {
              (object) "."
            }), (Exception) new Win32Exception());
          }
          else
          {
            int serviceType = 16;
            int num3 = 0;
            for (int index = 0; index < this.Parent.Installers.Count; ++index)
            {
              if (this.Parent.Installers[index] is ServiceInstaller)
              {
                ++num3;
                if (num3 > 1)
                  break;
              }
            }
            if (num3 > 1)
              serviceType = 32;
            try
            {
              num2 = System.ServiceProcess.NativeMethods.CreateService(num1, this.ServiceName, this.DisplayName, 983551, serviceType, (int) this.StartType, 1, binaryPath, (string) null, IntPtr.Zero, dependencies, servicesStartName, password);
              if (num2 == IntPtr.Zero)
                throw new Win32Exception();
              if (this.Description.Length != 0)
              {
                System.ServiceProcess.NativeMethods.SERVICE_DESCRIPTION serviceDesc = new System.ServiceProcess.NativeMethods.SERVICE_DESCRIPTION();
                serviceDesc.description = Marshal.StringToHGlobalUni(this.Description);
                bool flag = System.ServiceProcess.NativeMethods.ChangeServiceConfig2(num2, 1U, ref serviceDesc);
                Marshal.FreeHGlobal(serviceDesc.description);
                if (!flag)
                  throw new Win32Exception();
              }
              if (Environment.OSVersion.Version.Major > 5 && this.StartType == ServiceStartMode.Automatic)
              {
                if (!System.ServiceProcess.NativeMethods.ChangeServiceConfig2(num2, 3U, ref new System.ServiceProcess.NativeMethods.SERVICE_DELAYED_AUTOSTART_INFO()
                {
                  fDelayedAutostart = this.DelayedAutoStart
                }))
                  throw new Win32Exception();
              }
              stateSaver[(object) "installed"] = (object) true;
            }
            finally
            {
              if (num2 != IntPtr.Zero)
                SafeNativeMethods.CloseServiceHandle(num2);
              SafeNativeMethods.CloseServiceHandle(num1);
            }
            this.Context.LogMessage(System.ServiceProcess.Res.GetString("InstallOK", new object[1]
            {
              (object) this.ServiceName
            }));
          }
        }
      }
      finally
      {
        base.Install(stateSaver);
      }
    }

    public override bool IsEquivalentInstaller(ComponentInstaller otherInstaller)
    {
      ServiceInstaller serviceInstaller = otherInstaller as ServiceInstaller;
      if (serviceInstaller == null)
        return false;
      else
        return serviceInstaller.ServiceName == this.ServiceName;
    }

    public override void Rollback(IDictionary savedState)
    {
      base.Rollback(savedState);
      object obj = savedState[(object) "installed"];
      if (obj == null || !(bool) obj)
        return;
      this.RemoveService();
    }

    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
      this.RemoveService();
    }

    internal static void CheckEnvironment()
    {
      if (ServiceInstaller.environmentChecked)
      {
        if (ServiceInstaller.isWin9x)
          throw new PlatformNotSupportedException(System.ServiceProcess.Res.GetString("CantControlOnWin9x"));
      }
      else
      {
        ServiceInstaller.isWin9x = Environment.OSVersion.Platform != PlatformID.Win32NT;
        ServiceInstaller.environmentChecked = true;
        if (ServiceInstaller.isWin9x)
          throw new PlatformNotSupportedException(System.ServiceProcess.Res.GetString("CantInstallOnWin9x"));
      }
    }

    private void RemoveService()
    {
      this.Context.LogMessage(System.ServiceProcess.Res.GetString("ServiceRemoving", new object[1]
      {
        (object) this.ServiceName
      }));
      IntPtr num1 = SafeNativeMethods.OpenSCManager((string) null, (string) null, 983103);
      if (num1 == IntPtr.Zero)
        throw new Win32Exception();
      IntPtr num2 = IntPtr.Zero;
      try
      {
        num2 = System.ServiceProcess.NativeMethods.OpenService(num1, this.ServiceName, 65536);
        if (num2 == IntPtr.Zero)
          throw new Win32Exception();
        System.ServiceProcess.NativeMethods.DeleteService(num2);
      }
      finally
      {
        if (num2 != IntPtr.Zero)
          SafeNativeMethods.CloseServiceHandle(num2);
        SafeNativeMethods.CloseServiceHandle(num1);
      }
      this.Context.LogMessage(System.ServiceProcess.Res.GetString("ServiceRemoved", new object[1]
      {
        (object) this.ServiceName
      }));
      try
      {
        using (ServiceController serviceController = new ServiceController(this.ServiceName))
        {
          if (serviceController.Status != ServiceControllerStatus.Stopped)
          {
            this.Context.LogMessage(System.ServiceProcess.Res.GetString("TryToStop", new object[1]
            {
              (object) this.ServiceName
            }));
            serviceController.Stop();
            int num3 = 10;
            serviceController.Refresh();
            while (serviceController.Status != ServiceControllerStatus.Stopped)
            {
              if (num3 > 0)
              {
                Thread.Sleep(1000);
                serviceController.Refresh();
                --num3;
              }
              else
                break;
            }
          }
        }
      }
      catch
      {
      }
      Thread.Sleep(5000);
    }

    private bool ShouldSerializeServicesDependedOn()
    {
      return this.servicesDependedOn != null && this.servicesDependedOn.Length > 0;
    }

    private static bool ValidateServiceName(string name)
    {
      if (name == null || name.Length == 0 || name.Length > 80)
        return false;
      char[] chArray = name.ToCharArray();
      for (int index = 0; index < chArray.Length; ++index)
      {
        if ((int) chArray[index] < 32 || (int) chArray[index] == 47 || (int) chArray[index] == 92)
          return false;
      }
      return true;
    }
  }
}
