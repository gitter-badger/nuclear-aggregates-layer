using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels
// ReSharper restore CheckNamespace
{
    public abstract partial class EntityViewModelBase : ViewModel, IEntityViewModelBase
    {
        public virtual long Id { get; set; }
        public List<ValidationMessage> ValidationMessages { get; set; }
    }
}
