using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers
{
    public interface IUseCaseHandlersRegistry
    {
        IReadOnlyDictionary<Type, Type[]> Handlers { get; }
    }
}
