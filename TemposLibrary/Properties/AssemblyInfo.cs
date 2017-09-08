using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
#if !DEMO
[assembly: AssemblyProduct("Tempos Library")]
[assembly: AssemblyTitle("Tempos Library")]
[assembly: AssemblyDescription("")]
#else
[assembly: AssemblyProduct("Tempos Library (DEMO)")]
[assembly: AssemblyTitle("Tempos Library (DEMO)")]
[assembly: AssemblyDescription("Not for commercial use!")]
#endif
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
[assembly: AssemblyCompany("Tempos Software Company")]
[assembly: AssemblyCopyright("Copyright © Josh Guyette 2010-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("73cd0be1-8a4e-4ded-81a1-6b85220378c0")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.651.0.651.0.651")]
[assembly: AssemblyVersion("1.1.0.651")]
[assembly: AssemblyFileVersion("1.0.0")]
