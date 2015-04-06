using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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