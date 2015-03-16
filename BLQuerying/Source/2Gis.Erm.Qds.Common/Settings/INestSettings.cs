using System;
using System.Collections.Generic;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Qds.Common.Settings
{
    public interface INestSettings : ISettings
    {
        Protocol Protocol { get; }
        string IndexPrefix { get; }
        int BatchSize { get; }
        string BatchTimeout { get; }
        IEnumerable<Uri> Uris { get; }
    }
}