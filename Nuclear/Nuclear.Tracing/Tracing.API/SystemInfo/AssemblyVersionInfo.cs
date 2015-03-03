using System;
using System.Reflection;
using System.Text;

namespace DoubleGis.Erm.Platform.Common.Logging.SystemInfo
{
    /// <summary>
    /// Класс для получение информации по managed сборкам
    /// </summary>
    public static class AssemblyVersionInfo
    {
        private static T GetAssemblyAttribute<T>(ICustomAttributeProvider assembly)
            where T : Attribute
        {
            var attributes = assembly.GetCustomAttributes(typeof(T),true);
            if (attributes.Length > 0)
            {
                return (T)attributes[0];
            }

            return null;
        }

        /// <summary>
        /// Возвращает информацию о версии сборки, вызвавщей свойство 
        /// </summary>
        public static String CallingAssemblyInfo
        {
            get 
            {
                var sb = new StringBuilder();
                try
                {
                    var callingAssembly = Assembly.GetCallingAssembly();
                    sb.AppendLine("AssemblyInfo:");
                    sb.AppendLine("Assembly:" + callingAssembly.FullName);
                    var assemblyTitleAttribute = GetAssemblyAttribute<AssemblyTitleAttribute>(callingAssembly);
                    if (assemblyTitleAttribute != null)
                    {
                        sb.AppendLine("AssemblyTitle:" + assemblyTitleAttribute.Title);
                    }

                    var assemblyVersionAttribute = GetAssemblyAttribute<AssemblyVersionAttribute>(callingAssembly);
                    if (assemblyVersionAttribute != null)
                    {
                        sb.AppendLine("AssemblyVersion:" + assemblyVersionAttribute.Version);
                    }

                    var assemblyFileVersionAttribute = GetAssemblyAttribute<AssemblyFileVersionAttribute>(callingAssembly);
                    if (assemblyFileVersionAttribute != null)
                    {
                        sb.AppendLine("AssemblyFileVersion:" + assemblyFileVersionAttribute.Version);
                    }
                }
                catch (Exception)
                {
                    sb.AppendLine("Can't get calling assembly info");
                }

                return sb.ToString();
            }
        }
    }
}
