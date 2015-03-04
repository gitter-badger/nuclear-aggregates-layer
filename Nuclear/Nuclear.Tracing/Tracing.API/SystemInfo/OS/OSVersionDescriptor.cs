using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nuclear.Tracing.API.SystemInfo.OS
{
    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - номер build ОС
    /// </summary>
    public enum OSBuildNumber
    {
        None = 0,
        Win2000SP4 = 2195,
        WinXPSP2 = 2600,
        Win2003SP1 = 3790,
    }
    // ReSharper restore InconsistentNaming

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - версия выпуска ОС
    /// </summary>
    [Flags]
    public enum OSSuites : ushort
    {
        None = 0,
        SmallBusiness = 0x00000001,
        Enterprise = 0x00000002,
        BackOffice = 0x00000004,
        Communications = 0x00000008,
        Terminal = 0x00000010,
        SmallBusinessRestricted = 0x00000020,
        EmbeddedNT = 0x00000040,
        Datacenter = 0x00000080,
        SingleUserTS = 0x00000100,
        Personal = 0x00000200,
        Blade = 0x00000400,
        EmbeddedRestricted = 0x00000800,
    }
    // ReSharper restore InconsistentNaming

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - тип ОС - серверная, клиентская и т.д.
    /// </summary>
    public enum OSProductType : byte
    {
        Invalid = 0,
        Workstation = 1,
        DomainController = 2,
        Server = 3,
    }
    // ReSharper restore InconsistentNaming

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - старший номер версии ОС
    /// </summary>
    public enum MajorVersion
    {
        Win32s = 0,
        Win95 = 4,
        Win98 = 4,
        WinME = 4,
        WinNT351 = 3,
        WinNT4 = 4,
        WinNT5 = 5,
        Win2000 = WinNT5,
        WinXP = WinNT5,
        Win2003 = WinNT5,
        WinXPx64 = WinNT5,
        Vista = 6,
        Win2008 = 6,
        Win2008R2 = 6,
        Win7 = 6,
        Win8 = 6,
    }
    // ReSharper restore InconsistentNaming

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - младший номер версии ОС
    /// </summary>
    public enum MinorVersion
    {
        
        Win32s = 0,
        Win95 = 0,
        Win98 = 10,
        WinME = 90,
        WinNT351 = 51,
        WinNT4 = 0,
        Win2000 = 0,
        WinXP = 1,
        Win2003 = 2,
        WinXPx64 = 2,
        Vista = 0,
        Win2008 = 0,
        Win2008R2 = 1,
        Win7 = 1,
        Win8 = 2,
    }
    // ReSharper restore InconsistentNaming

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Перечисление - расширенный тип версии ОС - появился начиная с Windows Vista (в WinAPI появилась функция для определения)
    /// </summary>
    public enum OSProductTypeEx
    {
        PRODUCT_UNDEFINED = 0x00000000,
        PRODUCT_ULTIMATE = 0x00000001,
		PRODUCT_HOME_BASIC = 0x00000002,
		PRODUCT_HOME_PREMIUM = 0x00000003,
		PRODUCT_ENTERPRISE = 0x00000004,
		PRODUCT_HOME_BASIC_N = 0x00000005,
		PRODUCT_BUSINESS = 0x00000006,
		PRODUCT_STANDARD_SERVER = 0x00000007,
		PRODUCT_DATACENTER_SERVER = 0x00000008,
		PRODUCT_SMALLBUSINESS_SERVER = 0x00000009,
		PRODUCT_ENTERPRISE_SERVER = 0x0000000A,
		PRODUCT_STARTER = 0x0000000B,
		PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C,
		PRODUCT_STANDARD_SERVER_CORE = 0x0000000D,
		PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E,
		PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F,
		PRODUCT_BUSINESS_N = 0x00000010,
		PRODUCT_WEB_SERVER = 0x00000011,
		PRODUCT_CLUSTER_SERVER = 0x00000012,
		PRODUCT_HOME_SERVER = 0x00000013,
		PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014,
		PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015,
		PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016,
		PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017,
		PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018,
		PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019,
		PRODUCT_HOME_PREMIUM_N = 0x0000001A,
		PRODUCT_ENTERPRISE_N = 0x0000001B,
		PRODUCT_ULTIMATE_N = 0x0000001C,
		PRODUCT_WEB_SERVER_CORE = 0x0000001D,
		PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E,
		PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F,
		PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020,
		PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023,
		PRODUCT_STANDARD_SERVER_V = 0x00000024,
		PRODUCT_ENTERPRISE_SERVER_V = 0x00000026,
		PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028,
		PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029,
		PRODUCT_HYPERV = 0x0000002A,
    }
    // ReSharper restore InconsistentNaming

    /// <summary>
    /// Класс описатель версии ОС - содержит данные о версии,
    /// непосредственно его содавать нельзя - его готовые экземпляры возвращается выспомогательными методами
    /// </summary>
    public class OSVersionDescriptor
    {
        /// <summary>
        /// Старший номер версии
        /// </summary>
        public MajorVersion MajorVersion { get; internal set; }
        /// <summary>
        /// Младший номер версии
        /// </summary>
        public MinorVersion MinorVersion { get; internal set; }
        /// <summary>
        /// Номер build ОС
        /// </summary>
        public OSBuildNumber BuildNumber { get; internal set; }
        /// <summary>
        /// Платформа ОС - Win9x, WinNT и т.д.
        /// </summary>
        public PlatformID PlatformId { get; internal set; }
        /// <summary>
        /// Описание servicepack'ов установленных в ОС
        /// </summary>
        /// <value>The CSD version.</value>
        public string CSDVersion { get; internal set; }
        /// <summary>
        /// Старшая версия servicepack
        /// </summary>
        public short ServicePackMajor { get; internal set; }
        /// <summary>
        /// Младшая версия servicepack
        /// </summary>
        public short ServicePackMinor { get; internal set; }
        /// <summary>
        /// Версия выпуска ОС
        /// </summary>
        public OSSuites SuiteMask { get; internal set; }
        /// <summary>
        /// Тип ОС - серверная, клиентская и т.д.
        /// </summary>
        public OSProductType ProductType { get; internal set; }
        /// <summary>
        /// Расширенный тип версии ОС - появился начиная с Windows Vista (в WinAPI появилась функция для определения)
        /// Для версий ОС до Windows Vista - содержит null
        /// </summary>
        public OSProductTypeEx? ProductTypeEx { get; internal set; }

        internal OSVersionDescriptor()
        {
        }

        static OSVersionDescriptor()
        {
            FillOsNames();
        }

        private const String VersionStringTemplate = "{0} {1}{2} {3} bits";
        /// <summary>
        /// Полное описание версии ОС с редакцией, сервиспаками, разрядностью и т.д.
        /// </summary>
        /// <value>The version string.</value>
        public String VersionString
        {
            get 
            {
                return string.Format(VersionStringTemplate, Name, Edition, (ServicePack != null ? " " + ServicePack : string.Empty), Bits);
            }
        }

        #region BITS
        /// <summary>
        /// Разрядность среды 32-bit или 64-bit для текущего исполняемого приложения, т.е. конкретно разрядность среды, в которой исполняется приложение
        /// Например, в 64-битной ОС, процесс запущенный под Wow32 эмулятором вернет 32 бита
        /// </summary>
        public int AppBits
        {
            get
            {
                return IntPtr.Size * 8;
            }
        }

        /// <summary>
        /// Разрядность ОС 32-bit или 64-bit с проверкой на виртуализацию 32-битного процесса
        /// </summary>
        public int Bits
        {
            get
            {
                int bits = AppBits;
                //int bits = IntPtr.Size * 8;
                if (bits == 32)
                {
                    Exception exception;
                    bool isWow64Process = TryCheckIsWow64Process(out exception);
                    if (isWow64Process)
                    {
                        bits = 64;
                    }
                }
                return bits;
            }
        }
        #endregion BITS

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        private bool TryCheckIsWow64Process(out Exception exception)
        {
            exception = null;
            bool isWow64Process = false;
            try
            {
                IntPtr currentProcess = GetCurrentProcess();
                if (!IsWow64Process(currentProcess, out isWow64Process))
                {
                    exception = new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            return isWow64Process;
        }

        #region EDITION

        private string _edition;
        /// <summary>
        /// Версия выпуска ОС - редакция
        /// </summary>
        public string Edition
        {
            get
            {
                if (_edition != null)
                {
                    return _edition;  //***** RETURN *****//
                }

                string edition = string.Empty;

                #region VERSION 4
                if (MajorVersion == MajorVersion.WinNT4)
                {
                    if (ProductType == OSProductType.Workstation)
                    {
                        // Windows NT 4.0 Workstation
                        edition = "Workstation";
                    }
                    else if (ProductType == OSProductType.Server)
                    {
                        if ((SuiteMask & OSSuites.Enterprise) != 0)
                        {
                            // Windows NT 4.0 Server Enterprise
                            edition = "Enterprise Server";
                        }
                        else
                        {
                            // Windows NT 4.0 Server
                            edition = "Standard Server";
                        }
                    }
                }
                #endregion VERSION 4
                #region VERSION 5
                else if (MajorVersion == MajorVersion.WinNT5
                        || MajorVersion == MajorVersion.Win2000
                        || MajorVersion == MajorVersion.WinXP
                        || MajorVersion == MajorVersion.Win2003
                        || MajorVersion == MajorVersion.WinXPx64)
                {
                    if (ProductType == OSProductType.Workstation)
                    {
                        if ((SuiteMask & OSSuites.Personal) != 0)
                        {
                            // Windows XP Home Edition
                            edition = "Home";
                        }
                        else
                        {
                            // Windows XP / Windows 2000 Professional
                            edition = "Professional";
                        }
                    }
                    else if (ProductType == OSProductType.Server)
                    {
                        if (MinorVersion == MinorVersion.Win2000)
                        {
                            if ((SuiteMask & OSSuites.Datacenter) != 0)
                            {
                                // Windows 2000 Datacenter Server
                                edition = "Datacenter Server";
                            }
                            else if ((SuiteMask & OSSuites.Enterprise) != 0)
                            {
                                // Windows 2000 Advanced Server
                                edition = "Advanced Server";
                            }
                            else
                            {
                                // Windows 2000 Server
                                edition = "Server";
                            }
                        }
                        else
                        {
                            if ((SuiteMask & OSSuites.Datacenter) != 0)
                            {
                                // Windows Server 2003 Datacenter Edition
                                edition = "Datacenter";
                            }
                            else if ((SuiteMask & OSSuites.Enterprise) != 0)
                            {
                                // Windows Server 2003 Enterprise Edition
                                edition = "Enterprise";
                            }
                            else if ((SuiteMask & OSSuites.Blade) != 0)
                            {
                                // Windows Server 2003 Web Edition
                                edition = "Web Edition";
                            }
                            else
                            {
                                // Windows Server 2003 Standard Edition
                                edition = "Standard";
                            }
                        }
                    }
                }
                #endregion VERSION 5
                #region VERSION 6
                else if (MajorVersion == MajorVersion.Vista
                        || MajorVersion == MajorVersion.Win7
                        || MajorVersion == MajorVersion.Win2008
                        || MajorVersion == MajorVersion.Win2008R2
                        || MajorVersion == MajorVersion.Win8)
                {
                    if (ProductTypeEx != null)
                    {
                        switch (ProductTypeEx.Value)
                        {
                            case OSProductTypeEx.PRODUCT_BUSINESS:
                                edition = "Business";
                                break;
                            case OSProductTypeEx.PRODUCT_BUSINESS_N:
                                edition = "Business N";
                                break;
                            case OSProductTypeEx.PRODUCT_CLUSTER_SERVER:
                                edition = "HPC Edition";
                                break;
                            case OSProductTypeEx.PRODUCT_DATACENTER_SERVER:
                                edition = "Datacenter Server";
                                break;
                            case OSProductTypeEx.PRODUCT_DATACENTER_SERVER_CORE:
                                edition = "Datacenter Server (core installation)";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE:
                                edition = "Enterprise";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_N:
                                edition = "Enterprise N";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_SERVER:
                                edition = "Enterprise Server";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_SERVER_CORE:
                                edition = "Enterprise Server (core installation)";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                edition = "Enterprise Server without Hyper-V (core installation)";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_SERVER_IA64:
                                edition = "Enterprise Server for Itanium-based Systems";
                                break;
                            case OSProductTypeEx.PRODUCT_ENTERPRISE_SERVER_V:
                                edition = "Enterprise Server without Hyper-V";
                                break;
                            case OSProductTypeEx.PRODUCT_HOME_BASIC:
                                edition = "Home Basic";
                                break;
                            case OSProductTypeEx.PRODUCT_HOME_BASIC_N:
                                edition = "Home Basic N";
                                break;
                            case OSProductTypeEx.PRODUCT_HOME_PREMIUM:
                                edition = "Home Premium";
                                break;
                            case OSProductTypeEx.PRODUCT_HOME_PREMIUM_N:
                                edition = "Home Premium N";
                                break;
                            case OSProductTypeEx.PRODUCT_HYPERV:
                                edition = "Microsoft Hyper-V Server";
                                break;
                            case OSProductTypeEx.PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                edition = "Windows Essential Business Management Server";
                                break;
                            case OSProductTypeEx.PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                edition = "Windows Essential Business Messaging Server";
                                break;
                            case OSProductTypeEx.PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                edition = "Windows Essential Business Security Server";
                                break;
                            case OSProductTypeEx.PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                edition = "Windows Essential Server Solutions";
                                break;
                            case OSProductTypeEx.PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                edition = "Windows Essential Server Solutions without Hyper-V";
                                break;
                            case OSProductTypeEx.PRODUCT_SMALLBUSINESS_SERVER:
                                edition = "Windows Small Business Server";
                                break;
                            case OSProductTypeEx.PRODUCT_STANDARD_SERVER:
                                edition = "Standard Server";
                                break;
                            case OSProductTypeEx.PRODUCT_STANDARD_SERVER_CORE:
                                edition = "Standard Server (core installation)";
                                break;
                            case OSProductTypeEx.PRODUCT_STANDARD_SERVER_CORE_V:
                                edition = "Standard Server without Hyper-V (core installation)";
                                break;
                            case OSProductTypeEx.PRODUCT_STANDARD_SERVER_V:
                                edition = "Standard Server without Hyper-V";
                                break;
                            case OSProductTypeEx.PRODUCT_STARTER:
                                edition = "Starter";
                                break;
                            case OSProductTypeEx.PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                edition = "Enterprise Storage Server";
                                break;
                            case OSProductTypeEx.PRODUCT_STORAGE_EXPRESS_SERVER:
                                edition = "Express Storage Server";
                                break;
                            case OSProductTypeEx.PRODUCT_STORAGE_STANDARD_SERVER:
                                edition = "Standard Storage Server";
                                break;
                            case OSProductTypeEx.PRODUCT_STORAGE_WORKGROUP_SERVER:
                                edition = "Workgroup Storage Server";
                                break;
                            case OSProductTypeEx.PRODUCT_UNDEFINED:
                                edition = "Unknown product";
                                break;
                            case OSProductTypeEx.PRODUCT_ULTIMATE:
                                edition = "Ultimate";
                                break;
                            case OSProductTypeEx.PRODUCT_ULTIMATE_N:
                                edition = "Ultimate N";
                                break;
                            case OSProductTypeEx.PRODUCT_WEB_SERVER:
                                edition = "Web Server";
                                break;
                            case OSProductTypeEx.PRODUCT_WEB_SERVER_CORE:
                                edition = "Web Server (core installation)";
                                break;
                        }
                    }
                }
                #endregion VERSION 6
                
                _edition = edition;
                return edition;
            }
        }
        #endregion EDITION
        #region NAME

        private static void FillOsNames()
        {
            osNames = new List<OSNameChecker>
                           {
                               new OSNameChecker(
                                   "Windows 95",
                                   v =>
                                   v.PlatformId == PlatformID.Win32Windows && v.MajorVersion == MajorVersion.Win95
                                   && v.MinorVersion == MinorVersion.Win95
                                   && ((string.Compare(v.CSDVersion, "B", StringComparison.OrdinalIgnoreCase) != 0) 
                                    || (string.Compare(v.CSDVersion, "C", StringComparison.OrdinalIgnoreCase) != 0))),
                               new OSNameChecker(
                                   "Windows 95 OSR2",
                                   v =>
                                   v.PlatformId == PlatformID.Win32Windows && v.MajorVersion == MajorVersion.Win95
                                   && v.MinorVersion == MinorVersion.Win95
                                   && ((string.Compare(v.CSDVersion, "B", StringComparison.OrdinalIgnoreCase) == 0) 
                                    || (string.Compare(v.CSDVersion, "C", StringComparison.OrdinalIgnoreCase) == 0))),
                               new OSNameChecker(
                                   "Windows 98",
                                   v =>
                                   v.PlatformId == PlatformID.Win32Windows && v.MajorVersion == MajorVersion.Win98
                                   && v.MinorVersion == MinorVersion.Win98 && (string.Compare(v.CSDVersion, "A", StringComparison.OrdinalIgnoreCase) != 0)),
                               new OSNameChecker(
                                   "Windows 98 Second Edition",
                                   v =>
                                   v.PlatformId == PlatformID.Win32Windows && v.MajorVersion == MajorVersion.Win98
                                   && v.MinorVersion == MinorVersion.Win98 && (string.Compare(v.CSDVersion, "A", StringComparison.OrdinalIgnoreCase) == 0)),
                               new OSNameChecker(
                                   "Windows Me",
                                   v =>
                                   v.PlatformId == PlatformID.Win32Windows && v.MajorVersion == MajorVersion.WinME
                                   && v.MinorVersion == MinorVersion.WinME),
                               new OSNameChecker(
                                   "Windows NT 3.51", v => v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.WinNT351),
                               new OSNameChecker(
                                   "Windows NT 4.0",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.WinNT4
                                   && v.MinorVersion == MinorVersion.WinNT4 && v.ProductType == OSProductType.Workstation),
                               new OSNameChecker(
                                   "Windows NT 4.0 Server",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.WinNT4
                                   && v.MinorVersion == MinorVersion.WinNT4 && v.ProductType == OSProductType.Server),
                               new OSNameChecker(
                                   "Windows 2000",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win2000
                                   && v.MinorVersion == MinorVersion.Win2000),
                               new OSNameChecker(
                                   "Windows XP",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.WinXP
                                   && v.MinorVersion == MinorVersion.WinXP),
                               new OSNameChecker(
                                   "Windows XP 64",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.WinXPx64
                                   && v.MinorVersion == MinorVersion.WinXPx64),
                               new OSNameChecker(
                                   "Windows Server 2003",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win2003
                                   && v.MinorVersion == MinorVersion.Win2003),
                               new OSNameChecker(
                                   "Windows Vista",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Vista
                                   && v.MinorVersion == MinorVersion.Vista && v.ProductType == OSProductType.Workstation),
                               new OSNameChecker(
                                   "Windows 7",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win7
                                   && v.MinorVersion == MinorVersion.Win7 && v.ProductType == OSProductType.Workstation),
                               new OSNameChecker(
                                   "Windows 8",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win8
                                   && v.MinorVersion == MinorVersion.Win8 && v.ProductType == OSProductType.Workstation),
                               new OSNameChecker(
                                   "Windows Server 2008",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win2008
                                   && v.MinorVersion == MinorVersion.Win2008 && v.ProductType == OSProductType.Server),
                               new OSNameChecker(
                                   "Windows Server 2008 R2",
                                   v =>
                                   v.PlatformId == PlatformID.Win32NT && v.MajorVersion == MajorVersion.Win2008R2
                                   && v.MinorVersion == MinorVersion.Win2008R2 && v.ProductType == OSProductType.Server)
                           };
        }

        private static List<OSNameChecker> osNames;

        private class OSNameChecker
        {
            private readonly string _osName;
            private readonly Func<OSVersionDescriptor, bool> _checker;

            public OSNameChecker(String osName, 
                                Func<OSVersionDescriptor, bool> checker)
            {
                _checker = checker;
                _osName = osName;
            }

            public string OSName
            {
                get { return _osName; }
            }

            public Func<OSVersionDescriptor, bool> Checker
            {
                get { return _checker; }
            }
        }

        private string _name;
        /// <summary>
        /// Название ОС - например, Windows XP и т.д.
        /// </summary>
        public string Name
        {
            get
            {
                if (_name != null)
                    return _name;  //***** RETURN *****//

                string name = "unknown";

                var os = osNames.FirstOrDefault(descr=>descr.Checker(this));
                if (os != null)
                {
                    name = os.OSName;
                }

                _name = name;
                return name;
            }
        }
        #endregion NAME


        #region SERVICE PACK
        /// <summary>
        /// Servicepack установленный на ОС
        /// </summary>
        public string ServicePack
        {
            get
            {
                return CSDVersion;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Значение BUILD ОС
        /// </summary>
        public int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region REVISION
        /// <summary>
        /// Ревизия ОС
        /// </summary>
        public int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }
}
