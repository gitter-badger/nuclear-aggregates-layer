using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public abstract class EditableIdEntityViewModelBase<T> : EntityViewModelBase<T>
        where T : IEntityKey, new()
    {
        [NonZeroInteger]
        [RequiredLocalized]
        [OnlyDigitsLocalized]
        public override long Id { get; set; }
    }
}