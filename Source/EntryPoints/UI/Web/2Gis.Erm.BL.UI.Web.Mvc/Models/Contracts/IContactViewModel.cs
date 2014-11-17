using System.Collections.Generic;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Contracts
{
    public interface IContactViewModel
    {
        IDictionary<string, string[]> AvailableSalutations { get; set; }
        string BusinessModelArea { get; set; }
    }
}
