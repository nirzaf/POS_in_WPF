// Type: System.Configuration.Install.InstallEventArgs
// Assembly: System.Configuration.Install, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Configuration.Install.dll

using System;
using System.Collections;
using System.Runtime;

namespace System.Configuration.Install
{
  public class InstallEventArgs : EventArgs
  {
    private IDictionary savedState;

    public IDictionary SavedState
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.savedState;
      }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public InstallEventArgs()
    {
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public InstallEventArgs(IDictionary savedState)
    {
      this.savedState = savedState;
    }
  }
}
