using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationProvider
    {
        IEnumerable<Type> GetCustomizations(EntityName entityName);
    }
}