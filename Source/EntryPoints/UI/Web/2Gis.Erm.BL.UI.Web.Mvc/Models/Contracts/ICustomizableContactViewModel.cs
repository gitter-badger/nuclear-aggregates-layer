using System.Collections.Generic;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface ICustomizableContactViewModel : IEntityViewModelBase
    {
        IDictionary<string, string[]> AvailableSalutations { get; set; }
        string BusinessModelArea { get; set; }
    }
}
