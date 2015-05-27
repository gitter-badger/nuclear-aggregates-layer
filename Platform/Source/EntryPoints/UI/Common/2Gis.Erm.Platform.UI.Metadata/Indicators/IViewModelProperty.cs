using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    public interface IViewModelProperty
    {
        string Name { get; }
        Type PropertyType { get; }

        IEnumerable<IPropertyFeature> Features { get; }

    }
}
