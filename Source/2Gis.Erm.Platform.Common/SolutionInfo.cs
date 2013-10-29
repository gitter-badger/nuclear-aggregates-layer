using System;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Common
{
    // FIXME {all, 03.10.2013}: в процессе разделения Solution единый номер версии теряет актуальность - нужно для каждого под проекта уметь выводить номер версии, т.о.
    // когда куда то выводится номер версии - нужно вносить изменения, выводя версии компонентов (проектов) + иметь возможность ограничить список выводимых компонентов
    public static class SolutionInfo
    {
        public static readonly Version ProductVersion = GetProductVersion();
        public static readonly Version FileVersion = GetFileVersion();

        private static Version GetProductVersion()
        {
            var assemblyAttributes = typeof(SolutionInfo).Assembly.GetCustomAttributes(false);

            foreach (var assemblyAttribute in assemblyAttributes)
            {
                var productVersion = assemblyAttribute as AssemblyInformationalVersionAttribute;
                if (productVersion == null)
                {
                    continue;
                }

                return new Version(productVersion.InformationalVersion);
            }

            return null;
        }

        private static Version GetFileVersion()
        {
            var assemblyAttributes = typeof(SolutionInfo).Assembly.GetCustomAttributes(false);

            foreach (var assemblyAttribute in assemblyAttributes)
            {
                var fileVersion = assemblyAttribute as AssemblyFileVersionAttribute;
                if (fileVersion != null)
                {
                    return new Version(fileVersion.Version);
                }
            }

            return null;
        }
    }
}
