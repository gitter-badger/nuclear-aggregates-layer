using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels
{
    public interface IViewModelTypesRegistry
    {
        IEnumerable<Type> DeclaredViewModelTypes { get; }
    }
}