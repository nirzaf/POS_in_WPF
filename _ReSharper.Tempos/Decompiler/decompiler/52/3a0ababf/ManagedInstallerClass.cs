// Type: System.Configuration.Install.ManagedInstallerClass
// Assembly: System.Configuration.Install, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Configuration.Install.dll

using System;
using System.Collections;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Configuration.Install
{
  [ComVisible(true)]
  [Guid("42EB0342-0393-448f-84AA-D4BEB0283595")]
  public class ManagedInstallerClass : IManagedInstaller
  {
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public ManagedInstallerClass()
    {
    }

    int IManagedInstaller.ManagedInstall(string argString, int hInstall)
    {
      try
      {
        ManagedInstallerClass.InstallHelper(ManagedInstallerClass.StringToArgs(argString));
      }
      catch (Exception ex)
      {
        Exception exception = ex;
        StringBuilder stringBuilder = new StringBuilder();
        while (exception != null)
        {
          stringBuilder.Append(exception.Message);
          exception = exception.InnerException;
          if (exception != null)
            stringBuilder.Append(" --> ");
        }
        int record = System.Configuration.Install.NativeMethods.MsiCreateRecord(2);
        if (record != 0 && System.Configuration.Install.NativeMethods.MsiRecordSetInteger(record, 1, 1001) == 0 && System.Configuration.Install.NativeMethods.MsiRecordSetStringW(record, 2, ((object) stringBuilder).ToString()) == 0)
          System.Configuration.Install.NativeMethods.MsiProcessMessage(hInstall, 16777216, record);
        return -1;
      }
      return 0;
    }

    public static void InstallHelper(string[] args)
    {
      bool flag1 = false;
      bool flag2 = false;
      TransactedInstaller transactedInstaller = new TransactedInstaller();
      bool flag3 = false;
      try
      {
        ArrayList arrayList = new ArrayList();
        for (int index = 0; index < args.Length; ++index)
        {
          if (args[index].StartsWith("/", StringComparison.Ordinal) || args[index].StartsWith("-", StringComparison.Ordinal))
          {
            string strA = args[index].Substring(1);
            if (string.Compare(strA, "u", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
              flag1 = true;
            else if (string.Compare(strA, "?", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "help", StringComparison.OrdinalIgnoreCase) == 0)
              flag3 = true;
            else if (string.Compare(strA, "AssemblyName", StringComparison.OrdinalIgnoreCase) == 0)
              flag2 = true;
            else
              arrayList.Add((object) args[index]);
          }
          else
          {
            Assembly assembly;
            try
            {
              assembly = !flag2 ? Assembly.LoadFrom(args[index]) : Assembly.Load(args[index]);
            }
            catch (Exception ex)
            {
              if (args[index].IndexOf('=') != -1)
                throw new ArgumentException(Res.GetString("InstallFileDoesntExistCommandLine", new object[1]
                {
                  (object) args[index]
                }), ex);
              else
                throw;
            }
            AssemblyInstaller assemblyInstaller = new AssemblyInstaller(assembly, (string[]) arrayList.ToArray(typeof (string)));
            transactedInstaller.Installers.Add((Installer) assemblyInstaller);
          }
        }
        if (flag3 || transactedInstaller.Installers.Count == 0)
        {
          flag3 = true;
          transactedInstaller.Installers.Add((Installer) new AssemblyInstaller());
          throw new InvalidOperationException(ManagedInstallerClass.GetHelp((Installer) transactedInstaller));
        }
        else
          transactedInstaller.Context = new InstallContext("InstallUtil.InstallLog", (string[]) arrayList.ToArray(typeof (string)));
      }
      catch (Exception ex)
      {
        if (flag3)
          throw ex;
        throw new InvalidOperationException(Res.GetString("InstallInitializeException", (object) ex.GetType().FullName, (object) ex.Message));
      }
      try
      {
        string strA1 = transactedInstaller.Context.Parameters["installtype"];
        if (strA1 != null && string.Compare(strA1, "notransaction", StringComparison.OrdinalIgnoreCase) == 0)
        {
          string strA2 = transactedInstaller.Context.Parameters["action"];
          if (strA2 != null && string.Compare(strA2, "rollback", StringComparison.OrdinalIgnoreCase) == 0)
          {
            transactedInstaller.Context.LogMessage(Res.GetString("InstallRollbackNtRun"));
            for (int index = 0; index < transactedInstaller.Installers.Count; ++index)
              transactedInstaller.Installers[index].Rollback((IDictionary) null);
          }
          else if (strA2 != null && string.Compare(strA2, "commit", StringComparison.OrdinalIgnoreCase) == 0)
          {
            transactedInstaller.Context.LogMessage(Res.GetString("InstallCommitNtRun"));
            for (int index = 0; index < transactedInstaller.Installers.Count; ++index)
              transactedInstaller.Installers[index].Commit((IDictionary) null);
          }
          else if (strA2 != null && string.Compare(strA2, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
          {
            transactedInstaller.Context.LogMessage(Res.GetString("InstallUninstallNtRun"));
            for (int index = 0; index < transactedInstaller.Installers.Count; ++index)
              transactedInstaller.Installers[index].Uninstall((IDictionary) null);
          }
          else
          {
            transactedInstaller.Context.LogMessage(Res.GetString("InstallInstallNtRun"));
            for (int index = 0; index < transactedInstaller.Installers.Count; ++index)
              transactedInstaller.Installers[index].Install((IDictionary) null);
          }
        }
        else if (!flag1)
        {
          IDictionary stateSaver = (IDictionary) new Hashtable();
          transactedInstaller.Install(stateSaver);
        }
        else
          transactedInstaller.Uninstall((IDictionary) null);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    private static string GetHelp(Installer installerWithHelp)
    {
      return Res.GetString("InstallHelpMessageStart") + Environment.NewLine + installerWithHelp.HelpText + Environment.NewLine + Res.GetString("InstallHelpMessageEnd") + Environment.NewLine;
    }

    private static string[] StringToArgs(string cmdLine)
    {
      ArrayList arrayList = new ArrayList();
      StringBuilder stringBuilder = (StringBuilder) null;
      bool flag1 = false;
      bool flag2 = false;
      for (int index = 0; index < cmdLine.Length; ++index)
      {
        char c = cmdLine[index];
        if (stringBuilder == null)
        {
          if (!char.IsWhiteSpace(c))
            stringBuilder = new StringBuilder();
          else
            continue;
        }
        if (flag1)
        {
          if (flag2)
          {
            if ((int) c != 92 && (int) c != 34)
              stringBuilder.Append('\\');
            flag2 = false;
            stringBuilder.Append(c);
          }
          else if ((int) c == 34)
            flag1 = false;
          else if ((int) c == 92)
            flag2 = true;
          else
            stringBuilder.Append(c);
        }
        else if (char.IsWhiteSpace(c))
        {
          arrayList.Add((object) ((object) stringBuilder).ToString());
          stringBuilder = (StringBuilder) null;
          flag2 = false;
        }
        else if (flag2)
        {
          stringBuilder.Append(c);
          flag2 = false;
        }
        else if ((int) c == 94)
          flag2 = true;
        else if ((int) c == 34)
          flag1 = true;
        else
          stringBuilder.Append(c);
      }
      if (stringBuilder != null)
        arrayList.Add((object) ((object) stringBuilder).ToString());
      string[] strArray = new string[arrayList.Count];
      arrayList.CopyTo((Array) strArray);
      return strArray;
    }
  }
}
