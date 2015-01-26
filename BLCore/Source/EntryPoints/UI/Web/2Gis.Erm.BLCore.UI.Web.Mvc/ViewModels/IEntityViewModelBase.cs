using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public partial interface IEntityViewModelBase : IViewModel
    {
        long Id { get; set; }
        LookupField CreatedBy { get; set; }
        LookupField ModifiedBy { get; set; }
        LookupField Owner { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime? ModifiedOn { get; set; }
        bool IsNew { get; }
        bool IsActive { get; }
        bool IsDeleted { get; }

        string EntityStatus { get; }
        List<ValidationMessage> ValidationMessages { get; set; }
    }
}