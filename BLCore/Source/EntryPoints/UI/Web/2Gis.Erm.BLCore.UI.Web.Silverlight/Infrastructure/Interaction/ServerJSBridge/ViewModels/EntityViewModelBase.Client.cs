using System;
using System.Windows.Browser;

using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    [ScriptableType]
    public abstract partial class EntityViewModelBase
    {
        public virtual bool IsNew { get; set; }
        public string EntityStatus { get; set; }
        public LookupField CreatedBy { get; set; }
        public LookupField ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public virtual LookupField Owner { get; set; }
    }
}
