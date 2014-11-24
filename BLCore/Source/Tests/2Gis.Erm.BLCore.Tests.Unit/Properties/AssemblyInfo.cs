using System;
using System.Reflection;
using System.Runtime.InteropServices;

// attributes for all assemblies
[assembly: AssemblyCompany("2GIS")]
[assembly: AssemblyProduct("2GIS ERM")]
[assembly: AssemblyTitle("2GIS ERM")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]

#else

[assembly: AssemblyConfiguration("Release")]

#endif