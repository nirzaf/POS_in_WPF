// Type: System.Windows.Application
// Assembly: PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF\PresentationFramework.dll

using Microsoft.Win32;
using MS.Internal;
using MS.Internal.AppModel;
using MS.Internal.Interop;
using MS.Internal.IO.Packaging;
using MS.Internal.Utility;
using MS.Utility;
using MS.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Net;
using System.Reflection;
using System.Runtime;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Threading;

namespace System.Windows
{
  public class Application : DispatcherObject, IHaveResources, IQueryAmbient
  {
    [ThreadStatic]
    private static Stack<NestedBamlLoadInfo> s_NestedBamlLoadInfo = (Stack<NestedBamlLoadInfo>) null;
    private static readonly object EVENT_STARTUP = new object();
    private static readonly object EVENT_EXIT = new object();
    private static readonly object EVENT_SESSIONENDING = new object();
    private EventHandler Activated;
    private EventHandler Deactivated;
    private NavigatingCancelEventHandler Navigating;
    private NavigatedEventHandler Navigated;
    private NavigationProgressEventHandler NavigationProgress;
    private NavigationFailedEventHandler NavigationFailed;
    private LoadCompletedEventHandler LoadCompleted;
    private NavigationStoppedEventHandler NavigationStopped;
    private FragmentNavigationEventHandler FragmentNavigation;
    private static object _globalLock;
    private static bool _isShuttingDown;
    private static bool _appCreatedInThisAppDomain;
    private static Application _appInstance;
    private static Assembly _resourceAssembly;
    private Uri _startupUri;
    private Uri _applicationMarkupBaseUri;
    private HybridDictionary _htProps;
    private WindowCollection _appWindowList;
    private WindowCollection _nonAppWindowList;
    private Window _mainWindow;
    private ResourceDictionary _resources;
    private bool _ownDispatcherStarted;
    private NavigationService _navService;
    private SecurityCriticalDataForSet<MimeType> _appMimeType;
    private System.IServiceProvider _serviceProvider;
    private IBrowserCallbackServices _browserCallbackServices;
    private SponsorHelper _browserCallbackSponsor;
    private bool _appIsShutdown;
    private int _exitCode;
    private ShutdownMode _shutdownMode;
    [SecurityCritical]
    private HwndWrapper _parkingHwnd;
    [SecurityCritical]
    private HwndWrapperHook _appFilterHook;
    private EventHandlerList _events;
    private bool _hasImplicitStylesInResources;
    private const MS.Win32.SafeNativeMethods.PlaySoundFlags PLAYSOUND_FLAGS = MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_ASYNC | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_NODEFAULT | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_NOSTOP | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_FILENAME;
    private const string SYSTEM_SOUNDS_REGISTRY_LOCATION = "AppEvents\\Schemes\\Apps\\Explorer\\{0}\\.current\\";
    private const string SYSTEM_SOUNDS_REGISTRY_BASE = "HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\";
    private const string SOUND_NAVIGATING = "Navigating";
    private const string SOUND_COMPLETE_NAVIGATION = "ActivatingDocument";

