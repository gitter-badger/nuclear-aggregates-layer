using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Metadata.Entities;

namespace DoubleGis.Erm.Platform.UI.Metadata.Indicators
{
    public interface IViewModelProperty
    {
        string Name { get; }
        Type PropertyType { get; }

        IEnumerable<IPropertyFeature> Features { get; }

    }
}
