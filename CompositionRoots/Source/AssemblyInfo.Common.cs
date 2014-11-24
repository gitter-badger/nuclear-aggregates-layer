using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// attributes for all assemblies
[assembly: AssemblyCompany("2GIS")]
[assembly: AssemblyProduct("2GIS ERM")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

#if !SILVERLIGHT

[assembly: SuppressIldasm]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.Satellite)]

#endif

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]

#else

[assembly: AssemblyConfiguration("Release")]

#endif