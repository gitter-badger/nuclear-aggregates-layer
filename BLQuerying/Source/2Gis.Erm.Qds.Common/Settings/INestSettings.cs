using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Settings;

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