using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Platform.UI.WPF.Shell.Settings
{
    public sealed class ShellSettings : IShellSettings
    {
        private readonly IReadOnlyDictionary<string, CultureInfo> _supportedCulturesMap =
            new Dictionary<string, CultureInfo>
                {
                    { "ru", new CultureInfo("ru-RU") },
                    { "en", new CultureInfo("en-US") }
                };

        public ShellSettings()
        {
            InitializeTagetCulture();
        }

        private void InitializeTagetCulture()
        { 
            var specifiedTargetCulture = ConfigurationManager.AppSettings["TargetCulture"];
            CultureInfo targetCulture;
            if (string.IsNullOrEmpty(specifiedTargetCulture) || !_supportedCulturesMap.TryGetValue(specifiedTargetCulture, out targetCulture))
            {
                throw new InvalidOperationException("Can't resolve target app culture from specified value: " + specifiedTargetCulture);
            }

            TargetCulture = targetCulture;
        }

        public CultureInfo TargetCulture { get; private set; }
    }
}
