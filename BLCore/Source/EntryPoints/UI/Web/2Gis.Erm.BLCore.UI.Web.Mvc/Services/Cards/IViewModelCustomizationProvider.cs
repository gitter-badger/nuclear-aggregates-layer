using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationProvider
    {
        IEnumerable<Type> GetCustomizations(IEntityType entityName);
    }
}