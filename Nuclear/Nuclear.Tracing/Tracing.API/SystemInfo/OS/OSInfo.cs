using System;
using System.Runtime.InteropServices;

namespace DoubleGis.Erm.Platform.Common.Logging.SystemInfo.OS
{
	/// <summary>
	/// Класс для получения информации о версии ОС
	/// </summary>
	public static class OSInfo
	{
        /// <summary>
        /// Возвращает строку с полным описанием версии опреационной системы - включая разрядность, поддерживаются ОС от win95 до win7 и win2008R2
        /// </summary>
        /// <value>The os version string.</value>
        public static String OsVersionString
        {
            get 
            {
                var ver = GetOsVersion();
                return ver != null ? ver.VersionString : null;
            }
        }

        /// <summary>
        /// Возвращает описатель версии ОС
        /// </summary>
        /// <returns></returns>
		public static OSVersionDescriptor GetOsVersion()
        {
            var resultVersion = new OSVersionDescriptor();
            var osVersionInfo = new OSVERSIONINFOEX { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX)) };

            try
            {
                if (GetVersionEx(ref osVersionInfo))
                {
                    var majorVersion = TryingConvertTo(osVersionInfo.dwMajorVersion, i=>(MajorVersion)i);
                    if (!majorVersion.HasValue)
                    {
                        return null;
                    }
                    resultVersion.MajorVersion = majorVersion.Value;

                    var minorVersion = TryingConvertTo(osVersionInfo.dwMinorVersion, i=>(MinorVersion)i);
                    if (!minorVersion.HasValue)
                    {
                        return null;
                    }
                    resultVersion.MinorVersion = minorVersion.Value;

                    var buildNumber = TryingConvertTo(osVersionInfo.dwBuildNumber, i=>(OSBuildNumber)i);
                    if (!buildNumber.HasValue)
                    {
                        return null;
                    }
                    resultVersion.BuildNumber = buildNumber.Value;

                    resultVersion.PlatformId = Environment.OSVersion.Platform;

                    resultVersion.CSDVersion = osVersionInfo.szCSDVersion;
                    resultVersion.ServicePackMajor = osVersionInfo.wServicePackMajor;
                    resultVersion.ServicePackMinor = osVersionInfo.wServicePackMinor;
                    
                    var suiteMask = TryingConvertTo(osVersionInfo.wSuiteMask, i=>(OSSuites)i);
                    if (!suiteMask.HasValue)
                    {
                        return null;
                    }
                    resultVersion.SuiteMask = suiteMask.Value;

                    var productType = TryingConvertTo(osVersionInfo.wProductType, i=>(OSProductType)i);
                    if (!productType.HasValue)
                    {
                        return null;
                    }
                    resultVersion.ProductType = productType.Value;

                    if (resultVersion.MajorVersion == MajorVersion.Win7 
                        || resultVersion.MajorVersion == MajorVersion.Vista 
                        || resultVersion.MajorVersion == MajorVersion.Win2008
                        || resultVersion.MajorVersion == MajorVersion.Win2008R2
                        || resultVersion.MajorVersion == MajorVersion.Win8)
                    {
                        int productTypeEx;
                        if (GetProductInfo(osVersionInfo.dwMajorVersion, osVersionInfo.dwMinorVersion,
                                           osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                                           out productTypeEx))
                        {
                            var rezProductTypeEx = TryingConvertTo(productTypeEx, i=>(OSProductTypeEx)i);
                            if (!rezProductTypeEx.HasValue)
                            {
                                return null;
                            }
                            resultVersion.ProductTypeEx = rezProductTypeEx.Value;    
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return resultVersion;
        }

        private static T? TryingConvertTo<T, TUnderlying>(TUnderlying value, Func<TUnderlying, T> converter)
            where T:struct
            where TUnderlying : struct
        {
            try
            {
                return converter(value);
            }
            catch (Exception)
            {
                return null;
            }
        }

	    #region PINVOKE
		#region GET
		#region PRODUCT INFO
		[DllImport( "Kernel32.dll" )]
		internal static extern bool GetProductInfo(
			int osMajorVersion,
			int osMinorVersion,
			int spMajorVersion,
			int spMinorVersion,
			out int edition );
		#endregion PRODUCT INFO

		#region VERSION
		[DllImport( "kernel32.dll" )]
		private static extern bool GetVersionEx( ref OSVERSIONINFOEX osVersionInfo );
		#endregion VERSION
		#endregion GET


		#region OSVERSIONINFOEX
        // ReSharper disable InconsistentNaming
		[StructLayout( LayoutKind.Sequential )]
        private struct OSVERSIONINFOEX
        {
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable MemberCanBePrivate.Local
			public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
			public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 128 )]
			public string szCSDVersion;
			public short wServicePackMajor;
			public short wServicePackMinor;
			public short wSuiteMask;
			public byte wProductType;
			public byte wReserved;
            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local
		}
        // ReSharper restore InconsistentNaming
		#endregion OSVERSIONINFOEX
        #endregion PINVOKE
	}
}


