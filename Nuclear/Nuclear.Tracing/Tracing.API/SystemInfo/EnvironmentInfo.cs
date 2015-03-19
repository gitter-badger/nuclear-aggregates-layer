using System;
using System.Text;

using Nuclear.Tracing.API.SystemInfo.OS;

namespace Nuclear.Tracing.API.SystemInfo
{
    /// <summary>
    /// Класс сбора информации об окружении (ОС, hardware)
    /// </summary>
    public static class EnvironmentInfo
    {
        /// <summary>
        /// Возвращает информацию об окружении (ОС, hardware)
        /// </summary>
        public static String Description
        {
            get 
            {
                var sb = new StringBuilder();
                try
                {
                    var osVersion = OSInfo.OsVersionString;
                    sb.AppendLine("OS: " + (!String.IsNullOrEmpty(osVersion) ? osVersion : Environment.OSVersion.VersionString));
                    sb.AppendLine("CLR: " + Environment.Version);
                    sb.AppendLine("ProcessorsCount: " + Environment.ProcessorCount);
                }
                catch (Exception)
                {
                    sb.AppendLine("Can't Environment Info");
                }

                return sb.ToString();
            }
        }


    }
}