    public static Application Current
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return Application._appInstance;
      }
    }

    public WindowCollection Windows
    {
      get
      {
        this.VerifyAccess();
        return this.WindowsInternal.Clone();
      }
    }

    public Window MainWindow
    {
      get
      {
        this.VerifyAccess();
        return this._mainWindow;
      }
      set
      {
        this.VerifyAccess();
        if (this._mainWindow is RootBrowserWindow || this.BrowserCallbackServices != null && this._mainWindow == null && !(value is RootBrowserWindow))
          throw new InvalidOperationException(System.Windows.SR.Get("CannotChangeMainWindowInBrowser"));
        if (value == this._mainWindow)
          return;
        this._mainWindow = value;
      }
    }

    public ShutdownMode ShutdownMode
    {
      get
      {
        this.VerifyAccess();
        return this._shutdownMode;
      }
      set
      {
        this.VerifyAccess();
        if (!Application.IsValidShutdownMode(value))
          throw new InvalidEnumArgumentException("value", (int) value, typeof (ShutdownMode));
        if (Application.IsShuttingDown || this._appIsShutdown)
          throw new InvalidOperationException(System.Windows.SR.Get("ShutdownModeWhenAppShutdown"));
        this._shutdownMode = value;
      }
    }

    [Ambient]
    public ResourceDictionary Resources
    {
      get
      {
        bool flag = false;
        ResourceDictionary resourceDictionary;
        lock (Application._globalLock)
        {
          if (this._resources == null)
          {
            this._resources = new ResourceDictionary();
            flag = true;
          }
          resourceDictionary = this._resources;
        }
        if (flag)
          resourceDictionary.AddOwner((DispatcherObject) this);
        return resourceDictionary;
      }
      set
      {
        bool flag = false;
        ResourceDictionary oldDictionary;
        lock (Application._globalLock)
        {
          oldDictionary = this._resources;
          this._resources = value;
        }
        if (oldDictionary != null)
          oldDictionary.RemoveOwner((DispatcherObject) this);
        if (value != null && !value.ContainsOwner((DispatcherObject) this))
          value.AddOwner((DispatcherObject) this);
        if (oldDictionary != value)
          flag = true;
        if (!flag)
          return;
        this.InvalidateResourceReferences(new ResourcesChangeInfo(oldDictionary, value));
      }
    }

    ResourceDictionary IHaveResources.Resources
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this.Resources;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this.Resources = value;
      }
    }

    internal bool HasImplicitStylesInResources
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._hasImplicitStylesInResources;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this._hasImplicitStylesInResources = value;
      }
    }

    public Uri StartupUri
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return this._startupUri;
      }
      set
      {
        this.VerifyAccess();
        if (value == (Uri) null)
          throw new ArgumentNullException("value");
        this._startupUri = value;
      }
    }

    public IDictionary Properties
    {
      get
      {
        lock (Application._globalLock)
        {
          if (this._htProps == null)
            this._htProps = new HybridDictionary(5);
          return (IDictionary) this._htProps;
        }
      }
    }

    public static Assembly ResourceAssembly
    {
      get
      {
        if (Application._resourceAssembly == (Assembly) null)
        {
          lock (Application._globalLock)
            Application._resourceAssembly = Assembly.GetEntryAssembly();
        }
        return Application._resourceAssembly;
      }
      set
      {
        lock (Application._globalLock)
        {
          if (!(Application._resourceAssembly != value))
            return;
          if (Application._resourceAssembly == (Assembly) null && Assembly.GetEntryAssembly() == (Assembly) null)
          {
            Application._resourceAssembly = value;
            BaseUriHelper.ResourceAssembly = value;
          }
          else
            throw new InvalidOperationException(System.Windows.SR.Get("PropertyIsImmutable", (object) "ResourceAssembly", (object) "Application"));
        }
      }
    }

    internal WindowCollection WindowsInternal
    {
      get
      {
        lock (Application._globalLock)
        {
          if (this._appWindowList == null)
            this._appWindowList = new WindowCollection();
          return this._appWindowList;
        }
      }
      private set
      {
        lock (Application._globalLock)
          this._appWindowList = value;
      }
    }

    internal WindowCollection NonAppWindowsInternal
    {
      get
      {
        lock (Application._globalLock)
        {
          if (this._nonAppWindowList == null)
            this._nonAppWindowList = new WindowCollection();
          return this._nonAppWindowList;
        }
      }
      private set
      {
        lock (Application._globalLock)
          this._nonAppWindowList = value;
      }
    }

    internal MimeType MimeType
    {
      get
      {
        return this._appMimeType.Value;
      }
      [SecurityCritical] set
      {
        this._appMimeType = new SecurityCriticalDataForSet<MimeType>(value);
      }
    }

    internal System.IServiceProvider ServiceProvider
    {
      private get
      {
        this.VerifyAccess();
        if (this._serviceProvider != null)
          return this._serviceProvider;
        else
          return (System.IServiceProvider) null;
      }
      set
      {
        this.VerifyAccess();
        this._serviceProvider = value;
        if (value != null)
        {
          this._browserCallbackServices = (IBrowserCallbackServices) this._serviceProvider.GetService(typeof (IBrowserCallbackServices));
          ILease lease = RemotingServices.GetLifetimeService(this._browserCallbackServices as MarshalByRefObject) as ILease;
          if (lease == null)
            return;
          this._browserCallbackSponsor = new SponsorHelper(lease, new TimeSpan(0, 5, 0));
          this._browserCallbackSponsor.Register();
        }
        else
          this.CleanUpBrowserCallBackServices();
      }
    }

    internal IBrowserCallbackServices BrowserCallbackServices
    {
      get
      {
        this.VerifyAccess();
        return this._browserCallbackServices;
      }
    }

    internal NavigationService NavService
    {
      get
      {
        this.VerifyAccess();
        return this._navService;
      }
      set
      {
        this.VerifyAccess();
        this._navService = value;
      }
    }

    internal static bool IsShuttingDown
    {
      [SecurityTreatAsSafe, SecurityCritical] get
      {
        if (Application._isShuttingDown)
          return Application._isShuttingDown;
        if (BrowserInteropHelper.IsBrowserHosted)
        {
          Application current = Application.Current;
          if (current != null && current.CheckAccess())
          {
            IBrowserCallbackServices callbackServices = current.BrowserCallbackServices;
            if (callbackServices != null)
              return callbackServices.IsShuttingDown();
            else
              return false;
          }
        }
        return false;
      }
      set
      {
        lock (Application._globalLock)
          Application._isShuttingDown = value;
      }
    }

    internal static bool IsApplicationObjectShuttingDown
    {
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] get
      {
        return Application._isShuttingDown;
      }
    }

    internal IntPtr ParkingHwnd
    {
      [SecurityCritical] get
      {
        if (this._parkingHwnd != null)
          return this._parkingHwnd.Handle;
        else
          return IntPtr.Zero;
      }
    }

    internal Uri ApplicationMarkupBaseUri
    {
      get
      {
        if (this._applicationMarkupBaseUri == (Uri) null)
          this._applicationMarkupBaseUri = BaseUriHelper.BaseUri;
        return this._applicationMarkupBaseUri;
      }
      [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")] set
      {
        this._applicationMarkupBaseUri = value;
      }
    }

    EventHandlerList Events
    {
      private get
      {
        if (this._events == null)
          this._events = new EventHandlerList();
        return this._events;
      }
    }

    public event StartupEventHandler Startup
    {
      add
      {
        this.VerifyAccess();
        this.Events.AddHandler(Application.EVENT_STARTUP, (Delegate) value);
      }
      remove
      {
        this.VerifyAccess();
        this.Events.RemoveHandler(Application.EVENT_STARTUP, (Delegate) value);
      }
    }

    public event ExitEventHandler Exit
    {
      add
      {
        this.VerifyAccess();
        this.Events.AddHandler(Application.EVENT_EXIT, (Delegate) value);
      }
      remove
      {
        this.VerifyAccess();
        this.Events.RemoveHandler(Application.EVENT_EXIT, (Delegate) value);
      }
    }

    public event EventHandler Activated
    {
      add
      {
        EventHandler eventHandler = this.Activated;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.Activated, comparand + value, comparand);
        }
        while (eventHandler != comparand);
      }
      remove
      {
        EventHandler eventHandler = this.Activated;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.Activated, comparand - value, comparand);
        }
        while (eventHandler != comparand);
      }
    }

    public event EventHandler Deactivated
    {
      add
      {
        EventHandler eventHandler = this.Deactivated;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.Deactivated, comparand + value, comparand);
        }
        while (eventHandler != comparand);
      }
      remove
      {
        EventHandler eventHandler = this.Deactivated;
        EventHandler comparand;
        do
        {
          comparand = eventHandler;
          eventHandler = Interlocked.CompareExchange<EventHandler>(ref this.Deactivated, comparand - value, comparand);
        }
        while (eventHandler != comparand);
      }
    }

    public event SessionEndingCancelEventHandler SessionEnding
    {
      add
      {
        this.VerifyAccess();
        this.Events.AddHandler(Application.EVENT_SESSIONENDING, (Delegate) value);
      }
      remove
      {
        this.VerifyAccess();
        this.Events.RemoveHandler(Application.EVENT_SESSIONENDING, (Delegate) value);
      }
    }

    public event DispatcherUnhandledExceptionEventHandler DispatcherUnhandledException
    {
      add
      {
        this.Dispatcher.Invoke(DispatcherPriority.Send, (Delegate) (unused =>
        {
          this.Dispatcher.UnhandledException += value;
          return (object) null;
        }), (object) null);
      }
      remove
      {
        this.Dispatcher.Invoke(DispatcherPriority.Send, (Delegate) (unused =>
        {
          this.Dispatcher.UnhandledException -= value;
          return (object) null;
        }), (object) null);
      }
    }

    public event NavigatingCancelEventHandler Navigating
    {
      add
      {
        NavigatingCancelEventHandler cancelEventHandler = this.Navigating;
        NavigatingCancelEventHandler comparand;
        do
        {
          comparand = cancelEventHandler;
          cancelEventHandler = Interlocked.CompareExchange<NavigatingCancelEventHandler>(ref this.Navigating, comparand + value, comparand);
        }
        while (cancelEventHandler != comparand);
      }
      remove
      {
        NavigatingCancelEventHandler cancelEventHandler = this.Navigating;
        NavigatingCancelEventHandler comparand;
        do
        {
          comparand = cancelEventHandler;
          cancelEventHandler = Interlocked.CompareExchange<NavigatingCancelEventHandler>(ref this.Navigating, comparand - value, comparand);
        }
        while (cancelEventHandler != comparand);
      }
    }

    public event NavigatedEventHandler Navigated
    {
      add
      {
        NavigatedEventHandler navigatedEventHandler = this.Navigated;
        NavigatedEventHandler comparand;
        do
        {
          comparand = navigatedEventHandler;
          navigatedEventHandler = Interlocked.CompareExchange<NavigatedEventHandler>(ref this.Navigated, comparand + value, comparand);
        }
        while (navigatedEventHandler != comparand);
      }
      remove
      {
        NavigatedEventHandler navigatedEventHandler = this.Navigated;
        NavigatedEventHandler comparand;
        do
        {
          comparand = navigatedEventHandler;
          navigatedEventHandler = Interlocked.CompareExchange<NavigatedEventHandler>(ref this.Navigated, comparand - value, comparand);
        }
        while (navigatedEventHandler != comparand);
      }
    }

    public event NavigationProgressEventHandler NavigationProgress
    {
      add
      {
        NavigationProgressEventHandler progressEventHandler = this.NavigationProgress;
        NavigationProgressEventHandler comparand;
        do
        {
          comparand = progressEventHandler;
          progressEventHandler = Interlocked.CompareExchange<NavigationProgressEventHandler>(ref this.NavigationProgress, comparand + value, comparand);
        }
        while (progressEventHandler != comparand);
      }
      remove
      {
        NavigationProgressEventHandler progressEventHandler = this.NavigationProgress;
        NavigationProgressEventHandler comparand;
        do
        {
          comparand = progressEventHandler;
          progressEventHandler = Interlocked.CompareExchange<NavigationProgressEventHandler>(ref this.NavigationProgress, comparand - value, comparand);
        }
        while (progressEventHandler != comparand);
      }
    }

    public event NavigationFailedEventHandler NavigationFailed
    {
      add
      {
        NavigationFailedEventHandler failedEventHandler = this.NavigationFailed;
        NavigationFailedEventHandler comparand;
        do
        {
          comparand = failedEventHandler;
          failedEventHandler = Interlocked.CompareExchange<NavigationFailedEventHandler>(ref this.NavigationFailed, comparand + value, comparand);
        }
        while (failedEventHandler != comparand);
      }
      remove
      {
        NavigationFailedEventHandler failedEventHandler = this.NavigationFailed;
        NavigationFailedEventHandler comparand;
        do
        {
          comparand = failedEventHandler;
          failedEventHandler = Interlocked.CompareExchange<NavigationFailedEventHandler>(ref this.NavigationFailed, comparand - value, comparand);
        }
        while (failedEventHandler != comparand);
      }
    }

    public event LoadCompletedEventHandler LoadCompleted
    {
      add
      {
        LoadCompletedEventHandler completedEventHandler = this.LoadCompleted;
        LoadCompletedEventHandler comparand;
        do
        {
          comparand = completedEventHandler;
          completedEventHandler = Interlocked.CompareExchange<LoadCompletedEventHandler>(ref this.LoadCompleted, comparand + value, comparand);
        }
        while (completedEventHandler != comparand);
      }
      remove
      {
        LoadCompletedEventHandler completedEventHandler = this.LoadCompleted;
        LoadCompletedEventHandler comparand;
        do
        {
          comparand = completedEventHandler;
          completedEventHandler = Interlocked.CompareExchange<LoadCompletedEventHandler>(ref this.LoadCompleted, comparand - value, comparand);
        }
        while (completedEventHandler != comparand);
      }
    }

    public event NavigationStoppedEventHandler NavigationStopped
    {
      add
      {
        NavigationStoppedEventHandler stoppedEventHandler = this.NavigationStopped;
        NavigationStoppedEventHandler comparand;
        do
        {
          comparand = stoppedEventHandler;
          stoppedEventHandler = Interlocked.CompareExchange<NavigationStoppedEventHandler>(ref this.NavigationStopped, comparand + value, comparand);
        }
        while (stoppedEventHandler != comparand);
      }
      remove
      {
        NavigationStoppedEventHandler stoppedEventHandler = this.NavigationStopped;
        NavigationStoppedEventHandler comparand;
        do
        {
          comparand = stoppedEventHandler;
          stoppedEventHandler = Interlocked.CompareExchange<NavigationStoppedEventHandler>(ref this.NavigationStopped, comparand - value, comparand);
        }
        while (stoppedEventHandler != comparand);
      }
    }

    public event FragmentNavigationEventHandler FragmentNavigation
    {
      add
      {
        FragmentNavigationEventHandler navigationEventHandler = this.FragmentNavigation;
        FragmentNavigationEventHandler comparand;
        do
        {
          comparand = navigationEventHandler;
          navigationEventHandler = Interlocked.CompareExchange<FragmentNavigationEventHandler>(ref this.FragmentNavigation, comparand + value, comparand);
        }
        while (navigationEventHandler != comparand);
      }
      remove
      {
        FragmentNavigationEventHandler navigationEventHandler = this.FragmentNavigation;
        FragmentNavigationEventHandler comparand;
        do
        {
          comparand = navigationEventHandler;
          navigationEventHandler = Interlocked.CompareExchange<FragmentNavigationEventHandler>(ref this.FragmentNavigation, comparand - value, comparand);
        }
        while (navigationEventHandler != comparand);
      }
    }

    static Application()
    {
      Application.ApplicationInit();
    }

    [SecurityCritical]
    public Application()
    {
      EventTrace.EasyTraceEvent(EventTrace.Keyword.KeywordGeneral | EventTrace.Keyword.KeywordPerf, EventTrace.Event.WClientAppCtor);
      lock (Application._globalLock)
      {
        if (Application._appCreatedInThisAppDomain)
          throw new InvalidOperationException(System.Windows.SR.Get("MultiSingleton"));
        Application._appInstance = this;
        Application.IsShuttingDown = false;
        Application._appCreatedInThisAppDomain = true;
      }
      this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) new DispatcherOperationCallback(this.StartDispatcherInBrowser), (object) null);
      this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) (unused =>
      {
        if (Application.IsShuttingDown)
          return (object) null;
        StartupEventArgs local_0 = new StartupEventArgs();
        this.OnStartup(local_0);
        if (local_0.PerformDefaultAction)
          this.DoStartup();
        return (object) null;
      }), (object) null);
    }

    public int Run()
    {
      EventTrace.EasyTraceEvent(EventTrace.Keyword.KeywordGeneral | EventTrace.Keyword.KeywordPerf, EventTrace.Event.WClientAppRun);
      return this.Run((Window) null);
    }

    [SecurityCritical]
    public int Run(Window window)
    {
      this.VerifyAccess();
      if (Application.InBrowserHostedApp())
        throw new InvalidOperationException(System.Windows.SR.Get("CannotCallRunFromBrowserHostedApp"));
      else
        return this.RunInternal(window);
    }

    internal static bool InBrowserHostedApp()
    {
      if (BrowserInteropHelper.IsBrowserHosted)
        return !(Application.Current is XappLauncherApp);
      else
        return false;
    }

    internal object GetService(Type serviceType)
    {
      this.VerifyAccess();
      object obj = (object) null;
      if (this.ServiceProvider != null)
        obj = this.ServiceProvider.GetService(serviceType);
      return obj;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public void Shutdown()
    {
      this.Shutdown(0);
    }

    [SecurityCritical]
    public void Shutdown(int exitCode)
    {
      SecurityHelper.DemandUIWindowPermission();
      this.CriticalShutdown(exitCode);
    }

    [SecurityCritical]
    internal void CriticalShutdown(int exitCode)
    {
      this.VerifyAccess();
      if (Application.IsShuttingDown)
        return;
      this.SetExitCode(exitCode);
      Application.IsShuttingDown = true;
      this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) new DispatcherOperationCallback(this.ShutdownCallback), (object) null);
    }

    public object FindResource(object resourceKey)
    {
      ResourceDictionary resourceDictionary = this._resources;
      object obj = (object) null;
      if (resourceDictionary != null)
        obj = resourceDictionary[resourceKey];
      if (obj == DependencyProperty.UnsetValue || obj == null)
        obj = SystemResources.FindResourceInternal(resourceKey);
      if (obj == null)
        Helper.ResourceFailureThrow(resourceKey);
      return obj;
    }

    public object TryFindResource(object resourceKey)
    {
      ResourceDictionary resourceDictionary = this._resources;
      object obj = (object) null;
      if (resourceDictionary != null)
        obj = resourceDictionary[resourceKey];
      if (obj == DependencyProperty.UnsetValue || obj == null)
        obj = SystemResources.FindResourceInternal(resourceKey);
      return obj;
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal object FindResourceInternal(object resourceKey)
    {
      return this.FindResourceInternal(resourceKey, false, false);
    }

    internal object FindResourceInternal(object resourceKey, bool allowDeferredResourceReference, bool mustReturnDeferredResourceReference)
    {
      ResourceDictionary resourceDictionary = this._resources;
      if (resourceDictionary == null)
      {
        return (object) null;
      }
      else
      {
        bool canCache;
        return resourceDictionary.FetchResource(resourceKey, allowDeferredResourceReference, mustReturnDeferredResourceReference, out canCache);
      }
    }

    [SecurityCritical]
    public static void LoadComponent(object component, Uri resourceLocator)
    {
      if (component == null)
        throw new ArgumentNullException("component");
      if (resourceLocator == (Uri) null)
        throw new ArgumentNullException("resourceLocator");
      if (resourceLocator.OriginalString == null)
      {
        throw new ArgumentException(System.Windows.SR.Get("ArgumentPropertyMustNotBeNull", (object) "resourceLocator", (object) "OriginalString"));
      }
      else
      {
        if (resourceLocator.IsAbsoluteUri)
          throw new ArgumentException(System.Windows.SR.Get("AbsoluteUriNotAllowed"));
        Uri curComponentUri = new Uri(BaseUriHelper.PackAppBaseUri, resourceLocator);
        ParserContext parserContext = new ParserContext();
        parserContext.BaseUri = curComponentUri;
        Stream stream;
        bool closeStream;
        if (Application.IsComponentBeingLoadedFromOuterLoadBaml(curComponentUri))
        {
          NestedBamlLoadInfo nestedBamlLoadInfo = Application.s_NestedBamlLoadInfo.Peek();
          stream = nestedBamlLoadInfo.BamlStream;
          stream.Seek(0L, SeekOrigin.Begin);
          parserContext.SkipJournaledProperties = nestedBamlLoadInfo.SkipJournaledProperties;
          nestedBamlLoadInfo.BamlUri = (Uri) null;
          closeStream = false;
        }
        else
        {
          PackagePart resourceOrContentPart = Application.GetResourceOrContentPart(resourceLocator);
          ContentType contentType = new ContentType(resourceOrContentPart.ContentType);
          stream = resourceOrContentPart.GetStream();
          closeStream = true;
          if (!MimeTypeMapper.BamlMime.AreTypeAndSubTypeEqual(contentType))
            throw new Exception(System.Windows.SR.Get("ContentTypeNotSupported", new object[1]
            {
              (object) contentType
            }));
        }
        IStreamInfo streamInfo = stream as IStreamInfo;
        if (streamInfo == null || streamInfo.Assembly != component.GetType().Assembly)
          throw new Exception(System.Windows.SR.Get("UriNotMatchWithRootType", (object) component.GetType(), (object) resourceLocator));
        else
          XamlReader.LoadBaml(stream, parserContext, component, closeStream);
      }
    }

    public static object LoadComponent(Uri resourceLocator)
    {
      if (resourceLocator == (Uri) null)
        throw new ArgumentNullException("resourceLocator");
      if (resourceLocator.OriginalString == null)
        throw new ArgumentException(System.Windows.SR.Get("ArgumentPropertyMustNotBeNull", (object) "resourceLocator", (object) "OriginalString"));
      else if (resourceLocator.IsAbsoluteUri)
        throw new ArgumentException(System.Windows.SR.Get("AbsoluteUriNotAllowed"));
      else
        return Application.LoadComponent(resourceLocator, false);
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    internal static object LoadComponent(Uri resourceLocator, bool bSkipJournaledProperties)
    {
      Uri resolvedUri = BindUriHelper.GetResolvedUri(BaseUriHelper.PackAppBaseUri, resourceLocator);
      PackagePart resourceOrContentPart = Application.GetResourceOrContentPart(resolvedUri);
      ContentType contentType = new ContentType(resourceOrContentPart.ContentType);
      Stream stream = resourceOrContentPart.GetStream();
      ParserContext parserContext = new ParserContext();
      parserContext.BaseUri = resolvedUri;
      parserContext.SkipJournaledProperties = bSkipJournaledProperties;
      if (MimeTypeMapper.BamlMime.AreTypeAndSubTypeEqual(contentType))
        return Application.LoadBamlStreamWithSyncInfo(stream, parserContext);
      if (MimeTypeMapper.XamlMime.AreTypeAndSubTypeEqual(contentType))
        return XamlReader.Load(stream, parserContext);
      throw new Exception(System.Windows.SR.Get("ContentTypeNotSupported", new object[1]
      {
        (object) contentType.ToString()
      }));
    }

    internal static object LoadBamlStreamWithSyncInfo(Stream stream, ParserContext pc)
    {
      if (Application.s_NestedBamlLoadInfo == null)
        Application.s_NestedBamlLoadInfo = new Stack<NestedBamlLoadInfo>();
      NestedBamlLoadInfo nestedBamlLoadInfo = new NestedBamlLoadInfo(pc.BaseUri, stream, pc.SkipJournaledProperties);
      Application.s_NestedBamlLoadInfo.Push(nestedBamlLoadInfo);
      try
      {
        return XamlReader.LoadBaml(stream, pc, (object) null, true);
      }
      finally
      {
        Application.s_NestedBamlLoadInfo.Pop();
        if (Application.s_NestedBamlLoadInfo.Count == 0)
          Application.s_NestedBamlLoadInfo = (Stack<NestedBamlLoadInfo>) null;
      }
    }

    [SecurityCritical]
    public static StreamResourceInfo GetResourceStream(Uri uriResource)
    {
      if (uriResource == (Uri) null)
        throw new ArgumentNullException("uriResource");
      if (uriResource.OriginalString == null)
      {
        throw new ArgumentException(System.Windows.SR.Get("ArgumentPropertyMustNotBeNull", (object) "uriResource", (object) "OriginalString"));
      }
      else
      {
        if (uriResource.IsAbsoluteUri && !BaseUriHelper.IsPackApplicationUri(uriResource))
          throw new ArgumentException(System.Windows.SR.Get("NonPackAppAbsoluteUriNotAllowed"));
        ResourcePart resourcePart = Application.GetResourceOrContentPart(uriResource) as ResourcePart;
        if (resourcePart != null)
          return new StreamResourceInfo(resourcePart.GetStream(), resourcePart.ContentType);
        else
          return (StreamResourceInfo) null;
      }
    }

    [SecurityCritical]
    public static StreamResourceInfo GetContentStream(Uri uriContent)
    {
      if (uriContent == (Uri) null)
        throw new ArgumentNullException("uriContent");
      if (uriContent.OriginalString == null)
      {
        throw new ArgumentException(System.Windows.SR.Get("ArgumentPropertyMustNotBeNull", (object) "uriContent", (object) "OriginalString"));
      }
      else
      {
        if (uriContent.IsAbsoluteUri && !BaseUriHelper.IsPackApplicationUri(uriContent))
          throw new ArgumentException(System.Windows.SR.Get("NonPackAppAbsoluteUriNotAllowed"));
        ContentFilePart contentFilePart = Application.GetResourceOrContentPart(uriContent) as ContentFilePart;
        if (contentFilePart != null)
          return new StreamResourceInfo(contentFilePart.GetStream(), contentFilePart.ContentType);
        else
          return (StreamResourceInfo) null;
      }
    }

    [SecurityCritical]
    public static StreamResourceInfo GetRemoteStream(Uri uriRemote)
    {
      if (uriRemote == (Uri) null)
        throw new ArgumentNullException("uriRemote");
      if (uriRemote.OriginalString == null)
      {
        throw new ArgumentException(System.Windows.SR.Get("ArgumentPropertyMustNotBeNull", (object) "uriRemote", (object) "OriginalString"));
      }
      else
      {
        if (uriRemote.IsAbsoluteUri && !BaseUriHelper.SiteOfOriginBaseUri.IsBaseOf(uriRemote))
          throw new ArgumentException(System.Windows.SR.Get("NonPackSooAbsoluteUriNotAllowed"));
        Uri packageUri;
        Uri partUri;
        PackUriHelper.ValidateAndGetPackUriComponents(BindUriHelper.GetResolvedUri(BaseUriHelper.SiteOfOriginBaseUri, uriRemote), out packageUri, out partUri);
        SiteOfOriginPart siteOfOriginPart = Application.GetResourcePackage(packageUri).GetPart(partUri) as SiteOfOriginPart;
        Stream stream = (Stream) null;
        if (siteOfOriginPart != null)
        {
          try
          {
            stream = siteOfOriginPart.GetStream();
            if (stream == null)
              siteOfOriginPart = (SiteOfOriginPart) null;
          }
          catch (WebException ex)
          {
            siteOfOriginPart = (SiteOfOriginPart) null;
          }
        }
        if (stream != null)
          return new StreamResourceInfo(stream, siteOfOriginPart.ContentType);
        else
          return (StreamResourceInfo) null;
      }
    }

    public static string GetCookie(Uri uri)
    {
      return CookieHandler.GetCookie(uri, true);
    }

    public static void SetCookie(Uri uri, string value)
    {
      CookieHandler.SetCookie(uri, value);
    }

    bool IQueryAmbient.IsAmbientPropertyAvailable(string propertyName)
    {
      if (propertyName == "Resources")
        return this._resources != null;
      else
        return false;
    }

    protected virtual void OnStartup(StartupEventArgs e)
    {
      this.VerifyAccess();
      StartupEventHandler startupEventHandler = (StartupEventHandler) this.Events[Application.EVENT_STARTUP];
      if (startupEventHandler == null)
        return;
      startupEventHandler((object) this, e);
    }

    protected virtual void OnExit(ExitEventArgs e)
    {
      this.VerifyAccess();
      ExitEventHandler exitEventHandler = (ExitEventHandler) this.Events[Application.EVENT_EXIT];
      if (exitEventHandler == null)
        return;
      exitEventHandler((object) this, e);
    }

    protected virtual void OnActivated(EventArgs e)
    {
      this.VerifyAccess();
      if (this.Activated == null)
        return;
      this.Activated((object) this, e);
    }

    protected virtual void OnDeactivated(EventArgs e)
    {
      this.VerifyAccess();
      if (this.Deactivated == null)
        return;
      this.Deactivated((object) this, e);
    }

    protected virtual void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
      this.VerifyAccess();
      SessionEndingCancelEventHandler cancelEventHandler = (SessionEndingCancelEventHandler) this.Events[Application.EVENT_SESSIONENDING];
      if (cancelEventHandler == null)
        return;
      cancelEventHandler((object) this, e);
    }

    protected virtual void OnNavigating(NavigatingCancelEventArgs e)
    {
      this.VerifyAccess();
      if (this.Navigating == null)
        return;
      this.Navigating((object) this, e);
    }

    protected virtual void OnNavigated(NavigationEventArgs e)
    {
      this.VerifyAccess();
      if (this.Navigated == null)
        return;
      this.Navigated((object) this, e);
    }

    protected virtual void OnNavigationProgress(NavigationProgressEventArgs e)
    {
      this.VerifyAccess();
      if (this.NavigationProgress == null)
        return;
      this.NavigationProgress((object) this, e);
    }

    protected virtual void OnNavigationFailed(NavigationFailedEventArgs e)
    {
      this.VerifyAccess();
      if (this.NavigationFailed == null)
        return;
      this.NavigationFailed((object) this, e);
    }

    protected virtual void OnLoadCompleted(NavigationEventArgs e)
    {
      this.VerifyAccess();
      if (this.LoadCompleted == null)
        return;
      this.LoadCompleted((object) this, e);
    }

    protected virtual void OnNavigationStopped(NavigationEventArgs e)
    {
      this.VerifyAccess();
      if (this.NavigationStopped == null)
        return;
      this.NavigationStopped((object) this, e);
    }

    protected virtual void OnFragmentNavigation(FragmentNavigationEventArgs e)
    {
      this.VerifyAccess();
      if (this.FragmentNavigation == null)
        return;
      this.FragmentNavigation((object) this, e);
    }

    internal virtual void PerformNavigationStateChangeTasks(bool isNavigationInitiator, bool playNavigatingSound, Application.NavigationStateChange state)
    {
      if (!isNavigationInitiator)
        return;
      switch (state)
      {
        case Application.NavigationStateChange.Navigating:
          this.ChangeBrowserDownloadState(true);
          if (!playNavigatingSound)
            break;
          this.PlaySound("Navigating");
          break;
        case Application.NavigationStateChange.Completed:
          this.PlaySound("ActivatingDocument");
          this.ChangeBrowserDownloadState(false);
          this.UpdateBrowserCommands();
          break;
        case Application.NavigationStateChange.Stopped:
          this.ChangeBrowserDownloadState(false);
          break;
      }
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    internal void UpdateBrowserCommands()
    {
      EventTrace.EasyTraceEvent(EventTrace.Keyword.KeywordPerf | EventTrace.Keyword.KeywordHosting, EventTrace.Level.Verbose, EventTrace.Event.WpfHost_UpdateBrowserCommandsStart);
      IBrowserCallbackServices callbackServices = (IBrowserCallbackServices) this.GetService(typeof (IBrowserCallbackServices));
      if (callbackServices != null)
        callbackServices.UpdateCommands();
      EventTrace.EasyTraceEvent(EventTrace.Keyword.KeywordPerf | EventTrace.Keyword.KeywordHosting, EventTrace.Level.Verbose, EventTrace.Event.WpfHost_UpdateBrowserCommandsEnd);
    }

    internal void DoStartup()
    {
      if (!(this.StartupUri != (Uri) null))
        return;
      if (!this.StartupUri.IsAbsoluteUri)
        this.StartupUri = new Uri(this.ApplicationMarkupBaseUri, this.StartupUri);
      if (BaseUriHelper.IsPackApplicationUri(this.StartupUri))
      {
        NavigatingCancelEventArgs e = new NavigatingCancelEventArgs(BindUriHelper.GetUriRelativeToPackAppBase(this.StartupUri), (object) null, (CustomContentState) null, (object) null, NavigationMode.New, (WebRequest) null, (object) null, true);
        this.FireNavigating(e, true);
        if (e.Cancel)
          return;
        this.ConfigAppWindowAndRootElement(Application.LoadComponent(this.StartupUri, false), this.StartupUri);
      }
      else
      {
        this.NavService = new NavigationService((INavigator) null);
        this.NavService.AllowWindowNavigation = true;
        this.NavService.PreBPReady += new BPReadyEventHandler(this.OnPreBPReady);
        this.NavService.Navigate(this.StartupUri);
      }
    }

    [SecurityCritical]
    internal virtual void DoShutdown()
    {
      while (this.WindowsInternal.Count > 0)
      {
        if (!this.WindowsInternal[0].IsDisposed)
          this.WindowsInternal[0].InternalClose(true, true);
        else
          this.WindowsInternal.RemoveAt(0);
      }
      this.WindowsInternal = (WindowCollection) null;
      ExitEventArgs e = new ExitEventArgs(this._exitCode);
      try
      {
        this.OnExit(e);
      }
      finally
      {
        this.SetExitCode(e._exitCode);
        lock (Application._globalLock)
          Application._appInstance = (Application) null;
        this._mainWindow = (Window) null;
        this._htProps = (HybridDictionary) null;
        this.NonAppWindowsInternal = (WindowCollection) null;
        if (this._parkingHwnd != null)
          this._parkingHwnd.Dispose();
        if (this._events != null)
          this._events.Dispose();
        PreloadedPackages.Clear();
        AppSecurityManager.ClearSecurityManager();
        this._appIsShutdown = true;
      }
    }

    [SecurityCritical]
    internal int RunInternal(Window window)
    {
      this.VerifyAccess();
      EventTrace.EasyTraceEvent(EventTrace.Keyword.KeywordGeneral | EventTrace.Keyword.KeywordPerf, EventTrace.Event.WClientAppRun);
      if (this._appIsShutdown)
      {
        throw new InvalidOperationException(System.Windows.SR.Get("CannotCallRunMultipleTimes", new object[1]
        {
          (object) this.GetType().FullName
        }));
      }
      else
      {
        if (window != null)
        {
          if (!window.CheckAccess())
          {
            throw new ArgumentException(System.Windows.SR.Get("WindowPassedShouldBeOnApplicationThread", (object) window.GetType().FullName, (object) this.GetType().FullName));
          }
          else
          {
            if (!this.WindowsInternal.HasItem(window))
              this.WindowsInternal.Add(window);
            if (this.MainWindow == null)
              this.MainWindow = window;
            if (window.Visibility != Visibility.Visible)
              this.Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) (obj =>
              {
                (obj as Window).Show();
                return (object) null;
              }), (object) window);
          }
        }
        this.EnsureHwndSource();
        if (!BrowserInteropHelper.IsBrowserHosted)
          this.RunDispatcher((object) null);
        return this._exitCode;
      }
    }

    internal void InvalidateResourceReferences(ResourcesChangeInfo info)
    {
      this.InvalidateResourceReferenceOnWindowCollection(this.WindowsInternal.Clone(), info);
      this.InvalidateResourceReferenceOnWindowCollection(this.NonAppWindowsInternal.Clone(), info);
    }

    [SecurityCritical]
    internal NavigationWindow GetAppWindow()
    {
      NavigationWindow navigationWindow;
      if ((IBrowserCallbackServices) this.GetService(typeof (IBrowserCallbackServices)) == null)
      {
        navigationWindow = new NavigationWindow();
        new WindowInteropHelper((Window) navigationWindow).EnsureHandle();
      }
      else
        navigationWindow = (NavigationWindow) ((IHostService) this.GetService(typeof (IHostService))).RootBrowserWindowProxy.RootBrowserWindow;
      return navigationWindow;
    }

    internal void FireNavigating(NavigatingCancelEventArgs e, bool isInitialNavigation)
    {
      this.PerformNavigationStateChangeTasks(e.IsNavigationInitiator, !isInitialNavigation, Application.NavigationStateChange.Navigating);
      this.OnNavigating(e);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal void FireNavigated(NavigationEventArgs e)
    {
      this.OnNavigated(e);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal void FireNavigationProgress(NavigationProgressEventArgs e)
    {
      this.OnNavigationProgress(e);
    }

    internal void FireNavigationFailed(NavigationFailedEventArgs e)
    {
      this.PerformNavigationStateChangeTasks(true, false, Application.NavigationStateChange.Stopped);
      this.OnNavigationFailed(e);
    }

    internal void FireLoadCompleted(NavigationEventArgs e)
    {
      this.PerformNavigationStateChangeTasks(e.IsNavigationInitiator, false, Application.NavigationStateChange.Completed);
      this.OnLoadCompleted(e);
    }

    internal void FireNavigationStopped(NavigationEventArgs e)
    {
      this.PerformNavigationStateChangeTasks(e.IsNavigationInitiator, false, Application.NavigationStateChange.Stopped);
      this.OnNavigationStopped(e);
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    internal void FireFragmentNavigation(FragmentNavigationEventArgs e)
    {
      this.OnFragmentNavigation(e);
    }

    private void CleanUpBrowserCallBackServices()
    {
      if (this._browserCallbackServices == null)
        return;
      if (this._browserCallbackSponsor != null)
      {
        this._browserCallbackSponsor.Unregister();
        this._browserCallbackSponsor = (SponsorHelper) null;
      }
      this._browserCallbackServices = (IBrowserCallbackServices) null;
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    private static void ApplicationInit()
    {
      Application._globalLock = new object();
      PreloadedPackages.AddPackage(PackUriHelper.GetPackageUri(BaseUriHelper.PackAppBaseUri), (Package) new ResourceContainer(), true);
      MimeObjectFactory.Register(MimeTypeMapper.BamlMime, new StreamToObjectFactoryDelegate(AppModelKnownContentFactory.BamlConverter));
      StreamToObjectFactoryDelegate method1 = new StreamToObjectFactoryDelegate(AppModelKnownContentFactory.XamlConverter);
      MimeObjectFactory.Register(MimeTypeMapper.XamlMime, method1);
      MimeObjectFactory.Register(MimeTypeMapper.FixedDocumentMime, method1);
      MimeObjectFactory.Register(MimeTypeMapper.FixedDocumentSequenceMime, method1);
      MimeObjectFactory.Register(MimeTypeMapper.FixedPageMime, method1);
      MimeObjectFactory.Register(MimeTypeMapper.ResourceDictionaryMime, method1);
      StreamToObjectFactoryDelegate method2 = new StreamToObjectFactoryDelegate(AppModelKnownContentFactory.HtmlXappConverter);
      MimeObjectFactory.Register(MimeTypeMapper.HtmMime, method2);
      MimeObjectFactory.Register(MimeTypeMapper.HtmlMime, method2);
      MimeObjectFactory.Register(MimeTypeMapper.XbapMime, method2);
    }

    [SecurityCritical]
    private static PackagePart GetResourceOrContentPart(Uri uri)
    {
      Uri packageUri;
      Uri partUri;
      PackUriHelper.ValidateAndGetPackUriComponents(BindUriHelper.GetResolvedUri(BaseUriHelper.PackAppBaseUri, uri), out packageUri, out partUri);
      return Application.GetResourcePackage(packageUri).GetPart(partUri);
    }

    [SecurityCritical]
    private static Package GetResourcePackage(Uri packageUri)
    {
      Package package = PreloadedPackages.GetPackage(packageUri);
      if (package != null)
        return package;
      Uri uri = PackUriHelper.Create(packageUri);
      Invariant.Assert(uri == BaseUriHelper.PackAppBaseUri || uri == BaseUriHelper.SiteOfOriginBaseUri, "Unknown packageUri passed: " + (object) packageUri);
      Invariant.Assert(Application.IsApplicationObjectShuttingDown);
      throw new InvalidOperationException(System.Windows.SR.Get("ApplicationShuttingDown"));
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    private void EnsureHwndSource()
    {
      if (this.BrowserCallbackServices != null || this._parkingHwnd != null)
        return;
      this._appFilterHook = new HwndWrapperHook(this.AppFilterMessage);
      HwndWrapperHook[] hooks = new HwndWrapperHook[1]
      {
        this._appFilterHook
      };
      this._parkingHwnd = new HwndWrapper(0, 0, 0, 0, 0, 0, 0, "", IntPtr.Zero, hooks);
    }

    private IntPtr AppFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      IntPtr refInt = IntPtr.Zero;
      switch ((WindowMessage) msg)
      {
        case WindowMessage.WM_QUERYENDSESSION:
          handled = this.WmQueryEndSession(lParam, ref refInt);
          break;
        case WindowMessage.WM_ACTIVATEAPP:
          handled = this.WmActivateApp(MS.Win32.NativeMethods.IntPtrToInt32(wParam));
          break;
        default:
          handled = false;
          break;
      }
      return refInt;
    }

    private bool WmActivateApp(int wParam)
    {
      if (wParam != 0)
        this.OnActivated(EventArgs.Empty);
      else
        this.OnDeactivated(EventArgs.Empty);
      return false;
    }

    [SecuritySafeCritical]
    private bool WmQueryEndSession(IntPtr lParam, ref IntPtr refInt)
    {
      SessionEndingCancelEventArgs e = new SessionEndingCancelEventArgs((MS.Win32.NativeMethods.IntPtrToInt32(lParam) & int.MinValue) != 0 ? ReasonSessionEnding.Logoff : ReasonSessionEnding.Shutdown);
      this.OnSessionEnding(e);
      bool flag;
      if (!e.Cancel)
      {
        this.Shutdown();
        refInt = new IntPtr(1);
        flag = false;
      }
      else
      {
        SecurityHelper.DemandUnmanagedCode();
        refInt = IntPtr.Zero;
        flag = true;
      }
      return flag;
    }

    private void InvalidateResourceReferenceOnWindowCollection(WindowCollection wc, ResourcesChangeInfo info)
    {
      bool hasImplicitStyles = info.IsResourceAddOperation && this.HasImplicitStylesInResources;
      for (int index = 0; index < wc.Count; ++index)
      {
        if (wc[index].CheckAccess())
        {
          if (hasImplicitStyles)
            wc[index].ShouldLookupImplicitStyles = true;
          TreeWalkHelper.InvalidateOnResourcesChange((FrameworkElement) wc[index], (FrameworkContentElement) null, info);
        }
        else
          wc[index].Dispatcher.BeginInvoke(DispatcherPriority.Send, (Delegate) (obj =>
          {
            object[] local_0 = obj as object[];
            if (hasImplicitStyles)
              ((FrameworkElement) local_0[0]).ShouldLookupImplicitStyles = true;
            TreeWalkHelper.InvalidateOnResourcesChange((FrameworkElement) local_0[0], (FrameworkContentElement) null, (ResourcesChangeInfo) local_0[1]);
            return (object) null;
          }), (object) new object[2]
          {
            (object) wc[index],
            (object) info
          });
      }
    }

    private void SetExitCode(int exitCode)
    {
      if (this._exitCode == exitCode)
        return;
      this._exitCode = exitCode;
      Environment.ExitCode = exitCode;
    }

    [SecurityCritical]
    private object ShutdownCallback(object arg)
    {
      this.ShutdownImpl();
      return (object) null;
    }

    [SecurityCritical]
    private void ShutdownImpl()
    {
      try
      {
        this.DoShutdown();
      }
      finally
      {
        if (this._ownDispatcherStarted)
          this.Dispatcher.CriticalInvokeShutdown();
        this.ServiceProvider = (System.IServiceProvider) null;
      }
    }

    private static bool IsValidShutdownMode(ShutdownMode value)
    {
      if (value != ShutdownMode.OnExplicitShutdown && value != ShutdownMode.OnLastWindowClose)
        return value == ShutdownMode.OnMainWindowClose;
      else
        return true;
    }

    private void OnPreBPReady(object sender, BPReadyEventArgs e)
    {
      this.NavService.PreBPReady -= new BPReadyEventHandler(this.OnPreBPReady);
      this.NavService.AllowWindowNavigation = false;
      this.ConfigAppWindowAndRootElement(e.Content, e.Uri);
      this.NavService = (NavigationService) null;
      e.Cancel = true;
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    private void ConfigAppWindowAndRootElement(object root, Uri uri)
    {
      Window window = root as Window;
      if (window == null)
      {
        NavigationWindow appWindow = this.GetAppWindow();
        appWindow.Navigate(root, (object) new NavigateInfo(uri));
        this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Delegate) (window =>
        {
          if (((Window) window).IsDisposed)
            return;
          ((Window) window).Show();
        }), (object) appWindow);
      }
      else
      {
        if (window.IsVisibilitySet || window.IsDisposed)
          return;
        window.Visibility = Visibility.Visible;
      }
    }

    [SecurityTreatAsSafe]
    [SecurityCritical]
    private void ChangeBrowserDownloadState(bool newState)
    {
      IBrowserCallbackServices callbackServices = (IBrowserCallbackServices) this.GetService(typeof (IBrowserCallbackServices));
      if (callbackServices == null)
        return;
      callbackServices.ChangeDownloadState(newState);
    }

    [SecurityCritical]
    [SecurityTreatAsSafe]
    private void PlaySound(string soundName)
    {
      string systemSound = this.GetSystemSound(soundName);
      if (string.IsNullOrEmpty(systemSound))
        return;
      MS.Win32.UnsafeNativeMethods.PlaySound(systemSound, IntPtr.Zero, MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_ASYNC | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_NODEFAULT | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_NOSTOP | MS.Win32.SafeNativeMethods.PlaySoundFlags.SND_FILENAME);
    }

    [SecurityCritical]
    private string GetSystemSound(string soundName)
    {
      string str = (string) null;
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AppEvents\\Schemes\\Apps\\Explorer\\{0}\\.current\\", new object[1]
      {
        (object) soundName
      });
      PermissionSet permissionSet = new PermissionSet((PermissionSet) null);
      permissionSet.AddPermission((IPermission) new RegistryPermission(RegistryPermissionAccess.Read, "HKEY_CURRENT_USER\\AppEvents\\Schemes\\Apps\\Explorer\\"));
      permissionSet.AddPermission((IPermission) new EnvironmentPermission(PermissionState.Unrestricted));
      permissionSet.Assert();
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name))
        {
          if (registryKey != null)
            str = (string) registryKey.GetValue("");
        }
      }
      catch (IndexOutOfRangeException ex)
      {
      }
      finally
      {
        CodeAccessPermission.RevertAssert();
      }
      return str;
    }

    private static bool IsComponentBeingLoadedFromOuterLoadBaml(Uri curComponentUri)
    {
      bool flag = false;
      Invariant.Assert(curComponentUri != (Uri) null, "curComponentUri should not be null");
      if (Application.s_NestedBamlLoadInfo != null && Application.s_NestedBamlLoadInfo.Count > 0)
      {
        NestedBamlLoadInfo nestedBamlLoadInfo = Application.s_NestedBamlLoadInfo.Peek();
        if (nestedBamlLoadInfo != null && nestedBamlLoadInfo.BamlUri != (Uri) null && (nestedBamlLoadInfo.BamlStream != null && BindUriHelper.DoSchemeAndHostMatch(nestedBamlLoadInfo.BamlUri, curComponentUri)))
        {
          string localPath1 = nestedBamlLoadInfo.BamlUri.LocalPath;
          string localPath2 = curComponentUri.LocalPath;
          Invariant.Assert(localPath1 != null, "fileInBamlConvert should not be null");
          Invariant.Assert(localPath2 != null, "fileCurrent should not be null");
          if (string.Compare(localPath1, localPath2, StringComparison.OrdinalIgnoreCase) == 0)
          {
            flag = true;
          }
          else
          {
            string[] strArray1 = localPath1.Split(new char[2]
            {
              '/',
              '\\'
            });
            string[] strArray2 = localPath2.Split(new char[2]
            {
              '/',
              '\\'
            });
            int length1 = strArray1.Length;
            int length2 = strArray2.Length;
            Invariant.Assert(length1 >= 2 && length2 >= 2);
            int num = length1 - length2;
            if (Math.Abs(num) == 1 && string.Compare(strArray1[length1 - 1], strArray2[length2 - 1], StringComparison.OrdinalIgnoreCase) == 0)
              flag = BaseUriHelper.IsComponentEntryAssembly(num == 1 ? strArray1[1] : strArray2[1]);
          }
        }
      }
      return flag;
    }

    [SecurityCritical]
    [DebuggerNonUserCode]
    private object StartDispatcherInBrowser(object unused)
    {
      if (BrowserInteropHelper.IsBrowserHosted)
      {
        BrowserInteropHelper.InitializeHostFilterInput();
        try
        {
          this.RunDispatcher((object) null);
        }
        catch
        {
          throw;
        }
      }
      return (object) null;
    }

    [SecurityCritical]
    private object RunDispatcher(object ignore)
    {
      if (this._ownDispatcherStarted)
        throw new InvalidOperationException(System.Windows.SR.Get("ApplicationAlreadyRunning"));
      this._ownDispatcherStarted = true;
      Dispatcher.Run();
      return (object) null;
    }

    internal enum NavigationStateChange : byte
    {
      Navigating,
      Completed,
      Stopped,
    }
  }
}
