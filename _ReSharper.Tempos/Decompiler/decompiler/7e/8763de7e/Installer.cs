// Type: System.Configuration.Install.Installer
// Assembly: System.Configuration.Install, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Configuration.Install.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime;
using System.Text;

namespace System.Configuration.Install
{
  [DefaultEvent("AfterInstall")]
  public class Installer : Component
  {
    private InstallerCollection installers;
    private InstallContext context;
    internal Installer parent;
    private InstallEventHandler afterCommitHandler;
    private InstallEventHandler afterInstallHandler;
    private InstallEventHandler afterRollbackHandler;
    private InstallEventHandler afterUninstallHandler;
    private InstallEventHandler beforeCommitHandler;
    private InstallEventHandler beforeInstallHandler;
    private InstallEventHandler beforeRollbackHandler;
    private InstallEventHandler beforeUninstallHandler;
    private const string wrappedExceptionSource = "WrappedExceptionSource";

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public InstallContext Context
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.context;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.context = value;
      }
    }

    [ResDescription("Desc_Installer_HelpText")]
    public virtual string HelpText
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        for (int index = 0; index < this.Installers.Count; ++index)
        {
          string helpText = this.Installers[index].HelpText;
          if (helpText.Length > 0)
          {
            stringBuilder.Append("\r\n");
            stringBuilder.Append(helpText);
          }
        }
        return ((object) stringBuilder).ToString();
      }
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public InstallerCollection Installers
    {
      get
      {
        if (this.installers == null)
          this.installers = new InstallerCollection(this);
        return this.installers;
      }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(true)]
    [TypeConverter(typeof (InstallerParentConverter))]
    [ResDescription("Desc_Installer_Parent")]
    public Installer Parent
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.parent;
      }
      set
      {
        if (value == this)
          throw new InvalidOperationException(Res.GetString("InstallBadParent"));
        if (value == this.parent)
          return;
        if (value != null && this.InstallerTreeContains(value))
          throw new InvalidOperationException(Res.GetString("InstallRecursiveParent"));
        if (this.parent != null)
        {
          int index = this.parent.Installers.IndexOf(this);
          if (index != -1)
            this.parent.Installers.RemoveAt(index);
        }
        this.parent = value;
        if (this.parent == null || this.parent.Installers.Contains(this))
          return;
        this.parent.Installers.Add(this);
      }
    }

    public event InstallEventHandler Committed
    {
      add
      {
        this.afterCommitHandler += value;
      }
      remove
      {
        this.afterCommitHandler -= value;
      }
    }

    public event InstallEventHandler AfterInstall
    {
      add
      {
        this.afterInstallHandler += value;
      }
      remove
      {
        this.afterInstallHandler -= value;
      }
    }

    public event InstallEventHandler AfterRollback
    {
      add
      {
        this.afterRollbackHandler += value;
      }
      remove
      {
        this.afterRollbackHandler -= value;
      }
    }

    public event InstallEventHandler AfterUninstall
    {
      add
      {
        this.afterUninstallHandler += value;
      }
      remove
      {
        this.afterUninstallHandler -= value;
      }
    }

    public event InstallEventHandler Committing
    {
      add
      {
        this.beforeCommitHandler += value;
      }
      remove
      {
        this.beforeCommitHandler -= value;
      }
    }

    public event InstallEventHandler BeforeInstall
    {
      add
      {
        this.beforeInstallHandler += value;
      }
      remove
      {
        this.beforeInstallHandler -= value;
      }
    }

    public event InstallEventHandler BeforeRollback
    {
      add
      {
        this.beforeRollbackHandler += value;
      }
      remove
      {
        this.beforeRollbackHandler -= value;
      }
    }

    public event InstallEventHandler BeforeUninstall
    {
      add
      {
        this.beforeUninstallHandler += value;
      }
      remove
      {
        this.beforeUninstallHandler -= value;
      }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public Installer()
    {
    }

    public virtual void Commit(IDictionary savedState)
    {
      if (savedState == null)
        throw new ArgumentException(Res.GetString("InstallNullParameter", new object[1]
        {
          (object) "savedState"
        }));
      else if (savedState[(object) "_reserved_lastInstallerAttempted"] == null || savedState[(object) "_reserved_nestedSavedStates"] == null)
      {
        throw new ArgumentException(Res.GetString("InstallDictionaryMissingValues", new object[1]
        {
          (object) "savedState"
        }));
      }
      else
      {
        Exception exception1 = (Exception) null;
        try
        {
          this.OnCommitting(savedState);
        }
        catch (Exception ex)
        {
          this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnCommitting", ex);
          this.Context.LogMessage(Res.GetString("InstallCommitException"));
          exception1 = ex;
        }
        int num = (int) savedState[(object) "_reserved_lastInstallerAttempted"];
        IDictionary[] dictionaryArray = (IDictionary[]) savedState[(object) "_reserved_nestedSavedStates"];
        if (num + 1 != dictionaryArray.Length || num >= this.Installers.Count)
        {
          throw new ArgumentException(Res.GetString("InstallDictionaryCorrupted", new object[1]
          {
            (object) "savedState"
          }));
        }
        else
        {
          for (int index = 0; index < this.Installers.Count; ++index)
            this.Installers[index].Context = this.Context;
          for (int index = 0; index <= num; ++index)
          {
            try
            {
              this.Installers[index].Commit(dictionaryArray[index]);
            }
            catch (Exception ex)
            {
              if (!this.IsWrappedException(ex))
              {
                this.Context.LogMessage(Res.GetString("InstallLogCommitException", new object[1]
                {
                  (object) this.Installers[index].ToString()
                }));
                Installer.LogException(ex, this.Context);
                this.Context.LogMessage(Res.GetString("InstallCommitException"));
              }
              exception1 = ex;
            }
          }
          savedState[(object) "_reserved_nestedSavedStates"] = (object) dictionaryArray;
          savedState.Remove((object) "_reserved_lastInstallerAttempted");
          try
          {
            this.OnCommitted(savedState);
          }
          catch (Exception ex)
          {
            this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnCommitted", ex);
            this.Context.LogMessage(Res.GetString("InstallCommitException"));
            exception1 = ex;
          }
          if (exception1 == null)
            return;
          Exception exception2 = exception1;
          if (!this.IsWrappedException(exception1))
          {
            exception2 = (Exception) new InstallException(Res.GetString("InstallCommitException"), exception1);
            exception2.Source = "WrappedExceptionSource";
          }
          throw exception2;
        }
      }
    }

    public virtual void Install(IDictionary stateSaver)
    {
      if (stateSaver == null)
      {
        throw new ArgumentException(Res.GetString("InstallNullParameter", new object[1]
        {
          (object) "stateSaver"
        }));
      }
      else
      {
        try
        {
          this.OnBeforeInstall(stateSaver);
        }
        catch (Exception ex)
        {
          this.WriteEventHandlerError(Res.GetString("InstallSeverityError"), "OnBeforeInstall", ex);
          throw new InvalidOperationException(Res.GetString("InstallEventException", (object) "OnBeforeInstall", (object) this.GetType().FullName), ex);
        }
        int num = -1;
        ArrayList arrayList = new ArrayList();
        try
        {
          for (int index = 0; index < this.Installers.Count; ++index)
            this.Installers[index].Context = this.Context;
          for (int index = 0; index < this.Installers.Count; ++index)
          {
            Installer installer = this.Installers[index];
            IDictionary stateSaver1 = (IDictionary) new Hashtable();
            try
            {
              num = index;
              installer.Install(stateSaver1);
            }
            finally
            {
              arrayList.Add((object) stateSaver1);
            }
          }
        }
        finally
        {
          stateSaver.Add((object) "_reserved_lastInstallerAttempted", (object) num);
          stateSaver.Add((object) "_reserved_nestedSavedStates", (object) arrayList.ToArray(typeof (IDictionary)));
        }
        try
        {
          this.OnAfterInstall(stateSaver);
        }
        catch (Exception ex)
        {
          this.WriteEventHandlerError(Res.GetString("InstallSeverityError"), "OnAfterInstall", ex);
          throw new InvalidOperationException(Res.GetString("InstallEventException", (object) "OnAfterInstall", (object) this.GetType().FullName), ex);
        }
      }
    }

    protected virtual void OnCommitted(IDictionary savedState)
    {
      if (this.afterCommitHandler == null)
        return;
      this.afterCommitHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnAfterInstall(IDictionary savedState)
    {
      if (this.afterInstallHandler == null)
        return;
      this.afterInstallHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnAfterRollback(IDictionary savedState)
    {
      if (this.afterRollbackHandler == null)
        return;
      this.afterRollbackHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnAfterUninstall(IDictionary savedState)
    {
      if (this.afterUninstallHandler == null)
        return;
      this.afterUninstallHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnCommitting(IDictionary savedState)
    {
      if (this.beforeCommitHandler == null)
        return;
      this.beforeCommitHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnBeforeInstall(IDictionary savedState)
    {
      if (this.beforeInstallHandler == null)
        return;
      this.beforeInstallHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnBeforeRollback(IDictionary savedState)
    {
      if (this.beforeRollbackHandler == null)
        return;
      this.beforeRollbackHandler((object) this, new InstallEventArgs(savedState));
    }

    protected virtual void OnBeforeUninstall(IDictionary savedState)
    {
      if (this.beforeUninstallHandler == null)
        return;
      this.beforeUninstallHandler((object) this, new InstallEventArgs(savedState));
    }

    public virtual void Rollback(IDictionary savedState)
    {
      if (savedState == null)
        throw new ArgumentException(Res.GetString("InstallNullParameter", new object[1]
        {
          (object) "savedState"
        }));
      else if (savedState[(object) "_reserved_lastInstallerAttempted"] == null || savedState[(object) "_reserved_nestedSavedStates"] == null)
      {
        throw new ArgumentException(Res.GetString("InstallDictionaryMissingValues", new object[1]
        {
          (object) "savedState"
        }));
      }
      else
      {
        Exception exception1 = (Exception) null;
        try
        {
          this.OnBeforeRollback(savedState);
        }
        catch (Exception ex)
        {
          this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnBeforeRollback", ex);
          this.Context.LogMessage(Res.GetString("InstallRollbackException"));
          exception1 = ex;
        }
        int num = (int) savedState[(object) "_reserved_lastInstallerAttempted"];
        IDictionary[] dictionaryArray = (IDictionary[]) savedState[(object) "_reserved_nestedSavedStates"];
        if (num + 1 != dictionaryArray.Length || num >= this.Installers.Count)
        {
          throw new ArgumentException(Res.GetString("InstallDictionaryCorrupted", new object[1]
          {
            (object) "savedState"
          }));
        }
        else
        {
          for (int index = this.Installers.Count - 1; index >= 0; --index)
            this.Installers[index].Context = this.Context;
          for (int index = num; index >= 0; --index)
          {
            try
            {
              this.Installers[index].Rollback(dictionaryArray[index]);
            }
            catch (Exception ex)
            {
              if (!this.IsWrappedException(ex))
              {
                this.Context.LogMessage(Res.GetString("InstallLogRollbackException", new object[1]
                {
                  (object) this.Installers[index].ToString()
                }));
                Installer.LogException(ex, this.Context);
                this.Context.LogMessage(Res.GetString("InstallRollbackException"));
              }
              exception1 = ex;
            }
          }
          try
          {
            this.OnAfterRollback(savedState);
          }
          catch (Exception ex)
          {
            this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnAfterRollback", ex);
            this.Context.LogMessage(Res.GetString("InstallRollbackException"));
            exception1 = ex;
          }
          if (exception1 == null)
            return;
          Exception exception2 = exception1;
          if (!this.IsWrappedException(exception1))
          {
            exception2 = (Exception) new InstallException(Res.GetString("InstallRollbackException"), exception1);
            exception2.Source = "WrappedExceptionSource";
          }
          throw exception2;
        }
      }
    }

    public virtual void Uninstall(IDictionary savedState)
    {
      Exception exception1 = (Exception) null;
      try
      {
        this.OnBeforeUninstall(savedState);
      }
      catch (Exception ex)
      {
        this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnBeforeUninstall", ex);
        this.Context.LogMessage(Res.GetString("InstallUninstallException"));
        exception1 = ex;
      }
      IDictionary[] dictionaryArray;
      if (savedState != null)
      {
        dictionaryArray = (IDictionary[]) savedState[(object) "_reserved_nestedSavedStates"];
        if (dictionaryArray == null || dictionaryArray.Length != this.Installers.Count)
          throw new ArgumentException(Res.GetString("InstallDictionaryCorrupted", new object[1]
          {
            (object) "savedState"
          }));
      }
      else
        dictionaryArray = new IDictionary[this.Installers.Count];
      for (int index = this.Installers.Count - 1; index >= 0; --index)
        this.Installers[index].Context = this.Context;
      for (int index = this.Installers.Count - 1; index >= 0; --index)
      {
        try
        {
          this.Installers[index].Uninstall(dictionaryArray[index]);
        }
        catch (Exception ex)
        {
          if (!this.IsWrappedException(ex))
          {
            this.Context.LogMessage(Res.GetString("InstallLogUninstallException", new object[1]
            {
              (object) this.Installers[index].ToString()
            }));
            Installer.LogException(ex, this.Context);
            this.Context.LogMessage(Res.GetString("InstallUninstallException"));
          }
          exception1 = ex;
        }
      }
      try
      {
        this.OnAfterUninstall(savedState);
      }
      catch (Exception ex)
      {
        this.WriteEventHandlerError(Res.GetString("InstallSeverityWarning"), "OnAfterUninstall", ex);
        this.Context.LogMessage(Res.GetString("InstallUninstallException"));
        exception1 = ex;
      }
      if (exception1 == null)
        return;
      Exception exception2 = exception1;
      if (!this.IsWrappedException(exception1))
      {
        exception2 = (Exception) new InstallException(Res.GetString("InstallUninstallException"), exception1);
        exception2.Source = "WrappedExceptionSource";
      }
      throw exception2;
    }

    internal bool InstallerTreeContains(Installer target)
    {
      if (this.Installers.Contains(target))
        return true;
      foreach (Installer installer in (CollectionBase) this.Installers)
      {
        if (installer.InstallerTreeContains(target))
          return true;
      }
      return false;
    }

    internal static void LogException(Exception e, InstallContext context)
    {
      bool flag = true;
      for (; e != null; e = e.InnerException)
      {
        if (flag)
        {
          context.LogMessage(e.GetType().FullName + ": " + e.Message);
          flag = false;
        }
        else
          context.LogMessage(Res.GetString("InstallLogInner", (object) e.GetType().FullName, (object) e.Message));
        if (context.IsParameterTrue("showcallstack"))
          context.LogMessage(e.StackTrace);
      }
    }

    private bool IsWrappedException(Exception e)
    {
      if (e is InstallException && e.Source == "WrappedExceptionSource")
        return e.TargetSite.ReflectedType == typeof (Installer);
      else
        return false;
    }

    private void WriteEventHandlerError(string severity, string eventName, Exception e)
    {
      this.Context.LogMessage(Res.GetString("InstallLogError", (object) severity, (object) eventName, (object) this.GetType().FullName));
      Installer.LogException(e, this.Context);
    }
  }
}
